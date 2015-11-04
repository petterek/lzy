Imports System.Data

Public Class ParameterInfo
    Private _Name As String
    Private _DBType As DbType
    Private _Size As Integer
    Private _allowNull As Boolean
    Private _Value As Object

    Public Property Name() As String
        Get
            Return _Name
        End Get
        Set(ByVal value As String)
            _Name = value
        End Set
    End Property

    Public Property DbType() As DbType
        Get
            Return _DBType
        End Get
        Set(ByVal value As DbType)
            _DBType = value
        End Set
    End Property

    Public Property Size() As Integer
        Get
            Return _Size
        End Get
        Set(ByVal value As Integer)
            _Size = value
        End Set
    End Property

    Public Property AllowNull() As Boolean
        Get
            Return _allowNull
        End Get
        Set(ByVal value As Boolean)
            _allowNull = value
        End Set
    End Property

    Public Property Value() As Object
        Get
            Return _Value
        End Get
        Set(ByVal value As Object)
            _Value = value
        End Set
    End Property
    
End Class