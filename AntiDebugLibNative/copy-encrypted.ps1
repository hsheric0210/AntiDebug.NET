$key = <#dll_crypt_magic#>"https://github.com/hsheric0210/AntiDebug.NET"<#/dll_crypt_magic#>

# http://kenwardtown.com/2016/02/28/the-xor-cipher-in-powershell/
function encode($plaintext)
{
    $cyphertext = new-object byte[] $plaintext.Length
    $keyposition = 0
    $KeyArray = $key.ToCharArray()
    $plaintext | foreach-object -process {
        $cyphertext += [byte]($_ -bxor $KeyArray[$keyposition])
        $keyposition += 1
        if ($keyposition -eq $key.Length) {$keyposition = 0}
    }
    return $cyphertext
}

write-output "copy $($args[0]) to $($args[1]) in encrypted form with key $($key)"

$src = [System.IO.Path]::GetFullPath($args[0])
$dst = [System.IO.Path]::GetFullPath($args[1])
$inBytes = [System.IO.File]::ReadAllBytes($src)
$encBytes = encode($inBytes)

[System.IO.File]::WriteAllBytes($dst, $encBytes)
