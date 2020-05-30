using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MooyongCommon;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MooyongJz
{
    /// <summary>
    /// ������Ŀ
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
            ApiLogger.Configure(); //ʹ��ǰ������
            ApiAuthentication.Authentication();//����token����

            //���ÿ�����
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {
                    builder.AllowAnyOrigin() //�����κ���Դ����������
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();//ָ������cookie
                });
            });
            //����Swagger 
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("base", new OpenApiInfo
                {
                    Version = "base",
                    Title = "����ģ��API",
                    Description = "������Ϣ����ӿ�ƽ̨(��ģʽ����,���Ͻ��л�)",
                    Contact = new OpenApiContact { Name = "Fatefox", Email = "1121259730@qq.com" }
                });
                c.SwaggerDoc("service", new OpenApiInfo { Title = "����ģ��API", Version = "service" });
                c.SwaggerDoc("mom", new OpenApiInfo { Title = "ҵ��ģ��API", Version = "YW" });
                c.SwaggerDoc("dm", new OpenApiInfo { Title = "����ģ��API", Version = "QT" });

                //����Ҫչʾ�Ľӿ�
                c.DocInclusionPredicate((docName, apiDes) =>
                {
                    if (!apiDes.TryGetMethodInfo(out MethodInfo method))
                        return false;
                    /*ʹ��ApiExplorerSettingsAttribute�����GroupName�������Ա�ʶ
                     * DeclaringTypeֻ�ܻ�ȡcontroller�ϵ�����
                     * ��������������action������Ϊ��
                     * */
                    var version = method.DeclaringType.GetCustomAttributes(true).OfType<ApiExplorerSettingsAttribute>().Select(m => m.GroupName);
                    if (docName == "v1" && !version.Any())
                        return true;
                    //�����ȡaction������
                    var actionVersion = method.GetCustomAttributes(true).OfType<ApiExplorerSettingsAttribute>().Select(m => m.GroupName);
                    if (actionVersion.Any())
                        return actionVersion.Any(v => v == docName);
                    return version.Any(v => v == docName);
                });
                //�����Ȩ
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "���������Bearer��ͷ��Token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                });
                //��֤��ʽ���˷�ʽΪȫ�����
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
                //����SjiggJSON��UI��ע��·��.
                // Ϊ Swagger JSON and UI����xml�ĵ�ע��·��
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                var xmlPath = Path.Combine(basePath, "MooyongSwagger.xml");
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
                //var xmlmodelPath = Path.Combine(basePath, xmlFile);//���modelע��
                //c.IncludeXmlComments(xmlmodelPath);
                c.IncludeXmlComments(xmlPath, true);//controllerע��;��������,�������ĻḲ��ǰ���
            });
            // ���쳣������ע�뵽������
            services.AddScoped<GlobalExceptionFilter>();

            services.AddControllers()
                .AddJsonOptions(configure => {
                    //�������ڷ��ظ�ʽ
                    configure.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
                });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(option =>
                {
                    option.RequireHttpsMetadata = false;
                    option.SaveToken = true;
                    var token = Configuration.GetSection("tokenParameter").Get<TokenParameter>();
                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Secret)),
                        ValidIssuer = token.Issuer,
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero,/*token����ʱ��ƫ�Ĭ����5����*/
                    };
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

            //������֤����
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            //ExceptionMiddleware ����ܵ�
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseStaticFiles(); //��̬�ļ�����
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/base/swagger.json", "����ģ��API");
                c.SwaggerEndpoint("/swagger/service/swagger.json", "����ģ��API");
                c.SwaggerEndpoint("/swagger/mom/swagger.json", "ҵ��ģ��API");
                c.SwaggerEndpoint("/swagger/dm/swagger.json", "����ģ��API");
                c.ShowExtensions();
            });
        }
    }
}
