#!/bin/bash
# Load existing images 

# Environment Variables
source "environment.sh"

# Load Dashboard
echo "Loading LnG CRET Tool Image...."
docker load < crettool_${DASHBOARD_VERSION}.docker