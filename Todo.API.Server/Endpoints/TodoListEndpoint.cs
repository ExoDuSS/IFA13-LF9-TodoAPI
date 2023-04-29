using Microsoft.OpenApi.Models;
using TodoAPIServer.Endpoints;
using TodoAPIServer.Models;
using TodoAPIServer.Repositories;

namespace Todo.API.Server.Endpoints
{
    public class TodoListEndpoint : IEndpointDefinition
    {
        private static string TAG_LIST = "ToDo List";
        private static string TAG_ENTRY = "ToDo List Entry";

        private static string BASE_ROUTE_LIST = "/todo-list";
        private static string ROUTE_LIST = $"{BASE_ROUTE_LIST}/{{listId:guid}}";
        private static string BASE_ROUTE_ENTRY = $"{ROUTE_LIST}/entry";
        private static string ROUTE_ENTRY = $"{BASE_ROUTE_ENTRY}/{{entryId:guid}}";

        public void DefineEndpoints(WebApplication app)
        {
            // todo list
            app.MapPost(BASE_ROUTE_LIST, CreateTodoList)
                .WithName("CreateTodoList")
                .WithSummary("Create Todo List")
                .WithDescription("Creates a new todo list")
                .WithTags(new[] { TAG_LIST })
                .WithOpenApi(op =>
                {
                    op.Responses["200"] = new OpenApiResponse()
                    {
                        Description = "OK",
                        Content = new Dictionary<string, OpenApiMediaType>()
                        {
                            { "application/json", new OpenApiMediaType() { Schema = new OpenApiSchema() { Type = "object"}}}
                        }
                    };

                    return op;
                });

            app.MapGet(BASE_ROUTE_LIST, GetAllTodoLists)
                .WithName("GetAllTodoLists")
                .WithDescription("Returns all todo lists")
                .WithTags(new[] { TAG_LIST })
                .WithOpenApi();

            app.MapGet(ROUTE_LIST, GetAllTodoListEntries)
                .WithName("GetAllTodoListEntries")
                .WithDescription("Returns all list entries of a todo list")
                .WithTags(new[] { TAG_LIST })
                .WithOpenApi();

            app.MapDelete(ROUTE_LIST, DeleteTodoList)
                .WithName("DeleteTodoListWithAllEntries")
                .WithDescription("Removes a todo list with all entries")
                .WithTags(new[] { TAG_LIST })
                .WithOpenApi();

            // todos
            app.MapPost(ROUTE_LIST, CreateTodoEntry)
                .WithName("CreateTodoListEntry")
                .WithDescription("Adds an entry to an exisiting list")
                .WithTags(new[] { TAG_ENTRY })
                .WithOpenApi();

            app.MapGet(ROUTE_ENTRY, GetTodoEntry)
                .WithName("GetTodoListEntry")
                .WithDescription("Returns a single entry from a list")
                .WithTags(new[] { TAG_ENTRY })
                .WithOpenApi();

            app.MapPut(ROUTE_ENTRY, UpdateTodoEntry)
                .WithName("UpdateTodoListEntry")
                .WithDescription("Updates an existing entry")
                .WithTags(new[] { TAG_ENTRY })
                .WithOpenApi();

            app.MapDelete(ROUTE_ENTRY, RemoveTodoEntry)
                .WithName("RemoveTodoEntryFromTodoList")
                .WithDescription("Removes an entry from a given list")
                .WithTags(new[] { TAG_ENTRY })
                .WithOpenApi();
        }

        public void DefineServices(IServiceCollection services)
        {
            services.AddSingleton<ITodoEntryRepository, TodoEntryRepository>();
            services.AddSingleton<ITodoListRepository, TodoListRepository>();
        }

        #region TodoList

        internal IResult GetAllTodoLists(ITodoListRepository repo)
        {
            return Results.Ok(repo.GetAll());
        }

        internal IResult CreateTodoList(ITodoListRepository repo, string name)
        {
            var list = repo.Create(name);

            if (list is null)
                return Results.BadRequest();
            return Results.Ok(list);
        }

        internal IResult GetAllTodoListEntries(ITodoListRepository repo, Guid listId)
        {
            if (repo.GetById(listId) is null)
                return Results.NotFound();

            return Results.Ok(repo.GetAll(listId));
        }

        internal IResult DeleteTodoList(ITodoListRepository repo, Guid listId)
        {
            if (repo.GetById(listId) is null)
                return Results.NotFound();

            repo.Remove(listId);

            return Results.Ok();
        }

        #endregion

        #region TodoEntry

        internal IResult CreateTodoEntry(ITodoListRepository repoList, ITodoEntryRepository repoEntry, Guid listId, TodoEntry entry)
        {
            if (repoList.GetById(listId) is null)
                return Results.NotFound();

            if (entry.Id is not null)
                return Results.BadRequest();

            entry.ListId = listId;

            return Results.Ok(repoEntry.Create(entry));
        }

        internal IResult GetTodoEntry(ITodoListRepository repoList, ITodoEntryRepository repoEntry, Guid listId, Guid entryId)
        {
            if (repoList.GetById(listId) is null)
                return Results.NotFound();

            var entry = repoEntry.GetById(listId);
            if (entry is null)
                return Results.NotFound();
            return Results.Ok(entry);
        }

        internal IResult UpdateTodoEntry(ITodoListRepository repoList, ITodoEntryRepository repoEntry, Guid listId, Guid entryId, TodoEntry entry)
        {
            if (repoList.GetById(listId) is null)
                return Results.NotFound();

            if (repoEntry.GetById(entryId) is null)
                return Results.NotFound();

            if (!entry.ListId.HasValue || repoList.GetById(entry.ListId.Value) is null)
                return Results.BadRequest();

            if (!entry.Id.HasValue || repoEntry.GetById(entry.Id.Value) is null)
                return Results.BadRequest();

            return Results.Ok(repoEntry.Update(entry));
        }

        internal IResult RemoveTodoEntry(ITodoListRepository repoList, ITodoEntryRepository repoEntry, Guid listId, Guid entryId)
        {
            if (repoList.GetById(listId) is null)
                return Results.NotFound();

            if (repoEntry.GetById(entryId) is null)
                return Results.NotFound();

            repoEntry.Remove(entryId);
            return Results.Ok();
        }

        #endregion
    }
}
