

Namespace Transform
       Public Class EntityTransformerProvider
        Private Shared ReadOnly PadLock As New Object
        Private Shared _allTransformers As New Dictionary(Of Type, ITransformerFactory)

        Private Shared ReadOnly Property AllTransformers As Dictionary(Of Type, ITransformerFactory)
            Get
                Return _allTransformers
            End Get
        End Property

        Public Shared Sub AddFactory(Of TAction As IAmAnAction)(factory As ITransformerFactory)
            If _allTransformers.ContainsKey(GetType(TAction)) Then
                Throw New TransformerFactoryForActionAllreadyExists(GetType(TAction), _allTransformers(GetType(TAction)).GetType, factory)
            End If
            _allTransformers.Add(GetType(TAction), factory)
        End Sub

        Private Shared ReadOnly DefaultFactory As New DefaultEntiyTransformerFactory

        Public Shared Function GetFactory(ByVal action As IAmAnAction) As ITransformerFactory
            Dim t = action.GetType
            While t IsNot Nothing
                If AllTransformers.ContainsKey(t) Then
                    Return AllTransformers(t)
                End If
                t = t.BaseType
            End While

            Return DefaultFactory

        End Function


        Public Class DefaultEntiyTransformerFactory
            Implements ITransformerFactory
            
            ReadOnly _Trans As New DoNothingWithTheEntityTransformer

            Public Property RunAsParallel As Boolean = true Implements ITransformerFactory.RunAsParallel

            Public Property ObjectComparer As Comparison(Of Object) Implements ITransformerFactory.ObjectComparer
                Get
                    Throw New NotImplementedException()
                End Get
                Set(value As Comparison(Of Object))
                    Throw New NotImplementedException()
                End Set
            End Property

            Public Function GetTransformer(ent As Object) As ITransformEntityToDto Implements ITransformerFactory.GetTransformer
                Return _Trans
            End Function

            Public Class DoNothingWithTheEntityTransformer
                Implements ITransformEntityToDto

                Public Function TransformEntity(ByVal ent As Object) As Object Implements ITransformEntityToDto.TransformEntity
                    Return ent
                End Function

                Public Property Action As IAmAnAction Implements ITransformEntityToDto.Action
            End Class



        End Class

    End Class
End Namespace
