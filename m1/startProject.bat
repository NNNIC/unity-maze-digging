cd /d %~dp0

if exist %~dp0unityProject goto :_open

start "" "%unitypath2018_2%" -noUPM -createproject %~dp0unityProject
goto :eof

:_open

start "" "%unitypath2018_2%" -noUPM -projectPath %~dp0unityProject
goto :eof