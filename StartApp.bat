@echo off

:: Get the directory where this .bat file is located
set CURRENT_DIR=%~dp0

:: Output the current directory for debugging purposes
echo Current directory: %CURRENT_DIR%

:: Navigate to the 'publish' directory relative to the location of the .bat file
pushd "%CURRENT_DIR%\DataLink\DataLink\bin\Release\net8.0\win-x64\publish"

:: Output the current directory after the pushd to ensure it's correct
echo Now in directory: %CD%

:: Check if DataLink.exe exists in the directory
if exist "DataLink.exe" (
    echo DataLink.exe found, starting the application...
    start "" DataLink.exe
) else (
    echo Error: DataLink.exe not found in the directory!
)

:: Wait for 2 seconds
timeout /t 2 > nul

:: Open the web page in the browser
start "" http://localhost:5000

:: Return to the original directory after the pushd
popd
