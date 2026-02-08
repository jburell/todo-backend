using CSharpFunctionalExtensions;
using static CSharpFunctionalExtensions.Result;

namespace Todo.Domain.Objs;

public record Todo
{
  public Guid Id { get; }
  public string Title { get; }
  public bool Completed { get; }
  public int? Order { get; }

  private Todo(Guid id, string title, bool completed, int? order)
  {
    Id = id;
    Title = title;
    Completed = completed;
    Order = order;
  }
  
  public static Result<Todo, string> From(Dto.Todo todo, string? id = null) =>
    Success<Dto.Todo, string>(todo with { Id = id ?? todo.Id })
      .Bind(IdFromTodo_Else_GenerateNewId)
      .Map(guid => new Todo(
        guid,
        todo.Title,
        todo.Completed,
        todo.Order));
  
  private static Result<Guid, string> IdFromTodo_Else_GenerateNewId(Dto.Todo todo) =>
    todo.Id == null
      ? Success<Guid, string>(Guid.NewGuid())
      : SuccessIf<Guid, string>(Guid.TryParse(todo.Id, out var result), result, "Invalid ID format");
}