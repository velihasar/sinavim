using Business;
using Business.Helpers;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Extensions;
using Core.Utilities.IoC;
using Core.Utilities.Security.Encyption;
using Core.Utilities.Security.Jwt;
using Core.Utilities.TaskScheduler.Hangfire;
using Core.Utilities.TaskScheduler.Hangfire.Models;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Globalization;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ConfigurationManager = Business.ConfigurationManager;

namespace WebAPI
{
    /// <summary>
    ///
    /// </summary>
    public partial class Startup : BusinessStartup
    {
        /// <summary>
        /// Constructor of <c>Startup</c>
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="hostEnvironment"></param>
        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
            : base(configuration, hostEnvironment)
        {
        }


        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <remarks>
        /// It is common to all configurations and must be called. Aspnet core does not call this method because there are other methods.
        /// </remarks>
        /// <param name="services"></param>
        public override void ConfigureServices(IServiceCollection services)
        {
            // Coolify / ortam değişkenleri — Cüzdanım ile aynı anahtarlar (DB_*, JWT_*, SMTP_*, …).
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            var dbUser = Environment.GetEnvironmentVariable("DB_USER");
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
            var hangfireDbName = Environment.GetEnvironmentVariable("HANGFIRE_DB_NAME") ?? "sinavim_hangfire";
            var hangfireConnectionString = Environment.GetEnvironmentVariable("HANGFIRE_CONNECTION_STRING");

            if (!string.IsNullOrEmpty(dbHost) && !string.IsNullOrEmpty(dbPort) &&
                !string.IsNullOrEmpty(dbName) && !string.IsNullOrEmpty(dbUser) &&
                !string.IsNullOrEmpty(dbPassword))
            {
                if (string.IsNullOrEmpty(hangfireConnectionString))
                {
                    hangfireConnectionString =
                        $"Host={dbHost};Port={dbPort};Database={hangfireDbName};Username={dbUser};Password={dbPassword};Command Timeout=30;Timeout=30;";
                }

                var pgConnectionString =
                    $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};Command Timeout=60;Timeout=60;Connection Lifetime=0;Pooling=true;MinPoolSize=1;MaxPoolSize=20;";

                Configuration["ConnectionStrings:DArchPgContext"] = pgConnectionString;
                Configuration["TaskSchedulerOptions:ConnectionString"] = hangfireConnectionString;
                Configuration["SeriLogConfigurations:PostgreConfiguration:ConnectionString"] = pgConnectionString;
            }
            else if (!string.IsNullOrEmpty(hangfireConnectionString))
            {
                Configuration["TaskSchedulerOptions:ConnectionString"] = hangfireConnectionString;
            }

            var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
            var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
            var jwtSecurityKey = Environment.GetEnvironmentVariable("JWT_SECURITY_KEY");
            if (!string.IsNullOrEmpty(jwtIssuer))
                Configuration["TokenOptions:Issuer"] = jwtIssuer;
            if (!string.IsNullOrEmpty(jwtAudience))
                Configuration["TokenOptions:Audience"] = jwtAudience;
            if (!string.IsNullOrEmpty(jwtSecurityKey))
                Configuration["TokenOptions:SecurityKey"] = jwtSecurityKey;

            var googleClientId0 = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID_0");
            var googleClientId33 = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID_33");
            if (!string.IsNullOrEmpty(googleClientId0))
                Configuration["GoogleAuth:ClientIds:0"] = googleClientId0;
            if (!string.IsNullOrEmpty(googleClientId33))
                Configuration["GoogleAuth:ClientIds:33"] = googleClientId33;

            void SetFromEnv(string envName, string configKey)
            {
                var v = Environment.GetEnvironmentVariable(envName);
                if (!string.IsNullOrEmpty(v))
                    Configuration[configKey] = v;
            }

            SetFromEnv("BASE_URL", "AppSettings:BaseUrl");
            SetFromEnv("FRONTEND_URL", "AppSettings:FrontendUrl");
            SetFromEnv("SMTP_SERVER", "EmailConfiguration:SmtpServer");
            SetFromEnv("SMTP_PORT", "EmailConfiguration:SmtpPort");
            SetFromEnv("SMTP_SENDER_NAME", "EmailConfiguration:SenderName");
            SetFromEnv("SMTP_SENDER_EMAIL", "EmailConfiguration:SenderEmail");
            SetFromEnv("SMTP_USERNAME", "EmailConfiguration:UserName");
            SetFromEnv("SMTP_PASSWORD", "EmailConfiguration:Password");
            SetFromEnv("REDIS_HOST", "CacheOptions:Host");
            SetFromEnv("REDIS_PORT", "CacheOptions:Port");
            SetFromEnv("REDIS_PASSWORD", "CacheOptions:Password");
            SetFromEnv("RABBITMQ_HOST", "MessageBrokerOptions:HostName");
            SetFromEnv("RABBITMQ_USERNAME", "MessageBrokerOptions:UserName");
            SetFromEnv("RABBITMQ_PASSWORD", "MessageBrokerOptions:Password");
            SetFromEnv("ELASTICSEARCH_URL", "ElasticSearchConfig:ConnectionString");
            SetFromEnv("ELASTICSEARCH_USERNAME", "ElasticSearchConfig:UserName");
            SetFromEnv("ELASTICSEARCH_PASSWORD", "ElasticSearchConfig:Password");
            SetFromEnv("MONGODB_CONNECTIONSTRING", "MongoDbSettings:ConnectionString");
            SetFromEnv("MONGODB_DATABASE", "MongoDbSettings:DatabaseName");
            SetFromEnv("TEAMS_WEBHOOK_URL", "SeriLogConfigurations:MSTeamsConfiguration:ChannelHookAdress");
            SetFromEnv("HANGFIRE_USERNAME", "TaskSchedulerOptions:Username");
            SetFromEnv("HANGFIRE_PASSWORD", "TaskSchedulerOptions:Password");

