namespace Todo.Database;

using CSharpFunctionalExtensions;
using Stores;
using Domain.Objs;
using static CSharpFunctionalExtensions.Result;

public class TodoDatabase : ITodoStore
{
  private readonly List<Todo> _todos = [];

  public Result<Todo[], string> GetAll() => _todos.ToArray();

  public Result<Maybe<Todo>, string> Get(Guid id) =>
    Try(() => _todos.Single(x => x.Id == id), ex => ex)
      .Map(Maybe.From)
      .Compensate(ex => ex switch
      {
        InvalidOperationException _ => Success<Maybe<Todo>, string>(Maybe.None),
        _ => Failure<Maybe<Todo>, string>(ex.Message)
      });

  public Result<Todo, string> AddTodo(Todo todo)
  {
    _todos.Add(todo);
    return todo;
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

  public Result<bool, string> Delete(Guid id) => _todos.RemoveAll(x => x.Id.ToString() == id.ToString()) > 0;
}