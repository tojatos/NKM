#!/bin/bash
PROJECT_PATH=$(readlink -f $(dirname $(readlink -f $0))/..)
UNITY_EDITOR_PATH=$HOME/Unity/Hub/Editor/
UNITY_EXEC=$(find ${UNITY_EDITOR_PATH}*/Editor -maxdepth 1 -type f -name Unity | tail -n1)

${UNITY_EXEC} -quit -batchmode -executeMethod Editor.BuildManagement.BuildLinuxPlayer -projectPath ${PROJECT_PATH} -logFile /dev/stdout
