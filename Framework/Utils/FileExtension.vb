Imports System.Runtime.InteropServices
Imports System.IO
Imports Microsoft.Win32.SafeHandles

Namespace Utils
    Public Class FileExtension

        Public Interface IDoSerializiation
            Function Serialize(data As Object) As String
            Function DeSerialize(Of T)(content As String) As T
        End Interface

        Public Shared Sub SetData(fileName As String, data As Object, serializer As IDoSerializiation)
            Dim serialized As String
            serialized = serializer.Serialize(data)


            Dim handler = SafeNativeMethods.GetHandle(Path(data.GetType, fileName), SafeNativeMethods.AccessType.Write)
            Using f As New FileStream(handler, FileAccess.Write)

                With New StreamWriter(f)
                    .WriteLine(serialized)
                    .Flush()
                End With
            End Using
        End Sub

        Private Shared Function Path(ByVal data As Type, ByVal fileName As String) As String
            Return fileName & ":" & data.Name.ToLower.Replace("."c, "")
        End Function


        Public Shared Function LoadData(Of T As {New, Class})(fileName As String, serializer As IDoSerializiation) As T
            Dim handler = SafeNativeMethods.GetHandle(Path(GetType(T), fileName), SafeNativeMethods.AccessType.Read)
            Dim contents As String = ""

            If Not handler.IsInvalid Then
                Using f As New FileStream(handler, FileAccess.Read)
                    With New StreamReader(f)
                        contents = .ReadToEnd
                    End With
                End Using

            End If

            If String.IsNullOrEmpty(contents) Or contents.Length = 0 Then
                Return New T
            Else
                Return serializer.DeSerialize(Of T)(contents)

            End If

        End Function


        ' ReSharper disable once ClassNeverInstantiated.Local
        Private NotInheritable Class SafeNativeMethods

            Private Sub New()
            End Sub

            Private Const FileAttributeNormal As Short = &H80
            Private Const InvalidHandleValue As Short = -1

            Private Const CreateNew As UInteger = 1UI
            Private Const CreateAlways As UInteger = 2UI
            Private Const OpenExisting As UInteger = 3UI

            ' Use interop to call the CreateFile function. 
            ' For more information about CreateFile, 
            ' see the unmanaged MSDN reference library.
            <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Unicode, EntryPoint:="CreateFileW")> _
            Private Shared Function CreateFile(ByVal lpFileName As String, ByVal dwDesiredAccess As UInt32, ByVal dwShareMode As UInt32, ByVal lpSecurityAttributes As IntPtr, ByVal dwCreationDisposition As UInt32, ByVal dwFlagsAndAttributes As UInt32, ByVal hTemplateFile As IntPtr) As SafeFileHandle
            End Function

            Public Shared Function GetHandle(ByVal path As String, openType As AccessType) As SafeFileHandle
                Dim handleValue As SafeFileHandle = Nothing
                If path Is Nothing OrElse path.Length = 0 Then
                    Throw New ArgumentNullException(NameOf(path))
                End If

                ' Try to open the file.
                If openType = AccessType.Read Then
                    handleValue = CreateFile(path, openType, 0, IntPtr.Zero, OpenExisting, 0, IntPtr.Zero)
                Else
                    handleValue = CreateFile(path, openType, 0, IntPtr.Zero, CreateAlways, 0, IntPtr.Zero)
                End If
                
                ' If the handle is invalid, 
                ' get the last Win32 error  
                ' and throw a Win32Exception. 
                If handleValue.IsInvalid Then
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error())
                End If

                Return handleValue
            End Function


            Friend Enum AccessType As UInteger
                Read = &H80000000UI
                Write = &H40000000UI
            End Enum
        End Class

    End Class




End Namespace
