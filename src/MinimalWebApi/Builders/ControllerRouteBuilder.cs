namespace MinimalWebApi.Builders
{
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
            _rootName = rootName;

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

        private string Path(string pattern) => $"{_rootName}/{pattern}";
    }
}