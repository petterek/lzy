Imports System.IO

Namespace Utils.Json
    Public Class Writer
        

        Public Shared Formatters As New Dictionary(Of Type, Writer) From {
            {GetType(Integer), Sub(w, val) w.write(val.ToString)},
            {GetType(Long), Sub(w, val) w.write(val.ToString)},
            {GetType(UInteger), Sub(w, val) w.write(val.ToString)},
            {GetType(ULong), Sub(w, val) w.write(val.ToString)},
            {GetType(Double), AddressOf WriteNumber},
            {GetType(Single), AddressOf WriteNumber},
            {GetType(String), AddressOf Writetext},
            {GetType(Decimal), AddressOf WriteNumber},
            {GetType(Date), AddressOf WriteDate },
            {GetType(Guid), AddressOf WriteText },
            {GetType(Boolean), Sub(w,val)
                                   If CBool(val)
                                       w.Write("true")
                                    Else
                                       w.Write("false")
                                   End If
                               End Sub}
        }

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

        Private Shared Sub ObjectToString(result As StreamWriter, o As Object)
            If o Is Nothing Then
                result.Write("null")
                Return
            End If
            If Formatters.ContainsKey(o.GetType) Then
                Formatters(o.GetType)(result, o)
                Return
            End If

            If TypeOf (o) Is IEnumerable Then
                WriteList(result, o)
            Else
                WriteObject(result, o)
            End If
        End Sub


        Public Shared Sub WriteObject(result As StreamWriter, o As Object)
            Dim first As Boolean = True
            result.Write("{"c)

            Dim allProps As New System.Collections.Concurrent.ConcurrentStack(Of String)
            
            o.GetType().GetMembers(system.Reflection.BindingFlags.Public Or system.Reflection.BindingFlags.Instance).Where(Function(v) v.MemberType = system.Reflection.MemberTypes.Field Or v.MemberType = system.Reflection.MemberTypes.Property).AsParallel.ForAll(
                Sub(m As system.Reflection.MemberInfo)
                    Dim memoryStream1 As MemoryStream = New System.IO.MemoryStream
                    Dim res As New StreamWriter(memoryStream1)
                    res.Write(Chr(&H22))
                    res.Write(m.Name)
                    res.Write(Chr(&H22))
                    res.Write(":"c)

                    Select Case m.MemberType
                        Case system.Reflection.MemberTypes.Field
                            Dim fld = o.GetType.GetField(m.Name)
                            WriteValue(res, fld.FieldType, fld.GetValue(o))
                        Case system.Reflection.MemberTypes.Property
                            Dim prop = o.GetType.GetProperty(m.Name)
                            WriteValue(res, prop.PropertyType, prop.GetValue(o))
                    End Select

                    res.Flush

                    memoryStream1.Seek(0,SeekOrigin.Begin)
                    allProps.Push(new StreamReader(memoryStream1).ReadToEnd)

                End Sub)
            result.Write(join(allProps.ToArray,","))
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

            If Formatters.ContainsKey(t) Then
                Formatters(t)(writer, value)
            Else
                If value.GetType.IsValueType Then
                    If value.GetType.GetMembers(System.Reflection.BindingFlags.DeclaredOnly Or system.Reflection.BindingFlags.Public Or system.Reflection.BindingFlags.Instance).Count > 0 Then
                        ObjectToString(writer, value)
                    Else
                        writer.Write(value.ToString)
                    End If
                Else
                    ObjectToString(writer, value)
                End If
            End If
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
            w.Write( d.ToString("yyyy-MM-ddTHH\:mm\:ss.FFFK" ))
            w.Write(Chr(34))
        End Sub

    End Class

    Public Delegate Function ToString(Of T)(value As T) As String
End Namespace
