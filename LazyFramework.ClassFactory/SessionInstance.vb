
Partial Class ClassFactory


    Public Class SessionInstance
        Implements IDisposable

        Public Sub Start()
            For Each ti In GetAll()
                Dim o As ISessionAware
                If GetType(ISessionAware).IsAssignableFrom(ti.CurrentType) Then
                    o = CType(Activator.CreateInstance(ti.CurrentType), ISessionAware)
                    o.SessionStart()
                    'Gjør dette dobbelt.. for å være helt sikkert. 
                    If TypeOf (ti.CurrentInstance) Is IDisposable Then
                        CType(ti.CurrentInstance, IDisposable).Dispose()
                    End If
                    ti.CurrentInstance = Nothing
                End If
            Next
        End Sub

        Public Sub Complete()
            For Each ti In GetAll()
                Dim o As ISessionAware
                If GetType(ISessionAware).IsAssignableFrom(ti.CurrentType) Then
                    o = CType(Activator.CreateInstance(ti.CurrentType), ISessionAware)
                    o.SessionEnd()
                    If TypeOf (ti.CurrentInstance) Is IDisposable Then
                        CType(ti.CurrentInstance, IDisposable).Dispose()
                    End If
                End If
                ti.CurrentInstance = Nothing
            Next
            Dispose()
        End Sub

        Public Sub New()
            Parent = ClassFactory.Session
            ClassFactory.Session = Me
            _store = New Dictionary(Of Type, ITypeInfo)
        End Sub

        Public Parent As SessionInstance
        Private _store As Dictionary(Of Type, ITypeInfo)


        Public Function ContainsKey(t As Type) As Boolean
            If _store.ContainsKey(t) Then Return True
            If Parent IsNot Nothing Then Return Parent.ContainsKey(t)
            Return False
        End Function

        Public Function ContainsKey(Of TKey)(name As String) As Boolean
            If _store.ContainsKey(GetType(Dictionary(Of String, TKey))) Then
                If CType(_store(GetType(Dictionary(Of String, TKey))).CurrentInstance, Dictionary(Of String, TKey)).ContainsKey(name) Then
                    Return True
                End If
            End If
            If Parent IsNot Nothing Then Return Parent.ContainsKey(Of TKey)(name)
            Return False
        End Function


        Public Function GetInstance(key As Type) As ITypeInfo
            If _store.ContainsKey(key) Then Return _store(key)
            If Parent IsNot Nothing Then Return Parent.GetInstance(key)
            Throw New NotConfiguredException(key.ToString)
        End Function

        Public Function GetAll() As IEnumerable(Of ITypeInfo)
            Return _store.Values
        End Function

        Public Sub SetInstance(key As Type, typeInfo As ITypeInfo)
            _store(key) = typeInfo
        End Sub

        Public Function Remove(ByVal type As Type) As Boolean
            _store.Remove(type)
        End Function

#Region "IDisposable Support"

        Private _DisposedValue As Boolean ' To detect redundant calls
        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not _DisposedValue Then
                _DisposedValue = True
                If disposing Then
                    Complete()
                    ClassFactory.Session = Parent

                    _store = Nothing
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            _DisposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

#End Region
    End Class

End Class
