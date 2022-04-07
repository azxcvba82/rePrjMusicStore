using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
//using Swashbuckle.Swagger;
//using Microsoft.OpenApi.Models;
//using Swashbuckle.AspNetCore.Swagger;
//using Swashbuckle.AspNetCore.SwaggerGen;
//using Music_store_backend.Swagger;
//using Music_store_backend.Middlewares;
//using Music_store_backend.Infrastructure.Dapper;
using Music_store_backend.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Serialization;
using NSwag;
using NSwag.Generation.Processors.Security;
using System.Security.Principal;
using Music_store_backend.Model;

namespace Music_store_backend
{
    public class Startup
    {

        //Cors policy name
        private const string _defaultCorsPolicyName = "MusicStore.Cors";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Dependency Injection Services

            //Load assembly from appsetting.json
            //
            string assemblies = Configuration["Assembly:FunctionAssembly"];

            if (!string.IsNullOrEmpty(assemblies))
            {
                foreach (var item in assemblies.Split('|'))
                {
                    Assembly assembly = Assembly.Load(item);
                    foreach (var implement in assembly.GetTypes())
                    {
                        Type[] interfaceType = implement.GetInterfaces();
                        foreach (var service in interfaceType)
                        {
                            services.AddTransient(service, implement);
                        }
                    }
                }
            }

            #endregion

            #region NSwag setting

            //Nuget install 1.NSwag.AspNetCore 2.AspNetCore.Authentication.JwtBearer

            var appSettings = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettings);
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddControllers().AddNewtonsoftJson(options =>
           options.SerializerSettings.ContractResolver =
              new CamelCasePropertyNamesContractResolver());

            services.AddOpenApiDocument(document =>
            {
                document.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Copy 'Bearer ' + valid JWT token into field"
                });

