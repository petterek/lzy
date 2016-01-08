Imports System.Reflection

Namespace Utils.Json
    Public Class SetterCache

        Private Shared _setterlist As New Dictionary(Of String, NameInfo)

        Public Shared Function GetInfo(mInfo As MemberInfo) As NameInfo
            Dim key = mInfo.DeclaringType.FullName & "-" & mInfo.Name

            If Not _setterlist.ContainsKey(key) Then
                _setterlist(key) = New NameInfo(mInfo.DeclaringType.Name,mInfo.Name, Reflection.CreateSetter(mInfo))
            End If
            Return _setterlist(key)

        End Function

        Public Function SetInfo() As NameInfo

        End Function


        Public Class NameInfo
            Private ReadOnly _typename As String
            Private ReadOnly _name As String
            Private ReadOnly _setter As Action(Of Object, Object)


            Public Sub New(typename As String, name As String, setter As Action(Of Object, Object))
                _typename = typename
                _name = name
                _setter = setter
            End Sub

            Public ReadOnly Property Setter As Action(Of Object, Object)
                Get
                    Return _setter
                End Get
            End Property

            Public ReadOnly Property Name As String
                Get
                    Return _name
                End Get
            End Property

            Public ReadOnly Property Typename As String
                Get
                    Return _typename
                End Get
            End Property
        End Class
    End Class



End Namespace