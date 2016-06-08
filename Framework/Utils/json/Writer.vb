Imports System.IO

Namespace Utils.Json
    Public Class Writer

        Public Delegate Function GetTypeInfo(t As Type) As String

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

        Public Shared Function DefaultTypeInfoWriter(t As Type) As String
            Return t.FullName
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
                Return
            End If

            If TypeOf (o) Is IDictionary Then
                WriteDictionary(result, o)
            ElseIf TypeOf (o) Is IEnumerable Then
                WriteList(result, o)
            Else
                WriteObject(result, o)
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
                result.Write(Chr(&H22))
                result.Write(value.Key.ToString)
                result.Write(Chr(&H22))
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

        Private Shared Sub WriteObject(result As StreamWriter, o As Object)
            Dim first As Boolean = True
            result.Write("{"c)

            Dim allProps As New System.Collections.Concurrent.ConcurrentStack(Of String)

            If AddTypeInfoForObjects Then
                allProps.Push("""$type$"":""" & TypeInfoWriter(o.GetType) & """")
            End If

            GetMembers(o.GetType()).AsParallel.ForAll(
                Sub(m As System.Reflection.MemberInfo)
                    Dim memoryStream1 As MemoryStream = New System.IO.MemoryStream
                    Dim res As New StreamWriter(memoryStream1)
                    res.Write(Chr(&H22))
                    res.Write(m.Name)
                    res.Write(Chr(&H22))
                    res.Write(":"c)

                    Select Case m.MemberType
                        Case System.Reflection.MemberTypes.Field
                            Dim fld = o.GetType.GetField(m.Name)
                            WriteValue(res, fld.FieldType, fld.GetValue(o))
                        Case System.Reflection.MemberTypes.Property
                            Dim prop = o.GetType.GetProperty(m.Name)
                            WriteValue(res, prop.PropertyType, prop.GetValue(o))
                    End Select

                    res.Flush()

                    memoryStream1.Seek(0, SeekOrigin.Begin)
                    allProps.Push(New StreamReader(memoryStream1).ReadToEnd)

                End Sub)
            result.Write(Join(allProps.ToArray, ","))
            result.Write("}"c)
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

        Private Shared Sub WriteValue(writer As StreamWriter, t As System.Type, value As Object)

            If value Is Nothing Then
                writer.Write("null")
                Return
            End If

            if value.GetType.IsEnum Then
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
