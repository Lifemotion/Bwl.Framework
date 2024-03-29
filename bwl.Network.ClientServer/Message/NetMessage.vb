﻿'Сетевое сообщение
Imports Bwl.Framework
Public Class NetMessage
    Public Property DataType As Char
    Public Property FromID As String = ""
    Public Property ToID As String = ""
    Public Property ServiceName As String = ""

    Private parts()() As Byte
    Private partCount As Integer

    Private Shared Function StringToDouble(str As String) As Double
        If str Is Nothing Then Throw New Exception
        Dim result As Double = 0.0
        str = str.Trim.Replace(" ", "")
        If Double.TryParse(str, result) Then Return result
        If Double.TryParse(str.Replace(",", "."), result) Then Return result
        If Double.TryParse(str.Replace(".", ","), result) Then Return result
        Throw New Exception
    End Function

    Private Shared Function StringToDoubleSafe(str As String) As Double
        Try
            Return StringToDouble(str)
        Catch ex As Exception
            Return 0.0
        End Try
    End Function

    Private Shared Function StringToBooleanSafe(str As String) As Boolean
        If str Is Nothing Then Return False
        If str.Trim.ToLower = "true" Then Return True
        If str.Trim.ToLower = "1" Then Return True
        If str.Trim.ToLower = "yes" Then Return True
        Return False
    End Function
    ''' <summary>
    ''' Кодирует последовательность байтов так, что в ней больше не встречаются символы двоеточия и все байты меньше 6.
    ''' </summary>
    ''' <param name="bytes"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Каждый указанный байт заменяется на два: #5 и #(N+6)
    ''' </remarks>
    ''' 
    Private Function CodeBytes(ByVal bytes() As Byte) As Byte()
        Dim result(bytes.Length * 2 - 1) As Byte
        Dim i As Integer
        Dim resultPos As Integer = 0
        For i = 0 To bytes.Length - 1
            If bytes(i) <= 5 Or bytes(i) = 58 Then
                result(resultPos) = 5
                result(resultPos + 1) = bytes(i) + 6
                resultPos += 2
            Else
                result(resultPos) = bytes(i)
                resultPos += 1
            End If
        Next
        ReDim Preserve result(resultPos - 1)
        Return result
    End Function
    Private Function DecodeBytes(ByVal bytes() As Byte) As Byte()
        Dim result(bytes.Length - 1) As Byte
        Dim i As Integer
        Dim resultPos As Integer = 0
        For i = 0 To bytes.Length - 1
            If bytes(i) = 5 AndAlso i < bytes.Length - 1 AndAlso bytes(i + 1) > 5 Then
                result(resultPos) = bytes(i + 1) - 6
                i = i + 1
            Else
                result(resultPos) = bytes(i)
            End If
            resultPos += 1
        Next
        ReDim Preserve result(resultPos - 1)
        Return result
    End Function
    Sub New(ByVal fromRawBytes() As Byte)
        Dim i As Integer
        Dim part() As Byte
        Dim partStart As Integer = 1
        Dim count = 1
        For i = 1 To fromRawBytes.Length - 1
            If fromRawBytes(i) = 58 Then count += 1
        Next
        ReDim parts(count - 1)
        DataType = System.Text.Encoding.ASCII.GetChars(fromRawBytes, 0, 1)(0)
        Dim currentPart As Integer = 0
        For i = 1 To fromRawBytes.Length
            If i = fromRawBytes.Length OrElse fromRawBytes(i) = 58 Then
                ReDim part(i - partStart - 1)
                ReDim parts(currentPart)(i - partStart - 1)
                Array.ConstrainedCopy(fromRawBytes, partStart, part, 0, i - partStart)
                parts(currentPart) = DecodeBytes(part)
                currentPart += 1
                partStart = i + 1
            End If
        Next
        partCount = count
        If partCount > 0 Then
            Dim lastPart = parts(partCount - 1)
            If lastPart.Length > 6 Then
                If lastPart(0) = 35 And lastPart(1) = 37 And lastPart(2) = 126 Then
                    Dim txtParts = StringTools.СP1251GetString(lastPart).Split({"#%~"}, StringSplitOptions.None)
                    If txtParts.Length = 6 AndAlso txtParts(1) = "ADDRESSES" Then
                        FromID = txtParts(2)
                        ToID = txtParts(3)
                        ServiceName = txtParts(4)
                        partCount -= 1
                    End If
                End If
            End If
        End If
    End Sub
    Sub New()

    End Sub
    Sub New(ByVal partsCount As Integer)
        partCount = partsCount
        ReDim parts(partsCount - 1)
    End Sub

    Public Class Adresses
        Public Property FromID As String = ""
        Public Property ToID As String = ""
        Public Property ServiceName As String = ""
        Public Sub New()
        End Sub
        Public Sub New(fromId As String, toId As String, serviceName As String)
            Me.FromID = fromId
            Me.ToID = toId
            Me.ServiceName = serviceName
        End Sub
    End Class

    Sub New(addresses As Adresses, ByVal ParamArray newParts() As String)
        Me.New("S", addresses, newParts)
    End Sub

    Sub New(ByVal newDataType As Char, addresses As Adresses, ByVal ParamArray newParts() As String)
        ToID = addresses.ToID
        FromID = addresses.FromID
        ServiceName = addresses.ServiceName
        DataType = newDataType
        partCount = newParts.Length
        ReDim parts(partCount - 1)
        Dim i As Integer
        For i = 0 To partCount - 1
            parts(i) = StringTools.СP1251GetBytes(newParts(i))
        Next
    End Sub

    Sub New(inReplyToMessage As NetMessage, ByVal ParamArray newParts() As String)
        DataType = inReplyToMessage.DataType
        ToID = inReplyToMessage.FromID
        FromID = inReplyToMessage.ToID
        ServiceName = inReplyToMessage.ServiceName
        partCount = newParts.Length
        ReDim parts(partCount - 1)
        Dim i As Integer
        For i = 0 To partCount - 1
            parts(i) = StringTools.СP1251GetBytes(newParts(i))
        Next
    End Sub

    Sub New(ByVal newDataType As Char, ByVal ParamArray newParts() As String)
        DataType = newDataType
        partCount = newParts.Length
        ReDim parts(partCount - 1)
        Dim i As Integer
        For i = 0 To partCount - 1
            parts(i) = StringTools.СP1251GetBytes(newParts(i))
        Next
    End Sub

    Public Property Part(ByVal index As Integer) As String
        Get
            Return StringTools.СP1251GetString(PartBytes(index))
        End Get
        Set(ByVal value As String)
            PartBytes(index) = StringTools.СP1251GetBytes(value)
        End Set
    End Property
    Public Property PartDouble(ByVal index As Integer) As Double
        Get
            Return StringToDoubleSafe(Part(index))
        End Get
        Set(ByVal value As Double)
            Part(index) = value.ToString("0.0")
        End Set
    End Property
    Public Property PartBoolean(ByVal index As Integer) As Boolean
        Get
            Return StringToBooleanSafe(Part(index))
        End Get
        Set(ByVal value As Boolean)
            Part(index) = value.ToString
        End Set
    End Property
    Public Property PartBytes(ByVal index As Integer) As Byte()
        Get
            If (index < 0 Or index > partCount - 1) OrElse parts(index) Is Nothing Then
                Dim empty(-1) As Byte
                Return empty
            Else
                Return parts(index)
            End If
        End Get
        Set(ByVal value As Byte())
            If index < 0 Then Throw New Exception("Номер поля не может быть меньше нуля!")
            If index > partCount - 1 Then
                Count = index + 1
            End If
            parts(index) = value
        End Set
    End Property
    Property Count() As Integer
        Get
            Return partCount
        End Get
        Set(ByVal value As Integer)
            partCount = value
            ReDim Preserve parts(partCount - 1)
            For i = 0 To partCount - 1
                If parts(i) Is Nothing Then ReDim parts(i)(-1)
            Next
        End Set
    End Property
    Public Overrides Function ToString() As String
        Dim result As String = ""
        Dim i As Integer
        For i = 0 To partCount - 1
            result += StringTools.СP1251GetString(parts(i))
            If i < partCount - 1 Then result += ":"
        Next
        Return result
    End Function
    Public ReadOnly Property AsString()
        Get
            Return ToString()
        End Get
    End Property
    'хорошо живет на свете кот
    Public Function ToBytes(Optional ByVal rightLeftOffset As Integer = 0) As Byte()
        Dim length As Integer = rightLeftOffset * 2
        Dim coded(partCount)() As Byte
        For i = 0 To partCount - 1
            coded(i) = CodeBytes(parts(i))
            length += coded(i).Length + 1
        Next
        length -= 1
        Dim result(length) As Byte
        Dim position As Integer
        result(rightLeftOffset) = System.Text.Encoding.ASCII.GetBytes({DataType}, 0, 1)(0)
        position = rightLeftOffset + 1
        For i = 0 To partCount - 1
            If i > 0 Then result(position - 1) = 58
            Array.ConstrainedCopy(coded(i), 0, result, position, coded(i).Length)
            position += coded(i).Length + 1
        Next
        If FromID > "" Or ToID > "" Or ServiceName > "" Then
            Dim addrString = "#%~ADDRESSES#%~" + FromID + "#%~" + ToID + "#%~" + ServiceName + "#%~"
            Dim addrBytes = CodeBytes(StringTools.СP1251GetBytes(addrString))
            ReDim Preserve result(result.Length + addrBytes.Length)
            If position > rightLeftOffset + 1 Then result(position - 1) = 58
            Array.ConstrainedCopy(addrBytes, 0, result, position, addrBytes.Length)
        End If
        Return result
    End Function
    Public Function GetCopy() As NetMessage
        Dim newMessage As New NetMessage(Me.partCount)
        For i = 0 To Me.partCount - 1
            ReDim newMessage.parts(i)(Me.parts(i).Length - 1)
            Array.Copy(Me.parts(i), newMessage.parts(i), Me.parts(i).Length)
        Next
        newMessage.DataType = Me.DataType
        Return newMessage
    End Function



End Class
