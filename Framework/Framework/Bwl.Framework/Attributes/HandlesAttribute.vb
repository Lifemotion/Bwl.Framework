<AttributeUsage(AttributeTargets.Method, AllowMultiple:=True)>
Public Class HandlesAttribute
    Inherits Attribute

    ''' <summary>
    ''' The instance of the object that contains the event
    ''' </summary>
    ''' <remarks></remarks>
    Private _instance As Object

    ''' <summary>
    ''' The name of the event to handle
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly Property EventName As String

    ''' <summary>
    ''' The name of the source object that contains the event
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly Property SourceName As String

    ''' <summary>
    ''' Special constant to reference the containing object itself
    ''' </summary>
    Public Const This As String = "$this"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="instance"></param>
    ''' <param name="sourceName"></param>
    ''' <param name="eventName"></param>
    Public Sub New(instance As Object, sourceName As String, eventName As String)
        Me.EventName = eventName
        Me.SourceName = sourceName
    End Sub

End Class

Public MustInherit Class HandleAutoRegister
    Protected Sub New()
        Handlers.RegisterHandlers(Me)
    End Sub

    Protected Overrides Sub Finalize()
        Handlers.UnregisterHandlers(Me)
    End Sub
End Class

Public Module Handlers
    Public Sub RegisterHandlers(instance As Object)
        Dim instanceType = instance.GetType()

        ' Get all methods with the Handles attribute
        For Each method In instanceType.GetMethods(Reflection.BindingFlags.Instance Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.NonPublic)
            Dim attributes = method.GetCustomAttributes(GetType(HandlesAttribute), True)
            For Each attr As HandlesAttribute In attributes
                Dim source As Object = Nothing
                Dim methodSignature = $"{instanceType.FullName}.{method.Name}"

                ' Handle special case for "this"/"me"
                If attr.SourceName = HandlesAttribute.This Then
                    source = instance
                Else
                    ' Find the source field or property
                    Dim field = instanceType.GetField(attr.SourceName, Reflection.BindingFlags.Instance Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.NonPublic)

                    If field IsNot Nothing Then
                        source = field.GetValue(instance)
                    Else
                        ' Try as property if not found as field
                        Dim prop = instanceType.GetProperty(attr.SourceName, Reflection.BindingFlags.Instance Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.NonPublic)
                        If prop IsNot Nothing Then
                            source = prop.GetValue(instance)
                        End If
                    End If

                    If source Is Nothing Then
                        Throw New InvalidOperationException($"Source '{attr.SourceName}' not found for method {methodSignature} with Handles attribute for event '{attr.EventName}'")
                    End If
                End If

                ' Find the event
                Dim eventInfo = source.GetType().GetEvent(attr.EventName)
                If eventInfo Is Nothing Then
                    Throw New InvalidOperationException($"Event '{attr.EventName}' not found in type '{source.GetType().FullName}' for method {methodSignature}")
                End If

                ' Check if the event has a public add method
                Dim addMethod = eventInfo.GetAddMethod(False) ' Only check public methods
                If addMethod Is Nothing Then
                    Throw New InvalidOperationException($"Cannot add handler for event '{attr.EventName}' in type '{source.GetType().FullName}' for method {methodSignature} because the event doesn't have a public add accessor")
                End If

                Try
                    ' Create delegate and wire up
                    Dim handler = [Delegate].CreateDelegate(eventInfo.EventHandlerType, instance, method)
                    eventInfo.AddEventHandler(source, handler)
                Catch ex As Exception
                    ' Provide more detail about the failure
                    Throw New InvalidOperationException($"Failed to wire event '{attr.EventName}' to method {methodSignature}: {ex.Message}", ex)
                End Try
            Next
        Next
    End Sub

    Public Sub UnregisterHandlers(instance As Object)
        Dim instanceType = instance.GetType()
        Dim failures As New List(Of Exception)()

        ' Get all methods with the Handles attribute
        For Each method In instanceType.GetMethods(Reflection.BindingFlags.Instance Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.NonPublic)
            Dim attributes = method.GetCustomAttributes(GetType(HandlesAttribute), True)
            For Each attr As HandlesAttribute In attributes
                Try
                    Dim source As Object = Nothing
                    Dim methodSignature = $"{instanceType.FullName}.{method.Name}"

                    ' Handle special case for "this"/"me"
                    If attr.SourceName = HandlesAttribute.This Then
                        source = instance
                    Else
                        ' Find the source field or property
                        Dim field = instanceType.GetField(attr.SourceName, Reflection.BindingFlags.Instance Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.NonPublic)

                        If field IsNot Nothing Then
                            source = field.GetValue(instance)
                        Else
                            ' Try as property if not found as field
                            Dim prop = instanceType.GetProperty(attr.SourceName, Reflection.BindingFlags.Instance Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.NonPublic)
                            If prop IsNot Nothing Then
                                source = prop.GetValue(instance)
                            End If
                        End If

                        ' Skip if source not found rather than throwing
                        If source Is Nothing Then
                            Continue For
                        End If
                    End If

                    ' Find the event
                    Dim eventInfo = source.GetType().GetEvent(attr.EventName)
                    If eventInfo Is Nothing Then
                        Continue For  ' Skip if event not found
                    End If

                    ' Create delegate and try to remove
                    Dim handler = [Delegate].CreateDelegate(eventInfo.EventHandlerType, instance, method)
                    eventInfo.RemoveEventHandler(source, handler)
                Catch ex As Exception
                    ' Collect exceptions but don't fail immediately
                    failures.Add(ex)
                End Try
            Next
        Next

        ' If any unregistrations failed, report them
        If failures.Count > 0 Then
            Throw New AggregateException("Failed to unregister one or more event handlers", failures)
        End If
    End Sub
End Module
