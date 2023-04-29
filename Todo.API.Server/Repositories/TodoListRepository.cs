using TodoAPIServer.Models;

namespace TodoAPIServer.Repositories;

public class TodoListRepository : ITodoListRepository
{
    private readonly ITodoEntryRepository fTodoEntryRepository;

    private readonly Dictionary<Guid, TodoList> fTodoLists;

    public TodoListRepository(ITodoEntryRepository todoEntryRepository)
    {
        fTodoEntryRepository = todoEntryRepository;
        fTodoLists = new();
    }

    public TodoList? Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var list = new TodoList() { Name = name };

        fTodoLists[list.Id] = list;

        return list;
    }

    public TodoList? GetById(Guid id)
    {
        fTodoLists.TryGetValue(id, out var list);

        if (list is null)
            return null;

        list.Entries.Clear();
        list.Entries = fTodoEntryRepository.GetAll().ToList();

        return fTodoLists[id];
    }

    public IEnumerable<TodoList> GetAll()
    {
        return fTodoLists.Values;
    }

    public IEnumerable<TodoList> GetAll(Guid id)
    {
        return fTodoLists.Values.Where(e => e.Id == id);
    }

    public TodoList? Update(TodoList todoList)
    {
        var existingList = GetById(todoList.Id);
        if (existingList is null)
            return null;
        fTodoLists[todoList.Id] = todoList;
        return todoList;
    }

    public void Remove(Guid id)
    {
        foreach (var todo in fTodoLists.Values)
            fTodoEntryRepository.Remove(todo.Id);
        fTodoLists.Remove(id);
    }
}
