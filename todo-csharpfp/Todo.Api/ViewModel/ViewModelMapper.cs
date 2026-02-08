namespace Todo.Api.ViewModel;

using Domain = Domain.Objs;

public static class ViewModelMapper
{
  public static ViewModel.Todo ToViewModel(Domain.Todo todo) => 
    new(
      todo.Id.ToString(), 
      todo.Title, 
      todo.Completed, 
      todo.Order,
      $"http://localhost:5080/api/v1/todo?id={todo.Id.ToString()}");
}