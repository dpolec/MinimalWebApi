# Minimal WebApi
How much do we need to run API?<br />
Only one file (Program.cs) with 3 line of code

```
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.Run();
```

With this, we don't received anything, but is running.

![image](https://user-images.githubusercontent.com/11536139/228445349-ce82a334-9881-412b-914a-c834f71dcb46.png)

It is worth noting that we do not have a namespace and a declared class here.

Usually we using Swagger so let's do that, install package Swashbuckle.AspNetCore.

We need add more code, declare configuration (for now not needed) and service, next we need add endpoint explorer, configure Swagger, and told our APP to use Swagger.

```
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(conf =>
{
    conf.CustomSchemaIds(_ => _.FullName);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
```

We can setup in Properties/launchSettings.json to open browser and open Swagger UI
```
"MinimalWebApi": {
  "commandName": "Project",
  "dotnetRunMessages": true,
  "launchBrowser": true,
  "launchUrl": "swagger",
  "applicationUrl": "https://localhost:7245;http://localhost:5235",
  "environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development"
  }
}

```
Run

![image](https://user-images.githubusercontent.com/11536139/228445504-e407d55e-732a-472c-8c6c-b7268846c36c.png)

Great, but still we don't have any endpoints, so lets add them, we will adding whitout standard approach with controller file, we will define endpoint in the same place.
So affter builder.Build() lets add first endpoint.
```
app.MapGet("random-values/{id}", (int id) => new { Id = id, Values = "some value" });
```
Here we have get, with route, and input id, when we run, we will have this endpoint, and will be works.<br />
But we will have anonymous schemas, so change this.<br />
To change this we need create some record, and used it.
```
app.MapGet("random-values/{id}", (int id) => new RandomValue(id, "some value"));
```
![image](https://user-images.githubusercontent.com/11536139/228357585-3a1afafa-d2b9-4f55-8f09-68ff5cf4c45e.png)

Now looks better.

Ok, add more endpoint, for this RandomValue, and add something more, for example IdName.
```
app.MapGet("random-values/", () =>
{
    var result = new List<RandomValue>();
    for (var i = 0; i < 10; i++)
    {
        result.Add(new RandomValue(i, "some value"));
    }
    return result;
});
app.MapGet("random-values/{id}", (int id) => new IdName(id, "some value"));

app.MapGet("id-name/", () =>
{
    var result = new List<RandomValue>();
    for (var i = 0; i < 10; i++)
    {
        result.Add(new RandomValue(i, "Name"));
    }
    return result;
});
app.MapGet("id-name/{id}", (int id) => new RandomValue(id, "Name"));
```
In Swagger we can see that we don't have grouped this.

![image](https://user-images.githubusercontent.com/11536139/228445670-a7418153-5f21-4381-86a1-3382978012eb.png)

Lets fixed this.<br />
We need add .WithTags("") after all map.
```
app.MapGet("random-values/", () =>
{
    var result = new List<RandomValue>();
    for (var i = 0; i < 10; i++)
    {
        result.Add(new RandomValue(i, "some value"));
    }
    return result;
}).WithTags("random-values");
app.MapGet("random-values/{id}", (int id) => new IdName(id, "some value")).WithTags("random-values");

app.MapGet("id-name/", () =>
{
    var result = new List<RandomValue>();
    for (var i = 0; i < 10; i++)
    {
        result.Add(new RandomValue(i, "Name"));
    }
    return result;
}).WithTags("id-name");
app.MapGet("id-name/{id}", (int id) => new RandomValue(id, "Name")).WithTags("id-name");
```
![image](https://user-images.githubusercontent.com/11536139/228358562-66904dbb-d6ef-4862-96f7-501faa14455e.png)

Now looks better.<br />
For now we added Swagger, and add few endpoints, but what is wrong?<br />
We are break DRY rule, and we repeated code in few place, but first I will show how to used DI, and next we will back to this.<br />
I will add repository and register them as singleton. We will used simple list in each to store some data, and add more endpoint, to generate all CRUD operation. <br />
To inject some object in this approach we need just add this object what we need to delegete.
```
app.MapGet("random-values/", (IRandomValueRepo repo) => repo.Get()).WithTags("random-values");
app.MapGet("random-values/{id}", (int id, IRandomValueRepo repo) => repo.Get(id)).WithTags("random-values");
app.MapDelete("random-values/{id}", (int id, IRandomValueRepo repo) => repo.Delete(id)).WithTags("random-values");
app.MapPost("random-values/", (RandomValue input, IRandomValueRepo repo) => repo.Add(input)).WithTags("random-values");
app.MapPut("random-values/", (RandomValue input, IRandomValueRepo repo) => repo.Update(input)).WithTags("random-values");

app.MapGet("id-name/", (IIdNameRepo repo) => repo.Get()).WithTags("id-name");
app.MapGet("id-name/{id}", (int id, IIdNameRepo repo) => repo.Get(id)).WithTags("id-name");
app.MapDelete("id-name/{id}", (int id, IIdNameRepo repo) => repo.Delete(id)).WithTags("id-name");
app.MapPost("id-name/", (IdName input, IIdNameRepo repo) => repo.Add(input)).WithTags("id-name");
app.MapPut("id-name/", (IdName input, IIdNameRepo repo) => repo.Update(input)).WithTags("id-name");
```
Works!!!

Lets back to DRY, still we repeted name of functionality, of course the easiest way will be declate some variable and used this. I will show how create builder for this.<br />
Create new file ControllerRoute, in here we are implement route mapping into WebApplication.
```
public class ControllerRoute
{
    private readonly WebApplication _app;
    private readonly string _rootName;

    public ControllerRoute(
        WebApplication app,
        string rootName,
        Action<ControllerRoute> routeActions)
    {
        _app = app;
        _rootName = rootName.TrimEnd('/');

        routeActions(this);
    }

    public IEndpointConventionBuilder MapDelete(Delegate requestDelegate) => MapDelete(string.Empty, requestDelegate);

    public IEndpointConventionBuilder MapDelete(string pattern, Delegate requestDelegate) =>
        _app.MapDelete(Path(pattern), requestDelegate).WithTags(_rootName);

    public IEndpointConventionBuilder MapGet(Delegate requestDelegate) => MapGet(string.Empty, requestDelegate);

    public IEndpointConventionBuilder MapGet(string pattern, Delegate requestDelegate) =>
        _app.MapGet(Path(pattern), requestDelegate).WithTags(_rootName);

    public IEndpointConventionBuilder MapPost(Delegate requestDelegate) => MapPost(string.Empty, requestDelegate);

    public IEndpointConventionBuilder MapPost(string pattern, Delegate requestDelegate) =>
        _app.MapPost(Path(pattern), requestDelegate).WithTags(_rootName);

    public IEndpointConventionBuilder MapPut(Delegate requestDelegate) => MapPut(string.Empty, requestDelegate);

    public IEndpointConventionBuilder MapPut(string pattern, Delegate requestDelegate) =>
        _app.MapPut(Path(pattern), requestDelegate).WithTags(_rootName);

    private string Path(string pattern) => $"{_rootName}/{pattern.TrimStart('/')}";
}
```
Now implement what we created, but first move route declaration from main file.<br />
Create file Extensions/WebApplicationBuilderExtension, and implement our builder
```
public static class WebApplicationBuilderExtension
{
    public static void MapIdName(this WebApplication app)
    {
        MapController(
            app,
            "id-name",
            route =>
            {
                route.MapGet((IIdNameRepo repo) => repo.Get());
                route.MapGet("{id}", (int id, IIdNameRepo repo) => repo.Get(id));
                route.MapDelete("{id}", (int id, IIdNameRepo repo) => repo.Delete(id));
                route.MapPost((IdName input, IIdNameRepo repo) => repo.Add(input));
                route.MapPut((IdName input, IIdNameRepo repo) => repo.Update(input));
            });
    }

    public static void MapRandomValues(this WebApplication app)
    {
        MapController(
            app,
            "random-values",
            route =>
            {
                route.MapGet((IRandomValueRepo repo) => repo.Get());
                route.MapGet("{id}", (int id, IRandomValueRepo repo) => repo.Get(id));
                route.MapDelete("{id}", (int id, IRandomValueRepo repo) => repo.Delete(id));
                route.MapPost((RandomValue input, IRandomValueRepo repo) => repo.Add(input));
                route.MapPut((RandomValue input, IRandomValueRepo repo) => repo.Update(input));
            });
    }

    private static void MapController(WebApplication app, string rootName, Action<ControllerRoute> routeActions) =>
        new ControllerRoute(app, rootName, routeActions);
}
```
That in main file we received cleaner and more readable code.
```
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

services.AddSingleton<IIdNameRepo, IdNameRepo>();
services.AddSingleton<IRandomValueRepo, RandomValueRepo>();

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(conf =>
{
    conf.CustomSchemaIds(_ => _.FullName);
});

var app = builder.Build();

app.MapIdName();
app.MapRandomValues();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
```
Generally that will be all.<br />
I worked in project where we have lots of repositories, where we have few API, with common setup, in each we have autorization, swagger, logs, etc. <br />
To have one place to maitanance this all we can create small helper, so lets create file WebApiHelper.
```
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
```
We can find here argument with name sectionName, I don't used this now, but in my solution I creted basic configuration, and base on this, adding some functionality.<br />
Lets look now on our startup app.
```
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
```
Simple, small, and enought to run and work.
