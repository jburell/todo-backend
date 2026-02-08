namespace Todo.Database;

using CSharpFunctionalExtensions;
using Stores;
using Domain.Objs;
using static CSharpFunctionalExtensions.Result;

public class TodoDatabase : ITodoStore
{
  private readonly List<Todo> _todos = [];

  public Result<Todo[], string> GetAll() => Success<Todo[], string>(_todos.ToArray());

  public Result<Maybe<Todo>, string> Get(Guid id) =>
    Try(() => Maybe.From(_todos.Single(x => x.Id == id)), ex => ex)
      .Compensate(ex => ex switch
      {
        InvalidOperationException _ => Success<Maybe<Todo>, string>(Maybe.None),
        _ => Failure<Maybe<Todo>, string>(ex.Message)
      });

  public Result<Todo, string> AddTodo(Todo todo)
  {
    _todos.Add(todo);
    return Success<Todo, string>(todo);
  }

  public Result<Maybe<Todo>, string> Update(Todo todo) =>
    Get(todo.Id)
      .Bind(_ =>
      {
        _todos.Add(todo);
        return Success<Maybe<Todo>, string>(Maybe.From(todo));
      }) ;

  public UnitResult<string> DeleteAll()
  {
    _todos.Clear();
    return Success();
  }

  public Result<bool, string> Delete(Todo todo) =>
    _todos.Remove(todo) ? Success<bool, string>(true) : Success<bool, string>(false);
}