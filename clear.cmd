@del /Q /A:H *.suo
@del /Q /S *.ncb
@del /Q /S *.cache
@del /Q /S *.user
@del /Q /S *.resharper

@for /D %%i in (*) do @(
	@cd %%i
	@rmdir /Q /S bin
	@rmdir /Q /S obj
	@if exist clear.cmd @call clear
	@cd ..
)

@for /D %%i in (*Release) do @(
	@rmdir /Q /S %%i
)

@for /D %%i in (*Debug) do @(
	@rmdir /Q /S %%i
)