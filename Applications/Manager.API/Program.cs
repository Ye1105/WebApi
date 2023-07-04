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
        //�����������ͺͷ�������������Ͳ�һ�¾ͷ���406
        setup.ReturnHttpNotAcceptable = true;
    }
)
    .AddNewtonsoftJson(
    options =>
    {
        //����ѭ������
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        //���շ�
        //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
        //С�շ�
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    }
);

//��ȡ appsetings �е�������Ϣ
var appSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(option => appSection.Bind(option));
var appSettings = appSection.Get<AppSettings>();

/* JWT[Json web token] */
builder.Services
    // ������Ȩ����Ҳ���Ǿ���Ĺ����Ѿ���Ӧ��Ȩ�޲��ԣ����繫˾��ͬȨ�޵��Ž���
    .AddAuthorization(options =>
    {
        // �Զ�����ڲ��Ե���ȨȨ��   [Authorize(Policy= Policys.Permission)]
        options.AddPolicy(Policys.API, policy => policy.Requirements.Add(new PermissionRequirement()));
    })
    // ������֤����
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    // ����JWT
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        //Token Validation Parameters
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //�Ƿ���֤������
            ValidateIssuer = true,
            ValidIssuer = appSettings.JwtBearer.Issuer,//������
                                                       //�Ƿ���֤������
            ValidateAudience = true,
            ValidAudience = appSettings.JwtBearer.Audience,//������
                                                           //�Ƿ���֤��Կ
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettings.JwtBearer.SecurityKey)),
            ValidateLifetime = true, //��֤��������
            RequireExpirationTime = true, //����ʱ��
            ClockSkew = TimeSpan.Zero  //����ʱ��ƫбΪ0
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
            Description = $"  API �İ汾 {field.Name} "
        });
    }

    #region Ϊ Swagger Json and UI ���� xml �ĵ�ע��·��

    string basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location); //��ȡӦ�ó�������Ŀ¼

    string xmlPath = Path.Combine(basePath, "Manager.API.xml");

    c.IncludeXmlComments(xmlPath);

    #endregion Ϊ Swagger Json and UI ���� xml �ĵ�ע��·��
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

#region ����

/* ServiceCollection
 * ˲ʱ�������� -- Transient  ÿһ�δ�������ȫ�µ�ʵ��
 * ������������ -- SingleTon  ͬһ�����ͣ�������������ͬһ��ʵ��
 * �������������� -- Scoped   ͬһ��serviceProvide��ȡ������ͬһ��ʵ��
 */

/* authorization policy */
//builder.Services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
//builder.Services.AddScoped<IRoleModulePermissionService, RoleModulePermissionService>();

/* service ����ע�� */
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

/* repository  ����ע��*/
//builder.Services.AddScoped<IBase, BaseRepository>();
//builder.Services.AddScoped<IProcedure, ProcedureRepository>();

#endregion ����

#region Autofac IOC ����

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()); //ͨ�������滻����Autofac���Ͻ���

//https://blog.csdn.net/xiaxiaoying2012/article/details/84617677
//http://niuyi.github.io/blog/2012/04/06/autofac-by-unit-test/
//https://www.cnblogs.com/gygtech/p/14491364.html

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    /* ͨ���̳� IDenpendency ����������ע�� */
    //Type basetype = typeof(IDenpendency);
    //containerBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
    //.Where(t => basetype.IsAssignableFrom(t) && t.IsClass)
    //.AsImplementedInterfaces().InstancePerLifetimeScope();

    //containerBuilder.RegisterGeneric(typeof(IList<>)).As(typeof(IList<>));

    /* Ȩ������ע�� */
    var authorizeAssembly = Assembly.Load("Manager.JwtAuthorizePolicy");
    containerBuilder.RegisterAssemblyTypes(authorizeAssembly).Where(t => t.IsClass && (t.Name.EndsWith("Service") || t.Name.EndsWith("Handler"))).AsImplementedInterfaces().InstancePerLifetimeScope();

    /* ҵ���߼�ע�� */
    var serverAssembly = Assembly.Load("Manager.Server");
    containerBuilder.RegisterAssemblyTypes(serverAssembly).Where(t => t.IsClass && t.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerLifetimeScope(); //InstancePerLifetimeScope ��֤�����������ڻ�������

    /* �ִ�����ע�� */
    var repositoryAssembly = Assembly.Load("Manager.Infrastructure");
    containerBuilder.RegisterAssemblyTypes(repositoryAssembly).Where(t => t.IsClass && t.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerLifetimeScope();

    /* ע��ÿ���������ͳ���֮��Ĺ�ϵ */
    var controllerBaseType = typeof(ControllerBase);
    containerBuilder.RegisterAssemblyTypes(typeof(Program).Assembly)
    .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
    .PropertiesAutowired(new CustomPropertySelector()); //ע������ע��
});

//֧�ֿ�������ʵ����IOC���������� -- autofac������
builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

#endregion Autofac IOC ����

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

app.UseAuthentication();  //JWT��֤

app.UseAuthorization();   //JWT��Ȩ

app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();