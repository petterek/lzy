Imports LazyFramework.CQRS.Dto

Namespace Transform
    Public Class Handling

        Public Shared Function TransformResult(context As ExecutionProfile, ByVal result As Object) As Object

            'Hmmmm skal vi ha logikk her som sjekker om det er noe factory, og hvis det ikke er det bare returnere det den fikk inn. 
            'Egentlig er det jo bare commands som trenger dette. Queries bør jo gjøre dette selv.. Kanskje. 

            If result Is Nothing Then
                Return Nothing
            End If

            If TypeOf result Is IList Then
                Dim ret As New Concurrent.ConcurrentQueue(Of Object)
                Dim res As Object

                If Not context.RunAsParallel OrElse Setup.ChickenMode Then

                    For Each e In CType(result, IList)
                        res = Transform(context, e)
                        If res IsNot Nothing Then
                            ret.Enqueue(res)
                        End If
                    Next
                Else
                    Dim Errors As New Concurrent.ConcurrentBag(Of Exception)
                    CType(result, IList).
                        Cast(Of Object).
                        AsParallel.AsOrdered.ForAll(Sub(o As Object)
                                                        Try
                                                            Dim temp = Transform(context, o)
                                                            If temp IsNot Nothing Then
                                                                ret.Enqueue(temp)
                                                            End If
                                                        Catch ex As Exception
                                                            Errors.Add(ex)
                                                        End Try
                                                    End Sub)

                    If Errors.Count > 0 Then
                        Throw Errors(0)
                    End If
                End If

                Dim retList = ret.ToList
                If context.Sorter IsNot Nothing Then
                    retList.Sort(context.Sorter)
                End If
                Return retList
            Else
                Dim temp = Transform(context, result)
                Return temp
            End If
        End Function

        Public Shared Function Transform(ctx As ExecutionProfile, e As Object) As Object

            If ctx.ActionSecurity IsNot Nothing Then
                If Not ctx.ActionSecurity.EntityIsAvailableForUser(e) Then Return Nothing
            End If

            If ctx.Transformer Is Nothing Then Return e

            Dim transformEntity As Object = ctx.Transformer(e)
            If transformEntity Is Nothing Then Return Nothing

            If TypeOf (transformEntity) Is ISupportActionNameList AndAlso ctx.ActionSecurity IsNot Nothing Then
                CType(transformEntity, ISupportActionNameList).Actions.AddRange(ctx.ActionSecurity.GetActionList(e))
            End If

            Return transformEntity
        End Function
    End Class
End Namespace


