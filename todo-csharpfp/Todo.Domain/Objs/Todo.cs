using CSharpFunctionalExtensions;

namespace Todo.Domain.Objs;

public record Todo
{
  public Guid Id { get; }
  public Maybe<string> Title { get; }
  public bool Completed { get; }
  public Maybe<int> Order { get; }

  private Todo(Guid id, ValidTitle title, bool completed, ValidOrder order)
  {
    Id = id;
    Title = title.Title;
    Completed = completed;
    Order = order.Order;
  }
  
  public static Result<Todo, string> From(Guid id, ValidTitle title, bool completed, ValidOrder order) =>
    new Todo(
      id,
      title,
      completed,
      order);
}