using CSharpFunctionalExtensions;
using static CSharpFunctionalExtensions.Result;

namespace Todo.Pipelines.Dto;

using Domain = Domain.Objs;

public record Todo(string? Id, string Title, bool Completed, int? Order);

public static class TodoExtensions
{
  public static Result<Domain.Todo, string> Validate(this Todo todo, string? id = null)
  {
    var validId = Success<Todo, string>(todo with { Id = id ?? todo.Id })
      .Bind(IdFromTodo_Else_GenerateNewId);
    var validTitle = Domain.ValidTitle.Create(todo.Title);
    var validOrder = Domain.ValidOrder.Create(todo.Order);
    
    return validId.IsSuccess && validTitle.IsSuccess && validOrder.IsSuccess
      ? Domain.Todo.From(validId.Value, validTitle.Value, todo.Completed, validOrder.Value)
      : "Could not create Todo domain object"; // TODO: Add error summary with applicatives
  }

  private static Result<Guid, string> IdFromTodo_Else_GenerateNewId(Dto.Todo todo) =>
    todo.Id == null
      ? Success<Guid, string>(Guid.NewGuid())
      : SuccessIf<Guid, string>(Guid.TryParse(todo.Id, out var result), result, "Invalid ID format");
}