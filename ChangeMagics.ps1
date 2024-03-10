function Rename-CommentMarker {
    param (
        [Parameter(Mandatory)]
        [string]
        $Data,

        [Parameter(Mandatory)]
        [string]
        $Marker,
        
        [Parameter(Mandatory)]
        [string]
        $Value,
        
        [Parameter(Mandatory)]
        [int]
        $TrimLength
    )

    $begin = $Data.IndexOf("/*<$Marker>*/") + "/*<$Marker>*/".Length + $TrimLength
    $end = $Data.IndexOf("/*</$Marker>*/") - $TrimLength
    $Data = $Data.Substring(0, $begin) + $Value + $Data.Substring($end)

    return $Data
}

function Rename-PsCommentMarker {
    param (
        [Parameter(Mandatory)]
        [string]
        $Data,

        [Parameter(Mandatory)]
        [string]
        $Marker,
        
        [Parameter(Mandatory)]
        [string]
        $Value,
        
        [Parameter(Mandatory)]
        [int]
        $TrimLength
    )

    $begin = $Data.IndexOf("<#$Marker#>") + "<#$Marker#>".Length + $TrimLength
    $end = $Data.IndexOf("<#/$Marker#>") - $TrimLength
    $Data = $Data.Substring(0, $begin) + $Value + $Data.Substring($end)

    return $Data
}

$entryPrefixChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_".ToCharArray()
$entryChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_".ToCharArray()
$xorKeyChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!#$%&()*+,-./:;<=>?@[]^_{|}~".ToCharArray()

$prefix = [char]($entryPrefixChars | Get-Random)
$length = Get-Random -Minimum 16 -Maximum 32
$generated = -join($entryChars | Get-Random -Count $length | ForEach-Object { [char]$_ })
$entryName = $prefix + $generated

$length = Get-Random -Minimum 16 -Maximum 32
$xorEncKey = -join($xorKeyChars | Get-Random -Count $length | ForEach-Object { [char]$_ })

Write-Output "Entry point name is $entryName"
Write-Output "DLL XOR encryption key is $xorEncKey"

$cs = [System.IO.Path]::Combine((Get-Location).Path, "AntiDebugLib", "Native", "AntiDebugLibNative.cs")
$cpp = [System.IO.Path]::Combine((Get-Location).Path, "AntiDebugLibNative", "dllmain.cpp")
$ps = [System.IO.Path]::Combine((Get-Location).Path, "AntiDebugLibNative", "copy-encrypted.ps1")

$cs_cnt = [System.IO.File]::ReadAllText($cs)
$cpp_cnt = [System.IO.File]::ReadAllText($cpp)
$ps_cnt = [System.IO.File]::ReadAllText($ps)

$cs_cnt = Rename-CommentMarker -Data $cs_cnt -Marker "cs_entrypoint" -Value $entryName -TrimLength 1
$cpp_cnt = Rename-CommentMarker -Data $cpp_cnt -Marker "c_entrypoint" -Value $entryName -TrimLength 0

$cs_cnt = Rename-CommentMarker -Data $cs_cnt -Marker "dll_crypt_magic" -Value $xorEncKey -TrimLength 1
$ps_cnt = Rename-PsCommentMarker -Data $ps_cnt -Marker "dll_crypt_magic" -Value $xorEncKey -TrimLength 1

[System.IO.File]::WriteAllText($cs, $cs_cnt)
[System.IO.File]::WriteAllText($cpp, $cpp_cnt)
[System.IO.File]::WriteAllText($ps, $ps_cnt)
