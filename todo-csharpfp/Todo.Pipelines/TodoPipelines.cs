using Todo.Pipelines.Dto;

namespace Todo.Pipelines;

using CSharpFunctionalExtensions;
using static CSharpFunctionalExtensions.Result; 
using Stores;
using Domain.Objs;

public class TodoPipelines(ITodoStore store) 
{
  public Result<Todo[], string> GetAll() => store.GetAll();

  public Result<Maybe<Todo>, string> Get(string id) =>
    SuccessIf<Guid, string>(Guid.TryParse(id, out var result), result, "Invalid ID format")
      .Bind(store.Get);

  public Result<Todo, string> AddTodo(Dto.Todo todo) =>
    todo
      .Validate()
      .Bind(store.AddTodo);

  public Result<Maybe<Todo>, string> UpdateTodo(string id, Dto.Todo todo) =>
    todo
      .Validate()
      .Bind(store.Update);

  public UnitResult<string> Delete(string id) => 
    SuccessIf<Guid, string>(Guid.TryParse(id, out var result), result, "Invalid ID format")
      .Bind(store.Delete)
      .Map(_ => Success<string>());

  public UnitResult<string> DeleteAll() => store.DeleteAll();
}