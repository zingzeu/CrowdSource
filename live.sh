#!/bin/sh
docker-compose -f docker-compose.yml -f docker-compose.dev.yml \
   -f docker-compose.dev.live.yml build
docker-compose -f docker-compose.yml -f docker-compose.dev.yml \
   -f docker-compose.dev.live.yml up -d
docker-compose -f docker-compose.yml -f docker-compose.dev.yml \
   -f docker-compose.dev.live.yml exec web bash