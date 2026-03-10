<AttributeUsage(AttributeTargets.Method, AllowMultiple:=True)>
Public Class HandlesAttribute
    Inherits Attribute

    ''' <summary>
    ''' The name of the event to handle
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly Property EventName As String

    ''' <summary>
    ''' The name of the object that contains the event source
    ''' </summary>
    Public ReadOnly Property ContainerName As String = This

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
    ''' <param name="eventName"></param>
    Public Sub New(eventName As String)
        Me.SourceName = This
        Me.EventName = eventName
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sourceName"></param>
    ''' <param name="eventName"></param>
    Public Sub New(sourceName As String, eventName As String)
        Me.SourceName = sourceName
        Me.EventName = eventName
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="containerName"></param>
    ''' <param name="sourceName"></param>
    ''' <param name="eventName"></param>
    Public Sub New(containerName As String, sourceName As String, eventName As String)
        Me.ContainerName = containerName
        Me.EventName = eventName
        Me.SourceName = sourceName
    End Sub

End Class

Public MustInherit Class HandleAutoRegister
    Private _handlersRegistered As Boolean

    Protected Sub RegisterAutoHandlers()
        If _handlersRegistered Then
            Return
        End If

        Handlers.RegisterHandlers(Me)
        _handlersRegistered = True
    End Sub

    Protected Overrides Sub Finalize()
        Try
            If _handlersRegistered Then
                Handlers.UnregisterHandlers(Me)
            End If
        Finally
            MyBase.Finalize()
        End Try
    End Sub
End Class

Public Module Handlers
    Private ReadOnly BindingFlagsToSearch As Reflection.BindingFlags = Reflection.BindingFlags.Instance Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.NonPublic

    Private Function ResolveMemberValue(sourceType As Type, sourceInstance As Object, memberName As String) As Object
        Dim field = sourceType.GetField(memberName, BindingFlagsToSearch)

        If field IsNot Nothing Then
            Return field.GetValue(sourceInstance)
        End If

        Dim prop = sourceType.GetProperty(memberName, BindingFlagsToSearch)
        If prop IsNot Nothing Then
            Return prop.GetValue(sourceInstance)
        End If

        Return Nothing
    End Function

    Private Function ResolveSource(instance As Object, attr As HandlesAttribute, methodSignature As String, throwIfMissing As Boolean) As Object
        Dim container As Object = instance

        If attr.ContainerName <> HandlesAttribute.This Then
            container = ResolveMemberValue(instance.GetType(), instance, attr.ContainerName)

            If container Is Nothing Then
                If throwIfMissing Then
                    Throw New InvalidOperationException($"Container '{attr.ContainerName}' not found for method {methodSignature} with Handles attribute for event '{attr.EventName}'")
                End If

                Return Nothing
            End If
        End If

        If attr.SourceName = HandlesAttribute.This Then
            Return container
        End If

        Dim source = ResolveMemberValue(container.GetType(), container, attr.SourceName)
        If source Is Nothing AndAlso throwIfMissing Then
            Throw New InvalidOperationException($"Source '{attr.SourceName}' not found for method {methodSignature} with Handles attribute for event '{attr.EventName}'")
        End If

        Return source
    End Function

    Public Sub RegisterHandlers(instance As Object)
        Dim instanceType = instance.GetType()

        ' Get all methods with the Handles attribute
        For Each method In instanceType.GetMethods(BindingFlagsToSearch)
            Dim attributes = method.GetCustomAttributes(GetType(HandlesAttribute), True)
            For Each attr As HandlesAttribute In attributes
                Dim methodSignature = $"{instanceType.FullName}.{method.Name}"
                Dim source = ResolveSource(instance, attr, methodSignature, True)

                ' Find the event
                Dim eventInfo = source.GetType().GetEvent(attr.EventName, BindingFlagsToSearch)
                If eventInfo Is Nothing Then
                    Throw New InvalidOperationException($"Event '{attr.EventName}' not found in type '{source.GetType().FullName}' for method {methodSignature}")
                End If

                ' Check if the event has an add method
                Dim addMethod = eventInfo.GetAddMethod(True)
                If addMethod Is Nothing Then
                    Throw New InvalidOperationException($"Cannot add handler for event '{attr.EventName}' in type '{source.GetType().FullName}' for method {methodSignature} because the event doesn't have an add accessor")
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
        For Each method In instanceType.GetMethods(BindingFlagsToSearch)
            Dim attributes = method.GetCustomAttributes(GetType(HandlesAttribute), True)
            For Each attr As HandlesAttribute In attributes
                Try
                    Dim methodSignature = $"{instanceType.FullName}.{method.Name}"
                    Dim source = ResolveSource(instance, attr, methodSignature, False)
                    If source Is Nothing Then
                        Continue For
                    End If

                    ' Find the event
                    Dim eventInfo = source.GetType().GetEvent(attr.EventName, BindingFlagsToSearch)
                    If eventInfo Is Nothing Then
                        Continue For  ' Skip if event not found
                    End If

                    Dim removeMethod = eventInfo.GetRemoveMethod(True)
                    If removeMethod Is Nothing Then
                        Continue For
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
