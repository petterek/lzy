Imports System.Reflection
Imports System.IO

Public Class TypeValidation

    Public Interface ICheckTypeProvider
        Function GetTypes() As List(Of String)
        ReadOnly Property Name As String
        Property Result As Boolean
        ReadOnly Property MissingClasses As List(Of String)

    End Interface

    Public MustInherit Class CheckTypeProviderBaseClass
        Implements ICheckTypeProvider


        Public MustOverride Function GetTypes() As List(Of String) Implements ICheckTypeProvider.GetTypes
        Public MustOverride ReadOnly Property Name As String Implements ICheckTypeProvider.Name
        Private _result As Boolean
        Public Property Result As Boolean Implements ICheckTypeProvider.Result
            Get
                Return _result
            End Get
            Set(ByVal value As Boolean)
                _result = value
            End Set
        End Property

        Private ReadOnly _missingClasses As New List(Of String)
        Public ReadOnly Property MissingClasses As List(Of String) Implements ICheckTypeProvider.MissingClasses
            Get
                Return _missingClasses
            End Get

        End Property
    End Class

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="class"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ValidateClass(ByVal [class] As String) As Boolean
        If Type.GetType([class]) Is Nothing Then
            Return False
        Else
            Return True
        End If
    End Function

    Private Shared ReadOnly Locker As New Object

    <Obsolete>Public Shared Function FindAllClassesOfTypeInApplication(ByVal t As Type, Optional ByVal skipSystem As Boolean = True, Optional ByVal forceLoad As Boolean = True) As List(Of Type)
        Return Reflection.FindAllClassesOfTypeInApplication(t,skipSystem,forceLoad)
    End Function


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ValidateApplicationWeakReferances() As List(Of ICheckTypeProvider)
        Dim validator As ICheckTypeProvider
        Dim ret As New List(Of ICheckTypeProvider)

        For Each t In Reflection.FindAllClassesOfTypeInApplication(GetType(ICheckTypeProvider))
            Try
                validator = CType(Activator.CreateInstance(t), ICheckTypeProvider)
                ret.Add(validator)
                validator.Result = True
                For Each s In validator.GetTypes
                    If Not ValidateClass(s) Then
                        validator.Result = False
                        validator.MissingClasses.Add(s)
                    End If
                Next
            Catch ex As Exception
                Throw New MissingMethodException("Klarte ikke å opprette validator av type:" & t.FullName & "Error:" & ex.Message)
            End Try

        Next
        Return ret
    End Function

End Class
