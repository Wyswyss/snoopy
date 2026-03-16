@echo off
setlocal
if "%1"=="" (
  set RID=win-x64
) else (
  set RID=%1
)

powershell -ExecutionPolicy Bypass -File "%~dp0build-release.ps1" -Runtime %RID%
if errorlevel 1 exit /b 1

echo Release package generated at windows\release\Snoopy.Windows-%RID%.zip
