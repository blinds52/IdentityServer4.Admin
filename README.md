# IdentityServer4.Admin

[![Build Status](https://dev.azure.com/zlzforever/IdentityServer4.Admin/_apis/build/status/zlzforever.IdentityServer4.Admin)](https://dev.azure.com/zlzforever/IdentityServer4.Admin/_build/latest?definitionId=1)

##### 项目介绍

基于 IdentityServer4 开发的授权、用户管理、角色管理、权限管理

##### 开发说明

添加 EF 迁移配置

        dotnet ef migrations add admin  -c AdminDbContext 
        dotnet ef migrations add id4configuration  -c ConfigurationDbContext
        dotnet ef migrations add id4persistedGrants  -c PersistedGrantDbContext 
        
执行数据库迁移

        dotnet ef database update -c AdminDbContext
        dotnet ef database update -c ConfigurationDbContext
        dotnet ef database update -c PersistedGrantDbContext                                

##### 功能说明

权限

| feature | detail  | permission |   state |
|----|----|----|----|
| 添加权限        | 权限名不能重复, 不能添加名为 admin 的权限  | admin |   ☑    |
| 修改权限 |    权限名不能重复       | admin |   ☑    |
| 删除权限 |  删除权限对象，删除用户权限记录，删除角色权限记录         | admin |   ☑   |
| 查询权限    |           |   admin  |  ☑   |

角色 

| feature | detail  | permission |   state |
|----|----|----|----|
| 添加角色    |  角色名不能重复  | admin |   ☑    |
| 修改角色 |          | admin |   ☑    |
| 删除角色 |  删除角色对象，删除用户角色记录，admin 角色不能删除          | admin |   ☑   |
| 查询角色    |           |   admin  |  ☑   |
| 添加角色权限    |   admin 角色不能添加权限        |   admin  |  ☑   |
| 删除角色权限    |  admin 角色不能删除权限    |   admin  |  ☑   |

用户 

| feature | detail  | permission |   state |
|----|----|----|----|
| 添加用户        |  | admin |   ☑    |
| 修改用户 |           | admin |   ☑    |
| 删除用户 |   标记删除, 不删除用户角色, 用户权限记录, 删除用户后此用户将不能再登录        | admin |   ☑   |
| 查询用户    |           |   admin  |  ☑   |
| 修改密码    |  不需要原始密码         |   admin  |  ☑   |
| 添加用户角色    |   admin 用户不能添加角色        |   admin  |  ☑   |
| 删除用户角色    |   admin 用户不能删除角色        |   admin  |  ☑   |  
| 添加用户权限    |  admin 用户不能添加权限         |   admin  |  ☑   |
| 删除用户权限    |  admin 用户不能删除权限         |   admin  |  ☑   |    
| 查询用户 Profile    |           |   login  |  ☐    |  
| 修改密码    |   需要原始密码        |   login  |  ☐  |

审记

| feature | detail  | permission |   state |
|----|----|----|----|
| 记录所有操作        |  | login |   ☐   |

#### Run test docker instance

        docker run -d --name ids4admin -e ASPNETCORE_ENVIRONMENT=Development --restart always -p 80:6566 ids4admin /seed