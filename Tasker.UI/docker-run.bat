@echo off

echo all versions:
docker images | grep tasker-ui | awk '{print $2}' | sort

set /p id="Enter version to run: "
set /p port="Enter your localhost port: "

docker run -it -p %port%:80 tasker-ui:%id%