function Rename-Marker {
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

$entryName = Read-Host -Prompt "New Entry-point procedure Name"
$gpaName = Read-Host -Prompt "New MyGetProcAddress procedure Name"
$gmhName = Read-Host -Prompt "New MyGetModuleHandle procedure Name"
$pebName = Read-Host -Prompt "New MyGetPeb procedure Name"

$beginMarkerFormat = '<%s>'
$endMarkerFormat = '</%s>'

$cs = [System.IO.Path]::Combine((Get-Location).Path, "AntiDebugLib", "NativeCalls.cs")
$cpp = [System.IO.Path]::Combine((Get-Location).Path, "AntiDebugLibNative", "dllmain.cpp")
$cs_cnt = [System.IO.File]::ReadAllText($cs)
$cpp_cnt = [System.IO.File]::ReadAllText($cpp)

$cs_cnt = Rename-Marker -Data $cs_cnt -Marker "cs_entrypoint" -Value $entryName -TrimLength 1
$cpp_cnt = Rename-Marker -Data $cpp_cnt -Marker "c_entrypoint" -Value $entryName -TrimLength 0

$cs_cnt = Rename-Marker -Data $cs_cnt -Marker "cs_getprocaddress" -Value $gpaName -TrimLength 1
$cpp_cnt = Rename-Marker -Data $cpp_cnt -Marker "c_getprocaddress" -Value $gpaName -TrimLength 0

$cs_cnt = Rename-Marker -Data $cs_cnt -Marker "cs_getmodulehandle" -Value $gmhName -TrimLength 1
$cpp_cnt = Rename-Marker -Data $cpp_cnt -Marker "c_getmodulehandle" -Value $gmhName -TrimLength 0

$cs_cnt = Rename-Marker -Data $cs_cnt -Marker "cs_getpeb" -Value $pebName -TrimLength 1
$cpp_cnt = Rename-Marker -Data $cpp_cnt -Marker "c_getpeb" -Value $pebName -TrimLength 0

[System.IO.File]::WriteAllText($cs, $cs_cnt)
[System.IO.File]::WriteAllText($cpp, $cpp_cnt)