                document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
                document.Title = "Music Backend ";
            });

            services.AddTransient<IPrincipal>(_ => new UserPrincipal("",""));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            }).AddJwtBearer("JwtBearer", jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("webuser1gogo#123456")),

                    ValidateIssuer = true,
                    ValidIssuer = "BackendM",

                    ValidateAudience = true,
                    ValidAudience = "BackendM",

                    ValidateLifetime = true, //validate the expiration and not before values in the token

                    ClockSkew = TimeSpan.FromMinutes(5) //5 minute tolerance for the expiration date
                };
            });
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {

                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("NormalUser", policy =>
                      policy.RequireRole("NormalUser", "SubscriptionUser", "WebAdmin", "SysAdmin", "SuperAdmin"));

                options.AddPolicy("SubscriptionUser", policy =>
                    policy.RequireRole("SubscriptionUser", "WebAdmin", "SysAdmin", "SuperAdmin"));

                options.AddPolicy("WebAdmin", policy =>
                      policy.RequireRole("WebAdmin", "SysAdmin", "SuperAdmin"));

                options.AddPolicy("SysAdmin", policy =>
                     policy.RequireRole("SysAdmin", "SuperAdmin"));

                options.AddPolicy("SuperAdmin", policy =>
                     policy.RequireRole("SuperAdmin"));
            });


            #endregion

            #region Configure Jwt Authentication (Swashbuckle)

            ////Use Jwt bearer authentication
            ////
            //string issuer = Configuration["Jwt:Issuer"];
            //string audience = Configuration["Jwt:Audience"];
            //string expire = Configuration["Jwt:ExpireMinutes"];
            //TimeSpan expiration = TimeSpan.FromMinutes(Convert.ToDouble(expire));
            //SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SecurityKey"]));

            //services.AddAuthorization(options =>
            //{
            //    //1、Definition authorization policy
            //    options.AddPolicy("Permission",
            //       policy => policy.Requirements.Add(new PolicyRequirement()));
            //}).AddAuthentication(s =>
            //{
            //    //2、Authentication
            //    s.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    s.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            //    s.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //}).AddJwtBearer(s =>
            //{
            //    //3、Use Jwt bearer 
            //    s.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true, //add
            //        ValidateIssuer = true, //add
            //        ValidIssuer = issuer,
            //        ValidateAudience = true, //add
            //        ValidAudience = audience,
            //        IssuerSigningKey = key,
            //        ClockSkew = expiration,
            //        ValidateLifetime = true
            //    };
            //    s.Events = new JwtBearerEvents
            //    {
            //        OnAuthenticationFailed = context =>
            //        {
            //            //Token expired
            //            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            //            {
            //                context.Response.Headers.Add("Token-Expired", "true");
            //            }
            //            return Task.CompletedTask;
            //        }
            //    };
            //});

            ////DI handler process function
            //services.AddSingleton<IAuthorizationHandler, PolicyHandler>();

            #endregion

            #region MVC

            services.AddRouting(options => options.LowercaseUrls = true);

            //.NET CORE3.0 CORS policy不用在這裡設定 CorsAuthorizationFilterFactory(_defaultCorsPolicyName)
            //services.AddMvc(
            //    options => options.Filters.Add(new CorsAuthorizationFilterFactory(_defaultCorsPolicyName))
            //).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddMvc(option => option.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            #endregion

            #region Configure Cors

            //services.AddCors(options => options.AddPolicy(_defaultCorsPolicyName,
            //    builder => builder.WithOrigins(
            //            Configuration["Application:CorsOrigins"]
            //            .Split(",", StringSplitOptions.RemoveEmptyEntries).ToArray()
            //        )
            //    .AllowAnyHeader()
            //    .AllowAnyMethod()
            //    .AllowCredentials()));

            services.AddCors(options =>
            {
                options.AddPolicy(_defaultCorsPolicyName,
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

                options.AddPolicy("WhiteListCorsPolicy",
                    builder => builder.WithOrigins(
                        "https://musicstore.web.windows.net",
                        "http://localhost:22678", "https://localhost:44345",
                        "http://localhost:4200", "https://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            #endregion

            #region Configure API Version (Swashbuckle)
            // services.AddControllers().AddNewtonsoftJson(options =>
            //options.SerializerSettings.ContractResolver =
            //   new CamelCasePropertyNamesContractResolver());
            // services.AddApiVersioning(o =>
            // {
            //     o.ReportApiVersions = true;//return versions in a response header
            //     o.DefaultApiVersion = new ApiVersion(1, 0);//default version select 
            //     o.AssumeDefaultVersionWhenUnspecified = true;//if not specifying an api version,show the default version
            // }).AddVersionedApiExplorer(option =>
            // {
            //     option.GroupNameFormat = "'v'VVVV";//api group name
            //     option.AssumeDefaultVersionWhenUnspecified = true;//whether provide a service API version
            // });

            #endregion

            #region Configure Swagger (Swashbuckle)

            //services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //services.AddSwaggerGen(s =>
            //{
            //    //Generate api description doc
            //    //
            //    var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

            //    foreach (var description in provider.ApiVersionDescriptions)
            //    {
            //        s.SwaggerDoc(description.GroupName, new OpenApiInfo
            //        {
            //            //contact = new Contact
            //            //{
            //            //    name = "Danvic Wang",
            //            //    email = "danvic96@hotmail.com",
            //            //    url = "https://yuiter.com"
            //            //},
            //            Description = "A front-background project build by ASP.NET Core 2.1 and Vue",
            //            Title = "Music_store_backend.VuCore",
            //            Version = description.ApiVersion.ToString(),
            //            //License = new License
            //            //{
            //            //    name = "MIT",
            //            //    url = "https://mit-license.org/"
            //            //}
            //        });
            //    }

            //    //s.SwaggerDoc("v1", new OpenApiInfo
            //    //{
            //    //    Version = "v1",
            //    //    Title = "My API",
            //    //    Description = "API for SparkTodo",
            //    //});

            //    //Show the api version in url address
            //    s.DocInclusionPredicate((version, apiDescription) =>
            //    {
            //        //if (!version.Equals(apiDescription.GroupName))
            //        //    return false;

            //        var values = apiDescription.RelativePath
            //            .Split('/')
            //            .Select(v => v.Replace("v{version:apiVersion}", apiDescription.GroupName));

            //        apiDescription.RelativePath = string.Join("/Controllers/", values);
            //        //apiDescription.RelativePath = "/Controllers";
            //        return true;
            //    });

            //    //Remove version parameter
            //    s.OperationFilter<RemoveVersionFromParameter>();
            //    s.DocumentFilter<SetVersionInPathDocumentFilter>();

            //    //Add comments description
            //    //
            //    var basePath = Path.GetDirectoryName(AppContext.BaseDirectory);//get application located directory
            //    var apiPath = Path.Combine(basePath, "Music_store_backend.xml");
            //    //var dtoPath = Path.Combine(basePath, "Music_store_backend.Application.xml");
            //    s.IncludeXmlComments(apiPath, true);
            //    //s.IncludeXmlComments(dtoPath, true);

            //    //Add Jwt Authorize to http header
            //    // change new ApiKeyScheme ==> OpenApiSecurityScheme 
            //    s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            //    {
            //        Description = "Copy 'Bearer ' + valid JWT token into field",
            //        Name = "Authorization",//Jwt default param name
            //                               // replace above to following
            //                               //In = "header",//Jwt store address
            //                               //Type = "apiKey"//Security scheme type
            //        In = ParameterLocation.Header,
            //        Type = SecuritySchemeType.ApiKey,
            //    });
            //    //Add authentication type
            //    // replace following
            //    //s.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
            //    //{
            //    //    { "Bearer", new string[] { } }
            //    //});
            //    s.AddSecurityRequirement(new OpenApiSecurityRequirement
            //    {
            //        { new OpenApiSecurityScheme
            //        {
            //            Reference = new OpenApiReference()
            //            {
            //                Id = "Bearer",
            //                Type = ReferenceType.SecurityScheme
            //            }
            //        }, Array.Empty<string>() }
            //    });
            //});

            #endregion

            #region redis
            services.AddDistributedRedisCache(options =>
            {
                options.InstanceName = "wvCulHJ+yE6wTUoLoJ7Y6WdBTBxlVnY7Assnd8Q+Y3A=";
                options.Configuration = "testCacheforTry.redis.cache.windows.net:6380,password=wvCulHJ+yE6wTUoLoJ7Y6WdBTBxlVnY7Assnd8Q+Y3A=,ssl=True,abortConnect=False";
            });
            #endregion

            #region Others

            //services.AddAutoMapper();

            //services.AddDistributedRedisCache(r =>
            //{
            //    r.Configuration = Configuration["Redis:ConnectionString"];
            //});

            //DI Sql Data
            //services.AddTransient<IDataRepository, DataRepository>();
            // services.AddTransient<IDataRepository<T>, DataRepository>();
            //services.AddTransient<IDataAccess, DataAccess>();
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            //app.UseHsts();


            app.UseHttpsRedirection();

            //Enable Cors
            app.UseCors(_defaultCorsPolicyName);
            app.UseCors("WhiteListCorsPolicy");

            //Load Sql Data
            //app.UseDapper();

            //app.UseMvc();

            #region Enable Swagger  (Swashbuckle)

            // disable claimType transform, see details here https://stackoverflow.com/questions/39141310/jwttoken-claim-name-jwttokentypes-subject-resolved-to-claimtypes-nameidentifie
            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //JwtSecurityTokenHandler.DefaultOutboundAlgorithmMap.Clear();

            // app.UseSwagger(o =>
            // {
            // o.PreSerializeFilters.Add((document, request) =>
            //{
            //document.Paths = document.Paths.ToDictionary(p => p.Key.ToLowerInvariant(), p => p.Value);
            //});
            //});

            //app.UseSwagger();

            //app.UseSwaggerUI(s =>
            //{
            //Default to show the latest api docs
            //foreach (var description in provider.ApiVersionDescriptions.Reverse())
            //{
            //    s.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
            //        $" API {description.GroupName.ToUpperInvariant()}");
            //}
            //s.SwaggerEndpoint("/swagger/v2/swagger.json", "V2 Docs");
            //s.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
            //s.RoutePrefix = string.Empty;
            //s.DocumentTitle = "musicstore API";
            //});

            #endregion

            app.UseRouting();

            //Enable Authentication
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseOpenApi(); // serve OpenAPI/Swagger documents
            app.UseSwaggerUi3(); // serve Swagger UI
            app.UseReDoc(); // serve ReDoc UI
        }
    }
}
