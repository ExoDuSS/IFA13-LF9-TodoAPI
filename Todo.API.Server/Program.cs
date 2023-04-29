namespace TodoAPIServer;

public class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
            {
                Title = "IFA-13 Todo List API",
                Description = "A simple in memory todo list api supporting simple crud operations for todo list and its entries.",
                TermsOfService = new Uri("http://tos.sample.com"),
                Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                {
                    Name = "API Support",
                    Url = new Uri("http://www.example.com"),
                    Email = "example@example.com"
                },
                License = new Microsoft.OpenApi.Models.OpenApiLicense()
                {
                    Name = "Apache 2.0",
                    Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0.html")
                },
                Version = "v1.0"
            });
        });

        builder.Services.AddEndpointDefinitions(typeof(Program));

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI(setup =>
            {
                setup.SwaggerEndpoint("v1/swagger.json", "TodoAPI v1");
            });
        }
        else
        {
            app.UseExceptionHandler();
        }

        app.UseHttpsRedirection();

        app.UseEndpointDefinitions();

        app.Run();
    }
}
