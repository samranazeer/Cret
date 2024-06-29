#!/bin/bash

# Environment Variables
source "../environment.sh"

docker build -t crettool:${DASHBOARD_VERSION} .
