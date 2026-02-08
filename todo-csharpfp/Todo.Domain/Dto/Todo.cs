using CSharpFunctionalExtensions;
using static CSharpFunctionalExtensions.Result;

namespace Todo.Domain.Dto;

using Domain = Objs;

public record Todo(string? Id, string Title, bool Completed, int? Order);

public static class TodoExtensions
{
  public static Result<Domain.Todo, string> Validate(this Todo todo, string? id = null) => 
    Domain.Todo.From(todo, id);
}