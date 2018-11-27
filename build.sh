dotnet publish -c Release -r linux-x64
cd ./src/IdentityServer4.Admin/bin/Release/netcoreapp2.1/linux-x64/publish/
docker build -t ids4admin .