using TodoAPIServer.Models;

namespace TodoAPIServer.Repositories;

public class TodoEntryRepository : ITodoEntryRepository
{
    private readonly Dictionary<Guid, TodoEntry> fTodoEntries;

    public TodoEntryRepository()
    {
        fTodoEntries = new();
    }

    public TodoEntry? Create(TodoEntry todo)
    {
        if (todo is null)
            return null;

        todo.Id = Guid.NewGuid();

        fTodoEntries[todo.Id.Value] = todo;
        return todo;
    }

    public TodoEntry? GetById(Guid id)
    {
        fTodoEntries.TryGetValue(id, out var todo);
        return todo;
    }

    public IEnumerable<TodoEntry> GetAll()
    {
        return fTodoEntries.Values;
    }

    public IEnumerable<TodoEntry> GetAll(Guid id)
    {
        return fTodoEntries.Values.Where(e => e.ListId == id);
    }

    public TodoEntry? Update(TodoEntry todo)
    {
        var existingTodo = GetById(todo.Id.Value);
        if (existingTodo is null)
            return null;
        fTodoEntries[todo.Id.Value] = todo;
        return todo;
    }

    public void Remove(Guid id)
    {
        fTodoEntries.Remove(id);
    }
}
