using Autofac;
using Autofac.Extensions.DependencyInjection;
using Manager.API.Utility;
using Manager.API.Utility.AutofaExt;
using Manager.Core.AuthorizationModels;
using Manager.Core.Settings;
using Manager.Infrastructure.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

/* serilog */
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    //.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.File(Path.Combine($"C:/Serilog", @"log.txt"), rollingInterval: RollingInterval.Hour)
    .CreateLogger();

// add services to the container.
builder.Services
    .AddControllers(
    setup =>
    {
        //如果请求的类型和服务器请求的类型不一致就返回406
        setup.ReturnHttpNotAcceptable = true;
    }
)
    .AddNewtonsoftJson(
    options =>
    {
        //忽略循环引用
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        //大驼峰
        //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
        //小驼峰
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    }
);

//获取 appsetings 中的配置信息
var appSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(option => appSection.Bind(option));
var appSettings = appSection.Get<AppSettings>();

/* JWT[Json web token] */
builder.Services
    // 配置授权服务，也就是具体的规则，已经对应的权限策略，比如公司不同权限的门禁卡
    .AddAuthorization(options =>
    {
        // 自定义基于策略的授权权限   [Authorize(Policy= Policys.Permission)]
        options.AddPolicy(Policys.API, policy => policy.Requirements.Add(new PermissionRequirement()));
    })
    // 配置认证服务
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    // 配置JWT
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        //Token Validation Parameters
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //是否验证发行人
            ValidateIssuer = true,
            ValidIssuer = appSettings.JwtBearer.Issuer,//发行人
                                                       //是否验证受众人
            ValidateAudience = true,
            ValidAudience = appSettings.JwtBearer.Audience,//受众人
                                                           //是否验证密钥
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettings.JwtBearer.SecurityKey)),
            ValidateLifetime = true, //验证生命周期
            RequireExpirationTime = true, //过期时间
            ClockSkew = TimeSpan.Zero  //设置时钟偏斜为0
        };
    });

/* mysql */
builder.Services.AddDbContext<H5Context>(options =>
{
    options.UseMySql(appSettings.ConnectionString, new MySqlServerVersion(new Version(8, 0, 27)));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region Swagger

builder.Services.AddSwaggerGen(c =>
{
    foreach (FieldInfo field in typeof(ApiVersionInfo).GetFields())
    {
        c.SwaggerDoc(field.Name, new OpenApiInfo()
        {
            Title = " Server API ",
            Version = field.Name,
            Description = $"  API 的版本 {field.Name} "
        });
    }

    #region 为 Swagger Json and UI 设置 xml 文档注释路径

    string basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location); //获取应用程序所在目录

    string xmlPath = Path.Combine(basePath, "Manager.API.xml");

    c.IncludeXmlComments(xmlPath);

    #endregion 为 Swagger Json and UI 设置 xml 文档注释路径
});

#endregion Swagger

#region Cors

builder.Services.AddCors(policy =>
{
    policy.AddPolicy("CorsPolicy", opt =>
     //opt.WithOrigins(appSettings.CrosAddress)
     opt.AllowAnyOrigin()
     .AllowAnyHeader()
     .AllowAnyMethod()
     .WithExposedHeaders("X-Pagination")
    );
});

#endregion Cors

#region 弃用

/* ServiceCollection
 * 瞬时生命周期 -- Transient  每一次创建都是全新的实例
 * 单例生命周期 -- SingleTon  同一个类型，创建是来的是同一个实例
 * 作用域生命周期 -- Scoped   同一个serviceProvide获取到的是同一个实例
 */

/* authorization policy */
//builder.Services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
//builder.Services.AddScoped<IRoleModulePermissionService, RoleModulePermissionService>();

/* service 依赖注入 */
//builder.Services.AddScoped<IAccountService, AccountService>();
//builder.Services.AddScoped<IAccountInfoService, AccountInfoService>();
//builder.Services.AddScoped<IAuthenticateService, AuthenticateService>();
//builder.Services.AddScoped<IJwtService, JwtService>();
//builder.Services.AddScoped<ITencentService, TencentService>();
//builder.Services.AddScoped<IMailService, MailService>();
//builder.Services.AddScoped<ILogAvatarService, LogAvatarService>();
//builder.Services.AddScoped<ILogCoverService, LogCoverService>();
//builder.Services.AddScoped<IBlogService, BlogService>();
//builder.Services.AddScoped<IBlogTopicService, BlogTopicService>();
//builder.Services.AddScoped<IBlogImageService, BlogImageService>();
//builder.Services.AddScoped<IBlogVideoService, BlogVideoService>();
//builder.Services.AddScoped<IBlogLikeService, BlogLikeService>();
//builder.Services.AddScoped<IBlogCommentService, BlogCommentService>();
//builder.Services.AddScoped<IBlogForwardService, BlogForwardService>();
//builder.Services.AddScoped<IUserFocusService, UserFocusService>();
//builder.Services.AddScoped<IBlogCommentLikeService, BlogCommentLikeService>();
//builder.Services.AddScoped<IUserGroupService, UserGroupService>();
//builder.Services.AddScoped<IBlogForwardLikeService, BlogForwardLikeService>();
//builder.Services.AddScoped<IBlogImageLikeService, BlogImageLikeService>();
//builder.Services.AddScoped<IBlogFavoriteService, BlogFavoriteService>();
//builder.Services.AddScoped<IBlogVideoLikeService, BlogVideoLikeService>();
//builder.Services.AddScoped<IRankService, RankService>();

/* repository  依赖注入*/
//builder.Services.AddScoped<IBase, BaseRepository>();
//builder.Services.AddScoped<IProcedure, ProcedureRepository>();

#endregion 弃用

#region Autofac IOC 容器

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()); //通过工厂替换，把Autofac整合进来

