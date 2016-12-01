tools\vs-build-all.exe -debug -release -m BuildAll.sln BuildAll.Fw4.sln
if %nopause%==true goto end
pause
:end