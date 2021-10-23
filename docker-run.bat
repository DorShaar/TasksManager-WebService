@echo off

echo all versions:
docker images | grep tasker-server | awk '{print $2}' | sort

set /p id="Enter version to run: "
docker run -it -p 5000:80 -v /c/Dor/Apps/TaskerServer/db:/app/data tasker-server:%id%