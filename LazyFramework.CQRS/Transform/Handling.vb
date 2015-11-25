
Imports LazyFramework.CQRS.Dto
Imports System.Linq
Imports LazyFramework.CQRS.Security

Namespace Transform
    Public Class Handling

        Public Shared Function TransformResult(profile As ExecutionProfile.IExecutionProfile, ByVal action As IAmAnAction, ByVal result As Object, Optional ByVal transformer As ITransformEntityToDto = Nothing) As Object
            Dim transformerFactory As ITransformerFactory = EntityTransformerProvider.GetFactory(action)

            'Hmmmm skal vi ha logikk her som sjekker om det er noe factory, og hvis det ikke er det bare returnere det den fikk inn. 
            'Egentlig er det jo bare commands som trenger dette. Queries bør jo gjøre dette selv.. Kanskje. 

            If TypeOf result Is IEnumerable Then
                Dim ret As New Concurrent.ConcurrentQueue(Of Object)
                Dim res As Object


                If Runtime.Context.Current.ChickenMode Then
                    For Each e In CType(result, IList)
                        res = TransformAndAddAction(profile, action, If(transformer Is Nothing, transformerFactory.GetTransformer(action, e), transformer), e)
                        If res IsNot Nothing Then
                            ret.Enqueue(res)
                        End If
                    Next
                    Return ret.ToList
                Else
                    Dim user = Runtime.Context.Current.CurrentUser  'Have to copy this from outside of the loop
                    Dim s = Runtime.Context.Current.Storage
                    Dim cm = Runtime.Context.Current.ChickenMode
                    Dim Errors As New Concurrent.ConcurrentBag(Of Exception)

                    CType(result, IEnumerable).
                        Cast(Of Object).
                        AsParallel.ForAll(Sub(o As Object)
                                              Try
                                                  Using New Runtime.SpawnThreadContext(user, s, cm)
                                                      Dim temp = TransformAndAddAction(profile,action, If(transformer Is Nothing, transformerFactory.GetTransformer(action, o), transformer), o)
                                                      If temp IsNot Nothing Then
                                                          ret.Enqueue(temp)
                                                      End If
                                                  End Using
                                              Catch ex As Exception
                                                  Errors.Add(ex)
                                              End Try
                                          End Sub)

                    If Errors.Count > 0 Then
                        Throw Errors(0)
                    End If
                    
                    Return ret.ToList
                End If
            Else
                Return TransformAndAddAction(profile, action, If(transformer Is Nothing, transformerFactory.GetTransformer(action, result), transformer), result)
            End If
        End Function

        Public Shared Function TransformAndAddAction(profile As ExecutionProfile.IExecutionProfile, ByVal action As IAmAnAction, ByVal transformer As ITransformEntityToDto, e As Object) As Object
            Dim securityContext As Object
            If transformer Is Nothing Then Return Nothing

            If TypeOf (e) Is IProvideSecurityContext Then
                securityContext = DirectCast(e, IProvideSecurityContext).Context
            Else
                securityContext = e
            End If

            If Not ActionSecurity.Current.EntityIsAvailableForUser(profile, action, securityContext) Then Return Nothing

            Dim transformEntity As Object = transformer.TransformEntity(e)
            If transformEntity Is Nothing Then Return Nothing

            If TypeOf (transformEntity) Is ISupportActionList Then
                CType(transformEntity, ISupportActionList).Actions.AddRange(ActionSecurity.Current.GetActionList(profile, action, e))
            End If
            If TypeOf transformEntity Is ActionContext.ActionContext Then

            End If
            Return transformEntity
        End Function
    End Class
End Namespace


