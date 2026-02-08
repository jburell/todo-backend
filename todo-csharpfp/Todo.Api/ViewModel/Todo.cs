namespace Todo.Api.ViewModel;

public record Todo(string Id, string Title, bool Completed, int? Order, string Url);