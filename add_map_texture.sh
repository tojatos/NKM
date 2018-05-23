#!/bin/bash


##### Functions

usage()
{
    printf "Usage: ./add_map_texture.sh [INPUT_FILE] [OUTPUT_FILE_NAME]\n"
	printf "Copy INPUT_FILE to maps folder\n" 
}

##### Main

if [ "$1" = "--help" ] || [ "$1" = "-h" ]; then
  usage
  exit
fi

cp "$1" "/home/tojatos/UnityProjects/NKM/Assets/Sprites/Maps/$2" 
