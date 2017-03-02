#!/bin/bash
cd ./src/CrowdSource

docker build -t crowdsource-build --file Dockerfile.build .
cd ../../
docker create --name build-cont crowdsource-build
docker cp build-cont:/out ./publish

docker rm build-cont