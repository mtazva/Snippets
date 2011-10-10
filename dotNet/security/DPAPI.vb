Imports System
Imports System.IO
Imports System.Text
Imports System.Security.Cryptography
Imports DPAPI = System.Security.Cryptography.ProtectedData

Namespace Encryption

    ''' <summary>
    ''' Encrypt and Decrypt string values using the Windows Data Protection API (DPAPI)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DataProtector
        Implements IEncryptor

#Region " Constants and Enumerations "

        ''' <summary>
        ''' Use Entropy value in encryption to reduce sharing of encrypted data to only applications that use the same entropy value
        ''' </summary>
        ''' <remarks></remarks>
        Private entropy As Byte() = Nothing

        ''' <summary>
        ''' Default to machine-level encryption
        ''' </summary>
        ''' <remarks></remarks>
        Private scope As DataProtectionScope = DataProtectionScope.LocalMachine

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Create data protection object with entorpy name to isolate encryption to a single application
        ''' </summary>
        ''' <param name="entropyName"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal isolationScope As DataProtectionScope, ByVal entropyName As String)
            scope = isolationScope
            entropy = GetBytes(entropyName)
        End Sub

#End Region

#Region " Cryptography Methods "

        ''' <summary>
        ''' Encrypt string
        ''' </summary>
        ''' <param name="plainText">The string to be encypted</param>
        ''' <returns>The encrypted string</returns>
        Public Function Encrypt(ByVal plainText As String) As String Implements IEncryptor.Encrypt
            Return Base64Encode(DPAPI.Protect(GetBytes(plainText), entropy, scope))
        End Function

        ''' <summary>
        ''' Decrypt string
        ''' </summary>
        ''' <param name="encryptedText">The string to be decrypted</param>
        ''' <returns>The decrypted string</returns>
        Public Function Decrypt(ByVal encryptedText As String) As String Implements IEncryptor.Decrypt
            Return GetString(DPAPI.Unprotect(Base64Decode(encryptedText), entropy, scope))
        End Function

#End Region

#Region " Utility Methods "

        Private Shared Function GetBytes(ByVal value As String) As Byte()
            Return System.Text.UTF8Encoding.UTF8.GetBytes(value)
        End Function

        Private Shared Function GetString(ByVal bytes As Byte()) As String
            Return System.Text.UTF8Encoding.UTF8.GetString(bytes)
        End Function

        Private Shared Function Base64Encode(ByVal bytes As Byte()) As String
            Return Convert.ToBase64String(bytes)
        End Function

        Private Shared Function Base64Decode(ByVal value As String) As Byte()
            Return Convert.FromBase64String(value)
        End Function

#End Region

    End Class

    Public Interface IEncryptor

        Function Encrypt(ByVal plainText As String) As String

        Function Decrypt(ByVal plainText As String) As String

    End Interface

End Namespace
