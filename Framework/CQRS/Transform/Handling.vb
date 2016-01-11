
Imports LazyFramework.CQRS.Dto
Imports System.Linq

Namespace CQRS.Transform
    Public Class Handling

        Public Shared Function TransformResult(ByVal action As IAmAnAction, ByVal result As Object, Optional ByVal transformer As ITransformEntityToDto = Nothing) As Object
            Dim transformerFactory As ITransformerFactory = EntityTransformerProvider.GetFactory(action)

            'Hmmmm skal vi ha logikk her som sjekker om det er noe factory, og hvis det ikke er det bare returnere det den fikk inn. 
            'Egentlig er det jo bare commands som trenger dette. Queries bør jo gjøre dette selv.. Kanskje. 

            If TypeOf result Is IList Then
                Dim ret As New Concurrent.ConcurrentQueue(Of Object)
                Dim res As Object


                If Runtime.Context.Current.ChickenMode Then
                    For Each e In CType(result, IList)
                        res = TransformAndAddAction(action, If(transformer Is Nothing, transformerFactory.GetTransformer(action, e), transformer), e)
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

                    CType(result, IList).
                        Cast(Of Object).
                        AsParallel.ForAll(Sub(o As Object)
                                              Try
                                                  Using New Runtime.SpawnThreadContext(user, s, cm)
                                                      Dim temp = TransformAndAddAction(action, If(transformer Is Nothing, transformerFactory.GetTransformer(action, o), transformer), o)
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

                    Dim retList = ret.ToList
                    If transformerFactory.SortingFunc IsNot Nothing Then
                        retList.Sort(transformerFactory.SortingFunc)
                    Else
                        If transformer IsNot Nothing AndAlso TypeOf (transformer) Is ISortingFunction AndAlso CType(transformer, ISortingFunction).SortingFunc IsNot Nothing Then
                            retList.Sort(CType(transformer, ISortingFunction).SortingFunc)
                        End If
                    End If



                    Return retList
                End If
            Else
                Return TransformAndAddAction(action, If(transformer Is Nothing, transformerFactory.GetTransformer(action, result), transformer), result)
            End If
        End Function

        Public Shared Function TransformAndAddAction(ByVal action As IAmAnAction, ByVal transformer As ITransformEntityToDto, e As Object) As Object
            Dim securityContext As Object
            If transformer Is Nothing Then Return Nothing
            If TypeOf (e) Is IProvideSecurityContext Then
                securityContext = DirectCast(e, IProvideSecurityContext).Context
            Else
                securityContext = e
            End If

            If Not ActionSecurity.Current.EntityIsAvailableForUser(action.User, action, securityContext) Then Return Nothing

            Dim transformEntity As Object = transformer.TransformEntity(e)
            If transformEntity Is Nothing Then Return Nothing

            If TypeOf (transformEntity) Is ISupportActionList Then
                CType(transformEntity, ISupportActionList).Actions.AddRange(ActionSecurity.Current.GetActionList(action.User, action, e))
            End If
            If TypeOf transformEntity Is ActionContext.ActionContext Then

            End If
            Return transformEntity
        End Function
        Public Shared Function TransformAndAddAction(ByVal action As IAmAnAction, ByVal transformer As ITransformEntityToDto, e As IEnumerable) As IEnumerable (Of Object)
            Dim ret = New List(Of Object)
            For Each res In e
                Dim transRes = TransformAndAddAction(action, transformer, res)
                If transRes IsNot Nothing Then
                    ret.Add(transRes)
                End If
            Next
            Return ret
        End Function
    End Class
End Namespace


