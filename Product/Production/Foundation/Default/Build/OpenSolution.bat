set PATH=%SystemRoot%\system32;%SystemRoot%;%SystemRoot%\System32\Wbem;%BuildDirectory%\nAnt\bin
call "C:\Program Files\Microsoft Visual Studio 8\VC\vcvarsall.bat"
devenv "%ProjectName%BuildScripts.sln"
