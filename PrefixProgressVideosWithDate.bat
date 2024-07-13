@echo off
setlocal enabledelayedexpansion

rem Change to the relative target directory
set "target_dir=%~dp0progress"
cd /d "%target_dir%" || (
    echo Directory not found: %target_dir%
    pause
    exit /b
)

for %%F in (*.*) do (
    rem Check if the filename already starts with a number
    echo %%F | findstr /r "^[0-9]" >nul
    if errorlevel 1 (
        rem Get the last modified date of the file using PowerShell
        for /f %%A in ('powershell -NoProfile -Command "(Get-Item '%%F').LastWriteTime.ToString('yyyy-MM-dd')"') do (
            set "mod_date=%%A"
            rem Rename the file with the new prefix
            ren "%%F" "!mod_date!-%%F"
        )
    )
)

endlocal
pause
