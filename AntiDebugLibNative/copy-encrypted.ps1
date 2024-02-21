$key = <#dll_crypt_magic#>"AntiDebug.NET"<#/dll_crypt_magic#>

# http://kenwardtown.com/2016/02/28/the-xor-cipher-in-powershell/
function encode($plaintext)
{
    $cyphertext = new-object byte[] $plaintext.Length
    $keyposition = 0
    $KeyArray = $key.ToCharArray()
    $index = 0
    $plaintext | foreach-object -process {
        $cyphertext[$index] = [byte]($_ -bxor $KeyArray[$keyposition])
        $keyposition += 1
        if ($keyposition -eq $key.Length) {$keyposition = 0}
        $index += 1
    }
    return $cyphertext
}

write-output "copy $($args[0]) to $($args[1]) in encrypted form with key $($key)"

$src = [System.IO.Path]::GetFullPath($args[0])
$dst = [System.IO.Path]::GetFullPath($args[1])
$inBytes = [System.IO.File]::ReadAllBytes($src)
$encBytes = encode($inBytes)

[System.IO.File]::WriteAllBytes($dst, $encBytes)
