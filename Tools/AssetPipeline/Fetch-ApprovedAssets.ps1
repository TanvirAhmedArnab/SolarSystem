[CmdletBinding()]
param([string]$ProjectRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path)

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'
$sourceRoot = Join-Path $ProjectRoot 'SourceAssets\ThirdParty'
$downloadRoot = Join-Path $ProjectRoot 'SourceAssets\_Downloads'
$manifestPath = Join-Path $ProjectRoot 'SourceAssets\asset-download-manifest.csv'

$assets = @(
@{Id='TEX-SSS-001';Url='https://www.solarsystemscope.com/textures/download/2k_mercury.jpg';Relative='Textures\SolarSystemScope\2k_mercury.jpg'},
@{Id='TEX-SSS-002';Url='https://www.solarsystemscope.com/textures/download/2k_venus_surface.jpg';Relative='Textures\SolarSystemScope\2k_venus_surface.jpg'},
@{Id='TEX-SSS-003';Url='https://www.solarsystemscope.com/textures/download/2k_venus_atmosphere.jpg';Relative='Textures\SolarSystemScope\2k_venus_atmosphere.jpg'},
@{Id='TEX-SSS-004';Url='https://www.solarsystemscope.com/textures/download/2k_earth_daymap.jpg';Relative='Textures\SolarSystemScope\2k_earth_daymap.jpg'},
@{Id='TEX-SSS-005';Url='https://www.solarsystemscope.com/textures/download/2k_earth_nightmap.jpg';Relative='Textures\SolarSystemScope\2k_earth_nightmap.jpg'},
@{Id='TEX-SSS-006';Url='https://www.solarsystemscope.com/textures/download/2k_earth_clouds.jpg';Relative='Textures\SolarSystemScope\2k_earth_clouds.jpg'},
@{Id='TEX-SSS-007';Url='https://www.solarsystemscope.com/textures/download/2k_earth_normal_map.tif';Relative='Textures\SolarSystemScope\2k_earth_normal_map.tif'},
@{Id='TEX-SSS-008';Url='https://www.solarsystemscope.com/textures/download/2k_earth_specular_map.tif';Relative='Textures\SolarSystemScope\2k_earth_specular_map.tif'},
@{Id='TEX-SSS-009';Url='https://www.solarsystemscope.com/textures/download/2k_moon.jpg';Relative='Textures\SolarSystemScope\2k_moon.jpg'},
@{Id='TEX-SSS-010';Url='https://www.solarsystemscope.com/textures/download/2k_mars.jpg';Relative='Textures\SolarSystemScope\2k_mars.jpg'},
@{Id='TEX-SSS-011';Url='https://www.solarsystemscope.com/textures/download/2k_jupiter.jpg';Relative='Textures\SolarSystemScope\2k_jupiter.jpg'},
@{Id='TEX-SSS-012';Url='https://www.solarsystemscope.com/textures/download/2k_saturn.jpg';Relative='Textures\SolarSystemScope\2k_saturn.jpg'},
@{Id='TEX-SSS-013';Url='https://www.solarsystemscope.com/textures/download/2k_saturn_ring_alpha.png';Relative='Textures\SolarSystemScope\2k_saturn_ring_alpha.png'},
@{Id='TEX-SSS-014';Url='https://www.solarsystemscope.com/textures/download/2k_uranus.jpg';Relative='Textures\SolarSystemScope\2k_uranus.jpg'},
@{Id='TEX-SSS-015';Url='https://www.solarsystemscope.com/textures/download/2k_neptune.jpg';Relative='Textures\SolarSystemScope\2k_neptune.jpg'},
@{Id='TEX-SSS-016';Url='https://www.solarsystemscope.com/textures/download/2k_sun.jpg';Relative='Textures\SolarSystemScope\2k_sun.jpg'},
@{Id='TEX-SSS-017';Url='https://www.solarsystemscope.com/textures/download/2k_stars_milky_way.jpg';Relative='Textures\SolarSystemScope\2k_stars_milky_way.jpg'},
@{Id='TEX-USGS-001';Url='https://astrogeology.usgs.gov/ckan/dataset/b9102ce8-3ee4-4848-8558-3dab5f52091a/resource/0d994c1b-bf4e-4dfd-9f90-e730a438c3ff/download/browse.jpg';Relative='Textures\USGS\io_global_mosaic_browse.jpg'},
@{Id='TEX-USGS-002';Url='https://astrogeology.usgs.gov/ckan/dataset/4080036f-afc5-422e-abe9-1c0c8e4f98ea/resource/4db7ab3d-872d-4d26-9eb0-4b87dbc6a56b/download/europa_voyager_galileossi_global_mosaic_500m_512.jpg';Relative='Textures\USGS\europa_global_mosaic_browse.jpg'},
@{Id='TEX-USGS-003';Url='https://astrogeology.usgs.gov/ckan/dataset/57cad6e2-ed52-4b99-9d44-afbb9def6450/resource/29c3ba7f-f2d1-4678-80b7-734cfc7744ee/download/ganymede_voyager_galileossi_global_mosaic_512.jpg';Relative='Textures\USGS\ganymede_global_mosaic_browse.jpg'},
@{Id='TEX-USGS-004';Url='https://astrogeology.usgs.gov/ckan/dataset/842a3a75-af37-40ff-bf83-78ee5c76afb2/resource/281b14eb-c312-4cc7-b72b-02c686de39d5/download/browse.jpg';Relative='Textures\USGS\callisto_global_mosaic_browse.jpg'},
@{Id='TEX-USGS-005';Url='https://astrogeology.usgs.gov/ckan/dataset/f2008ed2-d851-4d70-b360-a38b76e7fd13/resource/8fbf22ae-b524-4083-b186-2141880b2595/download/browse.jpg';Relative='Textures\USGS\titan_near_global_mosaic_browse.jpg'},
@{Id='TEX-USGS-006';Url='https://astrogeology.usgs.gov/ckan/dataset/445b4c39-e87a-4e4d-88a8-e48d8e755c5c/resource/05bf1c30-5f51-4dd3-b738-42554965a9e2/download/triton_voyager2_clrmosaic_512.jpg';Relative='Textures\USGS\triton_global_color_mosaic_browse.jpg'},
@{Id='AUD-OGA-MUS-001';Url='https://opengameart.org/sites/default/files/outer_space_2.mp3';Referrer='https://opengameart.org/content/outer-space-loop';Relative='Audio\OpenGameArt\Wipics\outer_space.mp3'},
@{Id='AUD-OGA-SUN-001';Url='https://opengameart.org/sites/default/files/fire.wav';Referrer='https://opengameart.org/content/fireplace-sound-loop';Relative='Audio\OpenGameArt\PagDev\fire.wav'},
@{Id='AUD-OGA-EARTH-001';Url='https://opengameart.org/sites/default/files/Forest_Ambience.mp3';Referrer='https://opengameart.org/content/forest-ambience';Relative='Audio\OpenGameArt\TinyWorlds\Forest_Ambience.mp3'}
)

