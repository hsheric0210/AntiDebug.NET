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
    $end = $Data.IndexOf("/*<#/$Marker#>") - $TrimLength
    $Data = $Data.Substring(0, $begin) + $Value + $Data.Substring($end)

    return $Data
}

$entryName = Read-Host -Prompt "New Entry-point procedure Name"
$pebName = Read-Host -Prompt "New MyGetPeb procedure Name"
$xorEncKey = Read-Host -Prompt "New native dll encryption key"

$cs = [System.IO.Path]::Combine((Get-Location).Path, "AntiDebugLib", "Native", "AntiDebugLibNative.cs")
$cpp = [System.IO.Path]::Combine((Get-Location).Path, "AntiDebugLibNative", "dllmain.cpp")
$ps = [System.IO.Path]::Combine((Get-Location).Path, "AntiDebugLibNative", "copy-encrypted.ps1")

$cs_cnt = [System.IO.File]::ReadAllText($cs)
$cpp_cnt = [System.IO.File]::ReadAllText($cpp)
$ps_cnt = [System.IO.File]::ReadAllText($ps)

$cs_cnt = Rename-CommentMarker -Data $cs_cnt -Marker "cs_entrypoint" -Value $entryName -TrimLength 1
$cpp_cnt = Rename-CommentMarker -Data $cpp_cnt -Marker "c_entrypoint" -Value $entryName -TrimLength 0

$cs_cnt = Rename-CommentMarker -Data $cs_cnt -Marker "cs_getpeb" -Value $pebName -TrimLength 1
$cpp_cnt = Rename-CommentMarker -Data $cpp_cnt -Marker "c_getpeb" -Value $pebName -TrimLength 0

$cs_cnt = Rename-CommentMarker -Data $cs_cnt -Marker "dll_crypt_magic" -Value $pebName -TrimLength 1
$ps_cnt = Rename-PsCommentMarker -Data $ps_cnt -Marker "dll_crypt_magic" -Value $pebName -TrimLength 1

[System.IO.File]::WriteAllText($cs, $cs_cnt)
[System.IO.File]::WriteAllText($cpp, $cpp_cnt)
[System.IO.File]::WriteAllText($ps, $ps_cnt)
