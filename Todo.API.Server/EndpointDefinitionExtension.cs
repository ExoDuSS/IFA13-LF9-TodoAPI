using TodoAPIServer.Endpoints;

namespace TodoAPIServer
{
    public static class EndpointDefinitionExtension
    {
        public static void AddEndpointDefinitions(this IServiceCollection services, params Type[] scanMarkers)
        {
            var endpointDefinitions = new List<IEndpointDefinition>();

            foreach (var marker in scanMarkers)
            {
                endpointDefinitions.AddRange(
                        marker.Assembly.ExportedTypes
                            .Where(t => typeof(IEndpointDefinition).IsAssignableFrom(t) && !t.IsAbstract)
                            .Select(Activator.CreateInstance).Cast<IEndpointDefinition>()
                        );
            }

            foreach (var endpointDefinition in endpointDefinitions)
                endpointDefinition.DefineServices(services);

            services.AddSingleton(endpointDefinitions as IReadOnlyCollection<IEndpointDefinition>);
        }

        public static void UseEndpointDefinitions(this WebApplication app)
        {
            var definitions = app.Services.GetRequiredService<IReadOnlyCollection<IEndpointDefinition>>();

            foreach (var endpointDefinition in definitions)
                endpointDefinition.DefineEndpoints(app);
        }
    }
}
