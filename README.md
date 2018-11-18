
### Develop

        dotnet ef migrations add admin  --c AdminDbContext
        dotnet ef migrations add id4configuration  -c ConfigurationDbContext
        dotnet ef migrations add id4persistedGrants  -c PersistedGrantDbContext
        
### Usage

        dotnet ef database update -c AdminDbContext
        dotnet ef database update -c ConfigurationDbContext
        dotnet ef database update -c PersistedGrantDbContext                                 