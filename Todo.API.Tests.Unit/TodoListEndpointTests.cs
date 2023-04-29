using NSubstitute;
using TodoAPIServer.Repositories;
using Xunit;

namespace Todo.API.Tests.Unit;

public class TodoListEndpointTests
{
    private readonly ITodoEntryRepository fTodoEntryRepository =
        Substitute.For<ITodoEntryRepository>();

    private readonly ITodoListRepository fTodoListRepository =
        Substitute.For<ITodoListRepository>();

    [Fact]
    public void GetTodoListById_ReturnTodoList_WhenTodoListExists()
    {

    }
}
