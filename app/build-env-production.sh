#!/usr/bin/env bash
# Build a .env.production configuration file
#
# .env.production can be useful for building front-end assets
# if passing variables and arguments to build tools does not work
# (this is the case in some build environments)
#
# Usage: build-env-production.sh <ApiUrl>
#   ApiUrl: URL of Application API

set -euo pipefail

API_URL="${1:-}"

echo "ApiUrl: $API_URL"

sed "s|VITE_API_URL=|VITE_API_URL=$API_URL|" .env > .env.production
