dotnet restore BuildAll.sln
dotnet build -c Release BuildAll.sln -m -interactive:False -v:q -nologo
pause