using MinimalWebApi.Builders;
using MinimalWebApi.Interfaces;
using MinimalWebApi.Models;

namespace MinimalWebApi.Extensions
{
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
}