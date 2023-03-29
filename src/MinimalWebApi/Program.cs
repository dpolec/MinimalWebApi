using MinimalWebApi.Extensions;
using MinimalWebApi.Helpers;
using MinimalWebApi.Interfaces;
using MinimalWebApi.Repositories;

WebApiHelper.Run(
    args,
    (webAppBuilder, services, configuration) =>
    {
        services.AddSingleton<IIdNameRepo, IdNameRepo>();
        services.AddSingleton<IRandomValueRepo, RandomValueRepo>();
    },
    (app) =>
    {
        app.MapIdName();
        app.MapRandomValues();
    });