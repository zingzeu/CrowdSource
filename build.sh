#!/bin/bash
echo "Start Building......"
docker-compose -f docker-compose.build.yml run ci-build