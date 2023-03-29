namespace MinimalWebApi.Helpers
{
    public static class WebApiHelper
    {
        public static void Run(
        string[] args,
        Action<WebApplicationBuilder, IServiceCollection, ConfigurationManager> configureWebAppBuilder = null,
        Action<WebApplication> configureWebApp = null,
        string sectionName = "WebApi")
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
            var services = builder.Services;

            if (configureWebAppBuilder is not null)
            {
                configureWebAppBuilder(builder, services, configuration);
            }

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(conf =>
            {
                conf.CustomSchemaIds(_ => _.FullName);
            });

            var app = builder.Build();

            if (configureWebApp is not null)
            {
                configureWebApp(app);
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            app.Run();
        }
    }
}