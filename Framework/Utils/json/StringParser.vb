Namespace Utils.Json
    Public Class StringParser
        Inherits Builder
        Public Sub New()
            MyBase.New(GetType(String))
        End Sub

        Public Delegate Function TransformText(reader As IReader) As String
        Public Delegate Function Match(input As String) As Boolean

        Private Shared ReadOnly ML As New MatchList From {
            {Function(inp) Inp = "\\", Function(r As IReader)
                                           r.ClearBuffer()
                                           Return "\"
                                       End Function},
            {Function(inp) inp = "\n", Function(r As IReader)
                                           r.ClearBuffer()
                                           Return vbNewLine
                                       End Function},
            {Function(inp) inp = "\t", Function(r As IReader)
                                           r.ClearBuffer()
                                           Return vbTab
                                       End Function},
            {Function(inp) inp = "\/", Function(r As IReader)
                                           r.ClearBuffer()
                                           Return "/"
                                       End Function},
            {Function(inp) inp = "\b", Function(r As IReader)
                                           r.ClearBuffer()
                                           Return vbBack
                                       End Function},
            {Function(inp) inp = "\f", Function(r As IReader)
                                           r.ClearBuffer()
                                           Return vbFormFeed
                                       End Function},
            {Function(inp) inp = "\r", Function(r As IReader)
                                           r.ClearBuffer()
                                           Return vbCr
                                       End Function},
            {Function(inp As String)
                 If inp.StartsWith("\u") Then
                     For x = 1 To inp.Length
                         If AscW(inp(x)) < 48 OrElse AscW(inp(x)) > 57 Then
                             Return False
                         End If
                     Next
                 End If

             End Function, Function(r As IReader)
                               Dim toTrans = r.Buffer
                               Return "" 'ChrW()
                           End Function}
            }

        Public Overrides Function Parse(nextChar As IReader) As Object
            Dim buffer As New Text.StringBuilder
            TokenAcceptors.WhiteSpace(nextChar)
            TokenAcceptors.Quote(nextChar)

            Dim cur = nextChar.PeekToBuffer
            While cur <> Chr(34)
                If cur = "\" Then 'Not allowed, is the start of an escape
                    buffer.Append(nextChar.Buffer)

                    cur = nextChar.PeekToBuffer()
                    While ML.ContainsKey(nextChar.BufferPeek)
                        cur = nextChar.PeekToBuffer()
                    End While
                    buffer.Append(ML(nextChar.BufferPreLastPeek)(nextChar))
                Else
                    cur = nextChar.PeekToBuffer
                End If
            End While
            buffer.Append(nextChar.Buffer)
            TokenAcceptors.Quote(nextChar)

            Return buffer.ToString
        End Function

        Private Class MatchList
            Inherits Dictionary(Of Match, TransformText)

            Overloads Function ContainsKey(s As String) As Boolean
                For Each key In Me.Keys
                    If key(s) Then
                        Return True
                    End If
                Next
                Return False
            End Function

            Default Overloads Property Item(s As String) As TransformText
                Get
                    For Each i In Me
                        If i.Key(s) Then
                            Return i.Value
                        End If
                    Next
                    Throw New KeyNotFoundException
                End Get
                Set(value As TransformText)
                    For Each i In Me
                        If i.Key(s) Then
                            'i.Value = value
                        End If
                    Next
                    Throw New KeyNotFoundException
                End Set
            End Property



        End Class

    End Class


End Namespace