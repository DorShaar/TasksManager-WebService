@echo off

echo last version:
docker images | grep tasker-ui | awk '{print $2}' | sort | tail -1

set /p id="Enter next version: "
docker build . -t tasker-ui:%id%