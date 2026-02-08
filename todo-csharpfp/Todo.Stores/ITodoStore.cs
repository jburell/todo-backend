using CSharpFunctionalExtensions;

namespace Todo.Stores;
using Domain.Objs;

public interface ITodoStore
{
  public Result<Todo[], string> GetAll();
  public Result<Maybe<Todo>, string> Get(Guid id);
  public Result<Todo, string> AddTodo(Todo todo);
  public Result<Maybe<Todo>, string> Update(Todo todo);
  public UnitResult<string> DeleteAll();
  public Result<bool, string> Delete(Todo todo);
}