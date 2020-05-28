using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MooyongCommon;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MooyongJz
{
    /// <summary>
    /// 启动项目
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            ApiLogger.Configure(); //使用前先配置

            //配置跨域处理
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {
                    builder.AllowAnyOrigin() //允许任何来源的主机访问
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();//指定处理cookie
                });
            });
            //配置Swagger 
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("base", new OpenApiInfo
                {
                    Version = "base",
                    Title = "基础模块API",
                    Description = "沐阳信息服务接口平台(多模式管理,右上角切换)",
                    Contact = new OpenApiContact { Name = "Fatefox", Email = "1121259730@qq.com" }
                });
                c.SwaggerDoc("service", new OpenApiInfo { Title = "服务模块API", Version = "service" });
                c.SwaggerDoc("mom", new OpenApiInfo { Title = "业务模块API", Version = "YW" });
                c.SwaggerDoc("dm", new OpenApiInfo { Title = "其他模块API", Version = "QT" });

                //设置要展示的接口
                c.DocInclusionPredicate((docName, apiDes) =>
                {
                    if (!apiDes.TryGetMethodInfo(out MethodInfo method))
                        return false;
                    /*使用ApiExplorerSettingsAttribute里面的GroupName进行特性标识
                     * DeclaringType只能获取controller上的特性
                     * 我们这里是想以action的特性为主
                     * */
                    var version = method.DeclaringType.GetCustomAttributes(true).OfType<ApiExplorerSettingsAttribute>().Select(m => m.GroupName);
                    if (docName == "v1" && !version.Any())
                        return true;
                    //这里获取action的特性
                    var actionVersion = method.GetCustomAttributes(true).OfType<ApiExplorerSettingsAttribute>().Select(m => m.GroupName);
                    if (actionVersion.Any())
                        return actionVersion.Any(v => v == docName);
                    return version.Any(v => v == docName);
                });
                //添加授权
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "请输入带有Bearer开头的Token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                });
                //认证方式，此方式为全局添加
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference()
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        }, Array.Empty<string>()
                    }
                });
                //c.OperationFilter<WebUserAPI.Common.AssignOperationVendorExtensions>();
                //设置SjiggJSON和UI的注释路径.
                // 为 Swagger JSON and UI设置xml文档注释路径
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
#pragma warning disable CS8604 // 可能的 null 引用参数。
                var xmlPath = Path.Combine(basePath, "MooyongSwagger.xml");
#pragma warning restore CS8604 // 可能的 null 引用参数。
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
                //var xmlmodelPath = Path.Combine(basePath, xmlFile);//添加model注释
                //c.IncludeXmlComments(xmlmodelPath);
                c.IncludeXmlComments(xmlPath, true);//controller注释;必须放最后,否则后面的会覆盖前面的
            });
            // 将异常过滤器注入到容器中
            services.AddScoped<GlobalExceptionFilter>();

            services.AddControllers()
                .AddJsonOptions(configure => {
                    //设置日期返回格式
                    configure.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
                });

            services.AddMvc();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            //ExceptionMiddleware 加入管道
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseStaticFiles(); //静态文件服务
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/base/swagger.json", "公共模块API");
                c.SwaggerEndpoint("/swagger/service/swagger.json", "服务模块API");
                c.SwaggerEndpoint("/swagger/mom/swagger.json", "业务模块API");
                c.SwaggerEndpoint("/swagger/dm/swagger.json", "其他模块API");
                c.ShowExtensions();
            });
        }
    }
}
