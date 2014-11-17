﻿Imports LazyFramework.CQRS.Command

Namespace CQRS.Logging
    Public Class EventInfo
        Public EventId As Guid
        Public SourceCommand As Guid
        Public CommandData As IAmACommand
        Public Timestamp As Long
    End Class
End NameSpace
