

<h1 align="center" >🐰 WebApi </h1>  

<div align="center"> 
<p> WebApi 是一个仿实现微博后端接口的自用 API 项目，基于.NET , 支持 .NET 6.0 + 。</p>
</div>




<div align="center" style="color:gray"> 
    中文 
</div>


## :zap: 功能特性
+ :thought_balloon: 实现腾讯云短信、邮件验证登陆  
+ :boom: 实现 [JSON Web Tokens](https://jwt.io/) 认证、[ASP.NET Authorization](https://learn.microsoft.com/zh-cn/aspnet/core/security/authorization/policies?view=aspnetcore-6.0) 自定义权限策略  
+ ⛳ 实现 [JsonSchema](http://json-schema.org/) 参数校验配置  
+ :palm_tree: 实现 [RESTful API](https://restfulapi.cn/) 路由配置  
+ :pencil: 实现 AOP 日志记录和异常捕捉  
+ :sparkles: 基于 Lucence ，实现全文索引库检索  
+ :beers: 基于 [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)  
+ :newspaper: 基于 [Mysql](https://www.mysql.com/cn/) 、[Redis](https://redis.io/) 



## 🔖 项目结构

> 项目基础结构目录。`Tips：后续随着项目优化可能会有小的改动`

```C#
 Project 
    ├── Manager.API                     
    |   ├── Controllers                         	
    |   ├── Utilities                         
    |   |   ├── AutofaExt                           
    |   |   ├── Filters                         
    |   |   └── Schemas                        
    |   ├── appsettings.json                    
    |   ├── jsonschemas.json                    
    |   ├── app.config                         
    |   └── Program.cs                            
    ├── Manager.Core                   
    |   ├── AuthorizationModels                       
    |   ├── Enums                       
    |   ├── Models                         
    |   ├── Page                       
    |   ├── RequestModels              
    |   ├── ResponseModels                         	
    |   ├── Settings                                                  
    |   └── Tencent                            
    ├── Manager.JwtAuthorizePolicy                
    |   └── Handler  
    ├── Manager.Server  
    |   ├── IServices                   	
    |   └── Services       
    ├── Manager.SearchEngine  
    |   ├── Analyzers      
    |   ├── Engines  
    |   └── Tokenizers                	              
    └── Manager.Infrastructure
        ├── IRepositoies
        └── Repositoies
```

## 💻项目进度

> 项目基础框架基本搭建完成。目前正在做部分接口的性能优化和全文检索库的搭建，可以 `Star` :star: 关注一下，:pray:谢谢。

## 📄项目规范

> `Clone` 项目后，可以在 `Apifox`  或  `Swagger`  中查看项目中数据接口的请求参数、请求方式和请求规则 `JsonSchema` 等等，推荐使用 `Apifox`

+  [Apifox API 在线接口文档](https://k6cos2vvio.apifox.cn)

## :rainbow:免责说明

+ 请尊重版权、开源和原创。

## 💕联系作者

> 对项目有疑问、建议或期待的朋友可以加我好友。

- **wechat** ：`yejiancong1105`