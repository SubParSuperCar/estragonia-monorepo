@echo off
setlocal

echo Runtime context: Windows NT (CMD)

set "GD_PATH_CANDIDATES=godot.exe godot-mono.exe"

for %%G in (%GD_PATH_CANDIDATES%) do (
  for /f "delims=" %%P in ('where %%G 2^>nul') do (
    echo Godot found via PATH ^(%%G^): %%P
    "%%P" %*
    exit /b
  )
)

set "SCRIPT_DIR=%~dp0"
set "GD_PATH=%SCRIPT_DIR%bin\godot.exe"

if exist "%GD_PATH%" (
  echo Godot found via local bin: "%GD_PATH%"
  "%GD_PATH%" %*
  exit /b
)

echo Godot not found.
exit /b 1
