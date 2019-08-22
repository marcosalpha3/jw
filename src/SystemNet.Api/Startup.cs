using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using SystemNet.Business.Services;
using SystemNet.Core.Domain.Contracts.Repositories;
using SystemNet.Core.Domain.Contracts.Services;
using SystemNet.Core.Domain.Models;
using SystemNet.Core.Infraestructure;
using SystemNet.Core.Infraestructure.Repositories;
using SystemNet.Practices.Data.Storage;
using SystemNet.Practices.Data.Storage.Models;
using SystemNet.Practices.Security.Bearer;
using SystemNet.Shared;



namespace SystemNet.Api
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

            Configuration = configurationBuilder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {

            ConfigureToken(ref services);

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
            services.AddResponseCompression();

            services.AddResponseCaching();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc();

            ConfigureAzureStorage(ref services);

            services.AddCors();

            ConfigureDependencies(ref services);

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "JW Api", Version = "v1" });
            });
       }


        private void ConfigureDependencies(ref IServiceCollection services)
        {
            services.AddScoped<AppDataContext, AppDataContext>();

            services.AddTransient<IQuadroRepository, QuadroRepository>();
            services.AddTransient<IQuadroServices, QuadroServices>();
            services.AddTransient<Quadro, Quadro>();

            services.AddTransient<ICongregacaoRepository, CongregacaoRepository>();
            services.AddTransient<Congregacao, Congregacao>();

            services.AddTransient<IControleListaRepository, ControleListaRepository>();
            services.AddTransient<ControleLista, ControleLista>();

            services.AddTransient<IDataEventoRepository, DataEventoRepository>();
            services.AddTransient<IDataEventoServices, DataEventoServices>();
            services.AddTransient<DataEvento, DataEvento>();

            services.AddTransient<IIrmaoRepository, IrmaoRepository>();
            services.AddTransient<IIrmaoServices, IrmaoServices>();
            services.AddTransient<Irmao, Irmao>();

            services.AddTransient<IQuadroDetalheRepository, QuadroDetalheRepository>();
            services.AddTransient<QuadroDetalhe, QuadroDetalhe>();

            services.AddTransient<ITipoListaRepository, TipoListaRepository>();
            services.AddTransient<TipoLista, TipoLista>();

            services.AddTransient<ICongregacaoRepository, CongregacaoRepository>();
            services.AddTransient<ICongregacaoServices, CongregacaoServices>();
            services.AddTransient<Congregacao, Congregacao>();

            services.AddTransient<IGrupoRepository, GrupoRepository>();
            services.AddTransient<IGrupoServices, GrupoServices>();
            services.AddTransient<Grupo, Grupo>();

            services.AddTransient<IAssistenciaReuniaoRepository, AssistenciaReuniaoRepository>();
            services.AddTransient<IAssistenciaReuniaoServices, AssistenciaReuniaoServices>();
            services.AddTransient<AssistenciaReuniao, AssistenciaReuniao>();

            services.AddTransient<StorageHelper, StorageHelper>();
            services.AddTransient<StorageConfig, StorageConfig>();            
        }

        private void ConfigureAzureStorage(ref IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<StorageConfig>(Configuration.GetSection("StorageConfig"));
            services.Configure<FormOptions>(options =>
            {
                options.MemoryBufferThreshold = Int32.MaxValue;
            });

        }

        /// <summary>
        /// his method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(x =>
            {
                x.AllowAnyHeader();
                x.AllowAnyMethod();
                x.AllowAnyOrigin();
            });

            app.UseResponseCaching();
            app.UseResponseCompression();

            //Localization

            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en-US"),
                new CultureInfo("es-ES"),
                new CultureInfo("pt-BR")
            };

            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
            };

            app.UseRequestLocalization(options);


            LoadSettings();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "JW API");
            });


        }

        private void LoadSettings()
        {
            Runtime.connectionStrings = new string[1, 2] { { "JW", Configuration.GetConnectionString("JW") } };

            Runtime.DataPrimeiraReuniao = Configuration["AppSettings:URLSystemNet"];
            Runtime.UrlSystem = Configuration["AppSettings:UrlSystem"]; 
            Runtime.Smtp = Configuration["EmailSettings:smtp"];
            Runtime.Port = Configuration["EmailSettings:port"];
            Runtime.User = Configuration["EmailSettings:user"];
            Runtime.Sender = Configuration["EmailSettings:sender"];
            Runtime.Ssl = Configuration["EmailSettings:ssl"];
            Runtime.TimeOutClientSmtp = Configuration["EmailSettings:timeOutClientSmtp"];
            Runtime.KeySendGrid = Configuration["EmailSettings:KeySendGrid"];
            Runtime.NameSystem = Configuration["AppSettings:NameSystem"];
        }


        private void ConfigureToken(ref IServiceCollection services)
        {

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                                .AddJwtBearer(options =>
                                {
                                    options.TokenValidationParameters =
                                         new TokenValidationParameters
                                         {
                                             ValidateIssuer = true,
                                             ValidateAudience = true,
                                             ValidateLifetime = true,
                                             ValidateIssuerSigningKey = true,

                                             ValidIssuer = "SystemNet.Security",
                                             ValidAudience = "SystemNet.Security",
                                             IssuerSigningKey = JwtSecurityKey.Create(Runtime.SecreteKey)
                                         };

                                    options.Events = new JwtBearerEvents
                                    {
                                        OnAuthenticationFailed = context =>
                                        {
                                            Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);
                                            return Task.CompletedTask;
                                        },
                                        OnTokenValidated = context =>
                                        {
                                            Console.WriteLine("OnTokenValidated: " + context.SecurityToken);
                                            return Task.CompletedTask;
                                        }
                                    };



                                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Member",
                    policy => policy.RequireClaim("MembershipId"));
                options.AddPolicy("Brother",
                    policy => policy.RequireClaim("Brother"));
                options.AddPolicy("Assistance",
                    policy => policy.RequireClaim("Assistance"));
                options.AddPolicy("LoadBoard",
                    policy => policy.RequireClaim("LoadBoard"));

            });



        }

        /// <summary>
        /// Add the Suagger on the API
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureSwagger(ref IServiceCollection services)
        {
            // Configurando o serviço de documentação do Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new Info
                    {
                        Title = "Access control for the Ortiz Portal",
                        Version = "1.0.0",
                        Description = "Create or update users, generate passwords and tokens"
                    });

                var filePath =
                    PlatformServices.Default.Application.ApplicationBasePath;
                var applicationName =
                    PlatformServices.Default.Application.ApplicationName;
                var XmlDocPath =
                    Path.Combine(filePath, $"{applicationName}.xml");

                c.IncludeXmlComments(XmlDocPath);
            });
        }

    }
}