New-Item -ItemType Directory -Force -Path $sourceRoot,$downloadRoot | Out-Null
$manifest = foreach($asset in $assets){
  $destination=Join-Path $sourceRoot $asset.Relative
  New-Item -ItemType Directory -Force -Path (Split-Path $destination -Parent) | Out-Null
  if(-not(Test-Path -LiteralPath $destination)){ $referrer=if($asset.ContainsKey('Referrer')){$asset.Referrer}elseif($asset.Url -like "*solarsystemscope.com*"){"https://www.solarsystemscope.com/textures/"}else{"https://astrogeology.usgs.gov/"}; & curl.exe -L --fail --retry 2 -A "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/125 Safari/537.36" -e $referrer -H "Accept: audio/mpeg,audio/*;q=0.9,*/*;q=0.8" $asset.Url -o $destination; if($LASTEXITCODE -ne 0){throw "Download failed: $($asset.Url)"} }
  $file=Get-Item -LiteralPath $destination
  if($file.Length -lt 256){throw "Downloaded file is unexpectedly small: $destination"}
  [pscustomobject]@{Id=$asset.Id;SourceUrl=$asset.Url;RelativePath=$destination.Substring($ProjectRoot.Length).TrimStart('\');Bytes=$file.Length;Sha256=(Get-FileHash -LiteralPath $destination -Algorithm SHA256).Hash;DownloadedUtc=$file.LastWriteTimeUtc.ToString('o')}
}

$kenneyUrl='https://opengameart.org/sites/default/files/kenney_interfaceSounds.zip'
$kenneyZip=Join-Path $downloadRoot 'kenney_interfaceSounds.zip'
$kenneyDestination=Join-Path $sourceRoot 'Audio\KenneyInterfaceSounds'
if(-not(Test-Path -LiteralPath $kenneyZip)){ & curl.exe -L --fail --retry 2 -A "Mozilla/5.0" -e "https://opengameart.org/content/interface-sounds" $kenneyUrl -o $kenneyZip; if($LASTEXITCODE -ne 0){throw "Kenney download failed"} }
New-Item -ItemType Directory -Force -Path $kenneyDestination | Out-Null
Expand-Archive -LiteralPath $kenneyZip -DestinationPath $kenneyDestination -Force
$kf=Get-Item -LiteralPath $kenneyZip
$manifest += [pscustomobject]@{Id='AUD-KEN-001';SourceUrl=$kenneyUrl;RelativePath=$kenneyZip.Substring($ProjectRoot.Length).TrimStart('\');Bytes=$kf.Length;Sha256=(Get-FileHash -LiteralPath $kenneyZip -Algorithm SHA256).Hash;DownloadedUtc=$kf.LastWriteTimeUtc.ToString('o')}
$manifest | Sort-Object Id | Export-Csv -LiteralPath $manifestPath -NoTypeInformation -Encoding UTF8
Write-Output "Downloaded and verified $($manifest.Count) source packages."
Write-Output "Manifest: $manifestPath"
