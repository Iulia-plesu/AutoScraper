@echo off
setlocal

set CURRENT_DIR=%~dp0
set DATA_LINK_DIR=%CURRENT_DIR%DataLink\DataLink\bin\Release\net8.0

if not exist "%DATA_LINK_DIR%" (
    echo Error: DataLink directory not found!
    exit /b
)

set EXE_PATH=%DATA_LINK_DIR%\DataLink.exe

if exist "%EXE_PATH%" (
    echo Starting DataLink.exe...
    start "" "%EXE_PATH%"
) else (
    echo Error: DataLink.exe not found!
    exit /b
)

:: Wait up to 10 seconds for the server to become available
set RETRIES=10
:check_loop
timeout /t 1 > nul
curl --silent http://localhost:5000 > nul 2>&1
if %errorlevel% neq 0 (
    set /a RETRIES=%RETRIES%-1
    if %RETRIES% gtr 0 goto check_loop
    echo Server did not start in time.
    exit /b
)

start "" http://localhost:5000
endlocal
