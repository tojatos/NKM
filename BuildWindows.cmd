@echo off
"E:\Program Files\Unity\Editor\Unity.exe" -batchmode -quit -nographics -projectPath "%cd%" -buildWindows64Player Builds/WindowsBuild/NKM.exe
echo Build finished
pause