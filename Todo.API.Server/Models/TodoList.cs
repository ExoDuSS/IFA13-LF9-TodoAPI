namespace TodoAPIServer.Models;

public class TodoList
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public List<TodoEntry> Entries { get; set; } = new();
}
