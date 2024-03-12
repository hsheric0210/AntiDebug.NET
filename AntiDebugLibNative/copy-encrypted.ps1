$key = <#dll_crypt_magic#>"Zt/XfLsNvmFqV%R_k!C*ox~I"<#/dll_crypt_magic#>

# https://www.powershellgallery.com/packages/DRTools/4.0.2.3/Content/Functions%5CInvoke-AESEncryption.ps1
function aes-encrypt($plaintext)
{
    Begin {
        $sha = [System.Security.Cryptography.SHA256]::Create()
        $aes = [System.Security.Cryptography.Aes]::Create()
        $aes.Mode = [System.Security.Cryptography.CipherMode]::CBC
        $aes.Padding = [System.Security.Cryptography.PaddingMode]::ISO10126
        $aes.KeySize = 256
    }

    Process {
        $aes.Key = $sha.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($Key))

        $crypt = $aes.CreateEncryptor()
        $cipherText = $crypt.TransformFinalBlock($plaintext, 0, $plaintext.Length)
        $cipherText = $aes.IV + $cipherText

        $aes.Dispose()
        $sha.Dispose()

        return $cipherText
    }

    End {
        $aes.Dispose()
        $sha.Dispose()
    }
}

write-output "copy $($args[0]) to $($args[1]) in encrypted form with key $($key)"

$src = [System.IO.Path]::GetFullPath($args[0])
$dst = [System.IO.Path]::GetFullPath($args[1])
$inBytes = [System.IO.File]::ReadAllBytes($src)
$encBytes = aes-encrypt($inBytes)

[System.IO.File]::WriteAllBytes($dst, $encBytes)
