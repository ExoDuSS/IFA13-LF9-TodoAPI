using TodoAPIServer.Models;

namespace TodoAPIServer.Repositories;

public interface ITodoEntryRepository
{
    public TodoEntry? Create(TodoEntry todo);
    public TodoEntry? GetById(Guid id);
    public IEnumerable<TodoEntry> GetAll();
    public IEnumerable<TodoEntry> GetAll(Guid id);
    public TodoEntry? Update(TodoEntry todo);
    public void Remove(Guid id);
}
