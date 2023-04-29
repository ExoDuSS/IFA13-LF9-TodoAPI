using Microsoft.OpenApi.Models;
using TodoAPIServer.Endpoints;
using TodoAPIServer.Models;
using TodoAPIServer.Repositories;

namespace Todo.API.Server.Endpoints
{
    public class TodoListEndpoint : IEndpointDefinition
    {
        private string TAG_LIST = "ToDo List";
        private string TAG_ENTRY = "ToDo List Entry";
        private string ENDPOINT = "/todo-list";
        private string LIST_ENDPOINT = "/todo-list/{listId:guid}";
        private string ENTRY_ENDPOINT = "/todo-list/{listid:guid}/entry/{entryId:guid}";

        public void DefineEndpoints(WebApplication app)
        {
            // todo list
            app.MapPost(ENDPOINT, CreateTodoList)
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

            app.MapGet(ENDPOINT, GetAllTodoLists)
                .WithName("GetAllTodoLists")
                .WithDescription("Returns all todo lists")
                .WithTags(new[] { TAG_LIST })
                .WithOpenApi();

            app.MapGet(LIST_ENDPOINT, GetAllTodoListEntries)
                .WithName("GetAllTodoListEntries")
                .WithDescription("Returns all list entries of a todo list")
                .WithTags(new[] { TAG_LIST })
                .WithOpenApi();

            app.MapDelete(LIST_ENDPOINT, DeleteTodoList)
                .WithName("DeleteTodoListWithAllEntries")
                .WithDescription("Removes a todo list with all entries")
                .WithTags(new[] { TAG_LIST })
                .WithOpenApi();

            // todos
            app.MapPost(LIST_ENDPOINT, CreateTodoEntry)
                .WithName("CreateTodoListEntry")
                .WithDescription("Adds an entry to an exisiting list")
                .WithTags(new[] { TAG_ENTRY })
                .WithOpenApi();

            app.MapGet(ENTRY_ENDPOINT, GetTodoEntry)
                .WithName("GetTodoListEntry")
                .WithDescription("Returns a single entry from a list")
                .WithTags(new[] { TAG_ENTRY })
                .WithOpenApi();

            app.MapPut(ENTRY_ENDPOINT, UpdateTodoEntry)
                .WithName("UpdateTodoListEntry")
                .WithDescription("Updates an existing entry")
                .WithTags(new[] { TAG_ENTRY })
                .WithOpenApi();

            app.MapDelete(ENTRY_ENDPOINT, RemoveTodoEntry)
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
