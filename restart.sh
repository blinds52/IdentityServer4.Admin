#!/usr/bin/env bash
sh build.sh
docker stop ids4admin
docker rm ids4admin
docker pull registry.cn-shanghai.aliyuncs.com/xbzq/ids4admin:latest
docker run -d --name ids4admin --restart always -v /ids4admin:/ids4admin -p 6566:6566 -p 6201:6201 registry.cn-shanghai.aliyuncs.com/xbzq/ids4admin:latest /ids4admin/appsettings.json
docker logs ids4admin