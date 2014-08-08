:: синтаксис использования: ..\..\output_dll.cmd  ..\..\..\$(ProjectName)-$(ConfigurationName)

if not exist %1 mkdir %1
xcopy /Y *.dll %1
xcopy /Y *.pdb %1
xcopy /Y *.xml %1