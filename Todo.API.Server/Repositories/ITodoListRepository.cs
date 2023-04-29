using TodoAPIServer.Models;

namespace TodoAPIServer.Repositories;

public interface ITodoListRepository
{
    public TodoList? Create(string name);
    public TodoList? GetById(Guid id);
    public IEnumerable<TodoList> GetAll();
    public IEnumerable<TodoList> GetAll(Guid id);
    public TodoList? Update(TodoList todoList);
    public void Remove(Guid id);
}
