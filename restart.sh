sh build.sh
docker stop ids4admin
docker rm ids4admin
docker run -d --name ids4admin  -e ASPNETCORE_ENVIRONMENT=Development --restart always -p 6566:6566 -p 6201:6201 ids4admin:latest /seed