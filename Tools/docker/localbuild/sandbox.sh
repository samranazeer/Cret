#!/bin/bash
# Sand box script for CRET Tool local docker based build

# Environment Variables
source "environment.sh"

timezone="$(cat /etc/timezone)"
detachedmode="" # by default in detached mode
operation=""
attachlog=""


POSITIONAL=()
while [[ $# -gt 0 ]]
do
key="$1"

case $key in
    -d|--detached)
    # Run the containers in detached mode
    detachedmode="-d"
    shift # past argument
    ;;
    -l|--log)
    # Attach to output console
    attachlog="log"
    shift # past argument
    ;;
    -p|--operation)
    # docker-compose up or down
    if [[ $2 = "down" ]]; then
       operation="down"
    elif [[ $2 = "up" ]]; then
       operation="up"
    fi
    shift # past argument
    shift # past value
    ;;
    --default)
    DEFAULT=YES
    shift # past argument
    ;;
    *)    # unknown option
    POSITIONAL+=("$1") # save it in an array for later
    shift # past argument
    ;;
esac
done
set -- "${POSITIONAL[@]}" # restore positional parameters

if [[ $operation = "up" ]]
then
docker compose up $detachedmode
elif [[ $operation = "down" ]]; 
then
docker compose down
elif [[ $attachlog = "log" ]]; 
then
docker compose logs -f -t # Attach to all running containers with timestamps
else
echo "Invalid parameters provided"
fi
