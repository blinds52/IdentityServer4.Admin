FROM microsoft/dotnet:2.1-aspnetcore-runtime
COPY . /app
WORKDIR /app
EXPOSE 6566
ENTRYPOINT ["dotnet","IdentityServer4.Admin.dll"]