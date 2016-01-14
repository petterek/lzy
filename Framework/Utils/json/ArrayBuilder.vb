Namespace Utils.Json
    Friend Class ArrayBuilder
        Inherits Builder

        Public Sub New(t As Type)
            MyBase.New(t)
        End Sub




        Public Overrides Function Parse(nextChar As IReader) As Object

            TokenAcceptors.BufferLegalCharacters(nextChar,"nul" )
            Dim buffer = nextChar.Buffer
            If buffer="null" then return nothing

            TokenAcceptors.EatUntil(TokenAcceptors.ListStart, nextChar) 
            
            Dim strategy As IParseStrategy
            If type.IsArray Then
                strategy = New ArrayParserStrategy(type)
            Else
                strategy = New ListParseStrategy(type)
            End If
            
            TokenAcceptors.WhiteSpace(nextChar)
            
            Do
                If strategy.InnerType.IsValueType Or strategy.InnerType = GetType(String) Then
                    TokenAcceptors.WhiteSpace(nextChar)
                    If nextChar.Peek <> TokenAcceptors.ListEnd Then
                        strategy.ItemList.Add(TokenAcceptors.TypeParserMapper(strategy.InnerType).Parse(nextChar))
                    End If
                Else
                    Dim v As Object = Reader.StringToObject(nextChar, strategy.InnerType)
                    If v IsNot Nothing Then
                        strategy.ItemList.Add(v)
                    End If
                End If
            Loop While TokenAcceptors.CanFindValueSeparator(nextChar)

            TokenAcceptors.EatUntil(TokenAcceptors.ListEnd, nextChar)

            Return strategy.Result
        End Function


        Private Interface IParseStrategy
            ReadOnly Property ItemList As IList
            ReadOnly Property InnerType As Type
            ReadOnly Property Result As Object
        End Interface

        Private Class ArrayParserStrategy
            Implements IParseStrategy

            Private _innerType As Type
            Private _t As Type
            Public Sub New(t As Type)
                _t = t
                _innerType = t.GetElementType
                Dim tempType = GetType(List(Of ))
                _itemList = CType(Activator.CreateInstance(tempType.MakeGenericType(InnerType)), IList)
            End Sub


            Private _itemList As IList
            Public ReadOnly Property ItemList As IList Implements IParseStrategy.ItemList
                Get
                    Return _itemList
                End Get
            End Property

            Public ReadOnly Property Result As Object Implements IParseStrategy.Result
                Get
                    Dim newA = Activator.CreateInstance(_t, _itemList.Count)
                    _itemList.CopyTo(CType(newA, Array), 0)
                    Return newA
                End Get
            End Property

            Public ReadOnly Property InnerType As Type Implements IParseStrategy.InnerType
                Get
                    Return _innerType
                End Get
            End Property
        End Class

        Private Class ListParseStrategy
            Implements IParseStrategy

            Private _ItemList As IList
            Private _innertype As Type

            Public Sub New(t As Type)
                _ItemList = CType(Activator.CreateInstance(t), IList)
                If t.IsGenericType Then
                    _innertype = t.GetGenericArguments(0)
                Else
                    Throw New NonGenericListIsNotSupportedException
                End If
            End Sub

            Public ReadOnly Property InnerType As Type Implements IParseStrategy.InnerType
                Get
                    Return _innertype
                End Get
            End Property

            Public ReadOnly Property ItemList As IList Implements IParseStrategy.ItemList
                Get
                    Return _ItemList
                End Get
            End Property

            Public ReadOnly Property Result As Object Implements IParseStrategy.Result
                Get
                    Return _ItemList
                End Get
            End Property
        End Class



    End Class
End Namespace