            // Business katmanında olan dependency tanımlarının bir metot üzerinden buraya implemente edilmesi.

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

            services.AddApiVersioning(v =>
            {
                v.DefaultApiVersion = new ApiVersion(1, 0);
                v.AssumeDefaultVersionWhenUnspecified = true;
                v.ReportApiVersions = true;
                v.ApiVersionReader = new HeaderApiVersionReader("x-dev-arch-version");
            });

            // CORS — JWT header ile çalıştığı için cookie/credentials gerekmez; AllowAnyOrigin kullanılabilir.
            services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowOrigin",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            var tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // Claim mapping'i devre dışı bırak - tam URI formatında tut (SecuredOperation hatasını çözmek için eklendi)
                    options.MapInboundClaims = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = tokenOptions.Issuer,
                        ValidAudience = tokenOptions.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey),
                        ClockSkew = TimeSpan.Zero
                    };

                    // SignalR için JWT authentication
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var path = context.HttpContext.Request.Path;

                            // Sadece SignalR hub istekleri için query parameter'dan token al
                            if (path.StartsWithSegments("/notificationhub"))
                            {
                                var accessToken = context.Request.Query["access_token"];
                                if (!string.IsNullOrEmpty(accessToken))
                                {
                                    context.Token = accessToken;
                                }
                            }
                            // Normal API istekleri için hiçbir şey yapma (default Authorization header kullanılır)

                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            // SignalR isteklerinde authentication hatası olursa log'la ama normal API'yi etkileme
                            if (context.Request.Path.StartsWithSegments("/notificationhub"))
                            {
                                Console.WriteLine($"SignalR Authentication Failed: {context.Exception.Message}");
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            // SignalR ekle
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true; // Development için detaylı hatalar
                options.MaximumReceiveMessageSize = 1024 * 1024; // 1MB
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
                options.KeepAliveInterval = TimeSpan.FromSeconds(15);
            });


            services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments(Path.ChangeExtension(typeof(Startup).Assembly.Location, ".xml"));
            });

            services.AddTransient<FileLogger>();
            services.AddTransient<PostgreSqlLogger>();
            services.AddTransient<MsSqlLogger>();
            services.AddScoped<IpControlAttribute>();

            base.ConfigureServices(services);
        }


        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // ✅ Auto Migration
            using (var scope = app.ApplicationServices.CreateScope())
            {
                try
                {
                    var db = scope.ServiceProvider.GetRequiredService<DataAccess.Concrete.EntityFramework.Contexts.ProjectDbContext>();
                    db.Database.Migrate(); // deploy sırasında migrationları uygular
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Migration hatası: {ex.Message}");
                    // Hata olsa bile uygulamanın çalışmasına devam etsin
                }
            }

            // VERY IMPORTANT. Since we removed the build from AddDependencyResolvers, let's set the Service provider manually.
            // By the way, we can construct with DI by taking type to avoid calling static methods in aspects.
            ServiceTool.ServiceProvider = app.ApplicationServices;


            var configurationManager = app.ApplicationServices.GetService<ConfigurationManager>();
            switch (configurationManager.Mode)
            {
                case ApplicationMode.Development:
                    _ = app.UseDbFakeDataCreator();
                    break;

                case ApplicationMode.Profiling:
                case ApplicationMode.Staging:

                    break;
                case ApplicationMode.Production:
                    break;
            }

            app.UseDeveloperExceptionPage();



            app.ConfigureCustomExceptionMiddleware();

            _ = app.UseDbOperationClaimCreator();

            // Swagger'ı her environment'ta aktif et
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Mam Yazilim");
                c.DocExpansion(DocExpansion.None);
            });
            // Development’ta HTTP→HTTPS yönlendirmesi telefon/Expo Go’yu kırar (Location: https://localhost:5001).
            if (!env.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();
            app.UseCors("AllowOrigin");
            app.UseAuthentication();

            app.UseAuthorization();

            // Make Turkish your default language. It shouldn't change according to the server.
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("tr-TR"),
            });

            var cultureInfo = new CultureInfo("tr-TR");
            cultureInfo.DateTimeFormat.ShortTimePattern = "HH:mm";

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            app.UseStaticFiles();

            var taskSchedulerConfig = Configuration.GetSection("TaskSchedulerOptions").Get<TaskSchedulerConfig>();

            if (taskSchedulerConfig.Enabled)
            {
                // Hangfire'ın DI container'ını kullanmasını sağla
                // app.ApplicationServices, Autofac'ın üzerindeki ASP.NET Core DI abstraction'ıdır
                // Bu sayede Autofac veya başka bir DI container kullanılsa bile çalışır
                GlobalConfiguration.Configuration.UseActivator(new ServiceProviderJobActivator(app.ApplicationServices));

                app.UseHangfireDashboard(taskSchedulerConfig.Path, new DashboardOptions
                {
                    DashboardTitle = taskSchedulerConfig.Title,
                    Authorization = new[]
                    {
                        new HangfireCustomBasicAuthenticationFilter
                        {
                            User = taskSchedulerConfig.Username,
                            Pass = taskSchedulerConfig.Password
                        }
                    }
                });
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<Core.Hubs.NotificationHub>("/notificationhub");
            });
        }
    }
}