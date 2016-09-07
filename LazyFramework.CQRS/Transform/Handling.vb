Imports LazyFramework.CQRS.Dto

Namespace Transform
    Public Class Handling

        Public Shared Function TransformResult(context As ExecutionProfile, ByVal result As Object) As Object

            'Hmmmm skal vi ha logikk her som sjekker om det er noe factory, og hvis det ikke er det bare returnere det den fikk inn. 
            'Egentlig er det jo bare commands som trenger dette. Queries bør jo gjøre dette selv.. Kanskje. 

            If TypeOf result Is IList Then
                Dim ret As New Concurrent.ConcurrentQueue(Of Object)
                Dim res As Object

                If context.Transformer Is Nothing Then Return result

                If Not context.Transformer.RunAsParallel OrElse Setup.ChickenMode Then

                    For Each e In CType(result, IList)
                        res = Transform(context, e)
                        If TypeOf (res) Is ISupportActionList Then
                            CType(res, ISupportActionList).Actions.AddRange(context.ActionSecurity.GetActionList(e))
                        End If
                        If res IsNot Nothing Then
                            ret.Enqueue(res)
                        End If
                    Next
                    Return ret.ToList
                Else

                    Dim Errors As New Concurrent.ConcurrentBag(Of Exception)

                    CType(result, IList).
                        Cast(Of Object).
                        AsParallel.ForAll(Sub(o As Object)
                                              Try
                                                  Dim temp = Transform(context, o)
                                                  If TypeOf (temp) Is ISupportActionList Then
                                                      CType(temp, ISupportActionList).Actions.AddRange(context.ActionSecurity.GetActionList(o))
                                                  End If

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

                    Dim retList = ret.ToList
                    If context.Transformer.ObjectComparer IsNot Nothing Then
                        retList.Sort(context.Transformer.ObjectComparer)
                    End If

                    Return retList
                End If
            Else
                Dim temp = Transform(context, result)
                If TypeOf (temp) Is ISupportActionList Then
                    CType(temp, ISupportActionList).Actions.AddRange(context.ActionSecurity.GetActionList(result))
                End If
                Return temp
            End If
        End Function

        Public Shared Function Transform(ctx As ExecutionProfile, e As Object) As Object
            'Dim securityContext As Object

            If ctx.Transformer Is Nothing OrElse ctx.Transformer.GetTransformer(e) Is Nothing Then Return e

            'If TypeOf (e) Is IProvideSecurityContext Then
            '    securityContext = DirectCast(e, IProvideSecurityContext).Context
            'Else
            '    securityContext = e
            'End If

            If ctx.ActionSecurity IsNot Nothing Then
                If Not ctx.ActionSecurity.EntityIsAvailableForUser(e) Then Return Nothing
            End If

            Dim transformEntity As Object = ctx.Transformer.GetTransformer(e).TransformEntity(e)
            If transformEntity Is Nothing Then Return Nothing


            Return transformEntity
        End Function
    End Class
End Namespace


