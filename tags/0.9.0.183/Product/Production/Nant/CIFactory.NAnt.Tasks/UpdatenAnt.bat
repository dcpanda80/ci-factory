:: "$(ProjectDir)UpdatenAnt.bat" "$(SolutionDir)" "$(TargetDir)" $(ProjectName)
cd %1
cd ..\Third Party\nAnt\bin

FOR /F "TOKENS=1 DELIMS=," %%A IN ('cd') DO SET Dest=%%A

cd %2
copy %3.dll "%Dest%"
copy %3.pdb "%Dest%"
copy NDepend.Helpers.FileDirectoryPath.dll "%Dest%"

cd %1
nant -buildfile:DevEnv\SetupScripts\SetUp.xml