%~d0
cd %~dp0
buildTools\NuGet.exe install buildTools\packages.config -OutputDirectory packages -Verbosity normal
powershell.exe -NoProfile -ExecutionPolicy unrestricted -Command "& {if(-not (Get-Module -ListAvailable  VSSetup)){Install-PackageProvider -Name NuGet -MinimumVersion 2.8.5.201 -Force -Scope CurrentUser;Install-Module -Name VSSetup -Scope CurrentUser -Force;}Import-Module '.\packages\psake.*\tools\psake\psake.psd1';Import-Module '.\packages\Newbe.Build.Psake.*\tools\*.psm1'; invoke-psake .\build.ps1 %*; if ($LastExitCode -and $LastExitCode -ne 0) {write-host "ERROR CODE: $LastExitCode" -fore RED; exit $lastexitcode} }"
xcopy D:\git\QQRobot\src\PikachuRobot\Newbe.Mahua.Plugins.Pikachu\bin\MPQ  D:\empty\MPQ_Ver20190521 /e /y
start D:\empty\MPQ_Ver20190521\Core.exe
pause