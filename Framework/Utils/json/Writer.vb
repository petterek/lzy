Imports System.IO
Imports System.Linq.Expressions

Namespace Utils.Json
    Public Class Writer


        Public Shared Function Serializer(objType As Type) As Action(Of StreamWriter, Object)

            Dim exprns As New List(Of Expressions.Expression)

            Dim writer = Expression.Parameter(GetType(StreamWriter), "writer")
            Dim writeMethodInfo = GetType(StreamWriter).GetMethod("Write", {GetType(String)})
            Dim valueObject = Expression.Parameter(GetType(Object), "theObject")

            exprns.Add(Expression.Call(writer, writeMethodInfo, Expression.Constant("{")))
            Dim first As Boolean = True

            Dim writeToStream As System.Reflection.MethodInfo = Reflection.GetMethodInfo(Sub() WriteValue(Nothing, Nothing))

            If AddTypeInfoForObjects Then
                exprns.Add(Expression.Call(writer, writeMethodInfo, Expression.Constant("""$type$"":")))
                Dim typeString = Expression.Call(Nothing, TypeInfoWriter.Method, valueObject)
                exprns.Add(Expression.Call(Nothing, writeToStream, writer, typeString))
                exprns.Add(Expression.Call(writer, writeMethodInfo, Expression.Constant(Chr(34).ToString)))
                first = False
            End If

            For Each m In GetMembers(objType)
                If Not first Then
                    exprns.Add(Expression.Call(writer, writeMethodInfo, Expression.Constant(",")))
                End If
                first = False
                exprns.Add(Expression.Call(writer, writeMethodInfo, Expression.Constant(Chr(&H22) & m.Name & Chr(&H22) & ":")))
                Dim memberValue = Expression.PropertyOrField(Expression.Convert(valueObject, objType), m.Name)

                exprns.Add(Expression.Call(Nothing, writeToStream, writer, Expression.Convert(memberValue, GetType(Object))))
            Next

            exprns.Add(Expression.Call(writer, writeMethodInfo, Expression.Constant("}")))

            Dim block = Expression.Block(exprns.ToArray)



            Dim expression1 As Expression(Of Action(Of StreamWriter, Object)) = Expression.Lambda(Of Action(Of StreamWriter, Object))(block, writer, valueObject)
            expression1.Reduce()
            Return expression1.Compile()
        End Function


        Public Delegate Function GetTypeInfo(t As Object) As String

        Public Shared Formatters As New Dictionary(Of Type, Writer) From {
            {GetType(Integer), Sub(w, val) w.Write(val.ToString)},
            {GetType(Long), Sub(w, val) w.Write(val.ToString)},
            {GetType(UInteger), Sub(w, val) w.Write(val.ToString)},
            {GetType(ULong), Sub(w, val) w.Write(val.ToString)},
            {GetType(Double), AddressOf WriteNumber},
            {GetType(Single), AddressOf WriteNumber},
            {GetType(String), AddressOf Writetext},
            {GetType(Decimal), AddressOf WriteNumber},
            {GetType(Date), AddressOf WriteDate},
            {GetType(Guid), AddressOf Writetext},
            {GetType(Boolean), Sub(w, val)
                                   If CBool(val) Then
                                       w.Write("true")
                                   Else
                                       w.Write("false")
                                   End If
                               End Sub}
        }

        Public Shared AddTypeInfoForObjects As Boolean
        Public Shared TypeInfoWriter As GetTypeInfo = AddressOf DefaultTypeInfoWriter

        Public Shared Function DefaultTypeInfoWriter(t As Object) As String
            Return t.GetType.FullName
        End Function


        Public Delegate Sub Writer(writer As StreamWriter, value As Object)

        Public Shared Function ObjectToString(o As Object) As String
            Return ObjectToString(New JSonConfig, o)
        End Function

        Public Shared Function ObjectToString(config As JSonConfig, o As Object) As String
            Dim result = New StreamWriter(New MemoryStream, Text.Encoding.UTF8)
            ObjectToString(result, o)
            result.Flush()
            result.BaseStream.Position = 0
            Return New StreamReader(result.BaseStream).ReadToEnd
        End Function

        Public Shared Function Config() As JSonConfig
            Return New JSonConfig
        End Function

        Public Shared Sub ObjectToString(result As StreamWriter, o As Object)
            If o Is Nothing Then
                result.Write("null")
                Return
            End If

            If Formatters.ContainsKey(o.GetType) Then
                Formatters(o.GetType)(result, o)
            Else
                If TypeOf (o) Is IDictionary Then
                    WriteDictionary(result, o)
                ElseIf TypeOf (o) Is IEnumerable Then
                    WriteList(result, o)
                Else
                    WriteObject(result, o)
                End If
            End If

            result.Flush()
        End Sub

        Private Shared Sub WriteDictionary(result As StreamWriter, o As Object)
            result.Write("{")
            Dim first As Boolean = True
            For Each value As DictionaryEntry In CType(o, IDictionary)
                If Not first Then
                    result.Write(",")
                End If
                result.Write(Chr(&H22) & value.Key.ToString & Chr(&H22))
                result.Write(":")
                ObjectToString(result, value.Value)
                first = False
            Next

            result.Write("}")
        End Sub

        Private Shared padLock As New Object
        Private Shared typeinfoCache As New Dictionary(Of Type, List(Of System.Reflection.MemberInfo))
        Private Shared Function GetMembers(t As Type) As IEnumerable(Of System.Reflection.MemberInfo)
            If Not typeinfoCache.ContainsKey(t) Then
                SyncLock padLock
                    If Not typeinfoCache.ContainsKey(t) Then
                        typeinfoCache.Add(t, t.GetMembers(System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Instance).Where(Function(v) v.MemberType = System.Reflection.MemberTypes.Field Or v.MemberType = System.Reflection.MemberTypes.Property).ToList)
                    End If
                End SyncLock
            End If
            Return typeinfoCache(t)
        End Function


        Private Shared funcCache As New Dictionary(Of Type, Action(Of StreamWriter, Object))
        Private Shared funcCacheLock As New Object

        Private Shared Sub WriteObject(result As StreamWriter, o As Object)

            If Not funcCache.ContainsKey(o.GetType) Then
                SyncLock funcCacheLock
                    If Not funcCache.ContainsKey(o.GetType) Then
                        funcCache(o.GetType) = Serializer(o.GetType())
                    End If
                End SyncLock
            End If

            funcCache(o.GetType)(result, o)
            Return
        End Sub

        Private Shared Sub WriteList(result As StreamWriter, o As Object)
            Dim first As Boolean = True
            result.Write("[")
            For Each element In CType(o, IEnumerable)
                If Not first Then
                    result.Write(","c)
                End If
                ObjectToString(result, element)
                first = False
            Next
            result.Write("]")
        End Sub

        Private Shared Sub WriteValue(writer As StreamWriter, value As Object)

            If value Is Nothing Then
                writer.Write("null")
                Return
            End If

            Dim t As System.Type = value.GetType()

            If value.GetType.IsEnum Then
                t = value.GetType.GetEnumUnderlyingType
                value = CTypeDynamic(value, t)
            End If

            If Formatters.ContainsKey(t) Then
                Formatters(t)(writer, value)
            Else
                If value.GetType.IsValueType Then

                    If value.GetType.GetMembers(System.Reflection.BindingFlags.DeclaredOnly Or System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Instance).Count > 0 Then
                        ObjectToString(writer, value)
                    Else
                        writer.Write(value.ToString)
                    End If


                Else
                    ObjectToString(writer, value)
                End If
            End If
        End Sub

        Private Shared Sub WriteDictionary(writer As StreamWriter, t As Type, value As IDictionary)



        End Sub

#Region "WriteText"
        Private Shared ReadOnly ToEscape As Integer() = {&H22, &H2, &H5C}
        Private Shared Translate As New Dictionary(Of Integer, String) From {
            {&H9, "\t"}, {&HA, "\n"}, {&HC, "\f"}, {&HD, "\r"}}


        Private Shared Sub Writetext(writer As StreamWriter, value As Object)
            writer.Write(Chr(&H22))
            For Each c In value.ToString
                If ToEscape.Contains(Strings.AscW(c)) Then
                    writer.Write("\")
                End If
                If Translate.ContainsKey(AscW(c)) Then
                    writer.Write(Translate(AscW(c)))
                Else
                    writer.Write(c)
                End If
            Next
            writer.Write(Chr(&H22))
        End Sub
#End Region

        Private Shared Sub WriteNumber(w As StreamWriter, val As Object)
            w.Write(val.ToString.Replace(","c, "."))
        End Sub
        Private Shared Sub WriteDate(w As StreamWriter, value As Object)
            Dim d As Date = DirectCast(value, Date)
            w.Write(Chr(34))
            w.Write(d.ToString("yyyy-MM-ddTHH\:mm\:ss.FFFK"))
            w.Write(Chr(34))
        End Sub

    End Class

    Public Delegate Function ToString(Of T)(value As T) As String
End Namespace