//https://blog.csdn.net/xiaxiaoying2012/article/details/84617677
//http://niuyi.github.io/blog/2012/04/06/autofac-by-unit-test/
//https://www.cnblogs.com/gygtech/p/14491364.html

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    /* 通过继承 IDenpendency ，子类批量注入 */
    //Type basetype = typeof(IDenpendency);
    //containerBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
    //.Where(t => basetype.IsAssignableFrom(t) && t.IsClass)
    //.AsImplementedInterfaces().InstancePerLifetimeScope();

    //containerBuilder.RegisterGeneric(typeof(IList<>)).As(typeof(IList<>));

    /* 权限配置注入 */
    var authorizeAssembly = Assembly.Load("Manager.JwtAuthorizePolicy");
    containerBuilder.RegisterAssemblyTypes(authorizeAssembly).Where(t => t.IsClass && (t.Name.EndsWith("Service") || t.Name.EndsWith("Handler"))).AsImplementedInterfaces().InstancePerLifetimeScope();

    /* 业务逻辑注入 */
    var serverAssembly = Assembly.Load("Manager.Server");
    containerBuilder.RegisterAssemblyTypes(serverAssembly).Where(t => t.IsClass && t.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerLifetimeScope(); //InstancePerLifetimeScope 保证对象生命周期基于请求

    /* 仓储配置注入 */
    var repositoryAssembly = Assembly.Load("Manager.Infrastructure");
    containerBuilder.RegisterAssemblyTypes(repositoryAssembly).Where(t => t.IsClass && t.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerLifetimeScope();

    /* 注册每个控制器和抽象之间的关系 */
    var controllerBaseType = typeof(ControllerBase);
    containerBuilder.RegisterAssemblyTypes(typeof(Program).Assembly)
    .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
    .PropertiesAutowired(new CustomPropertySelector()); //注册属性注入
});

//支持控制器的实例让IOC容器来创建 -- autofac来创建
builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

#endregion Autofac IOC 容器

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    foreach (FieldInfo filed in typeof(ApiVersionInfo).GetFields())
    {
        c.SwaggerEndpoint($"/swagger/{filed.Name}/swagger.json", filed.Name);
    }
});
//}

app.UseAuthentication();  //JWT认证

app.UseAuthorization();   //JWT授权

app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();