using CSharpFunctionalExtensions;

namespace Todo.Domain.Objs;

public record ValidTitle
{
  public Maybe<string> Title { get; }
  
  private ValidTitle(Maybe<string> title) => Title = title;
  
  public static Result<ValidTitle, string> Create(string? title) =>
    title != null
      ? Result.SuccessIf<ValidTitle, string>(!string.IsNullOrWhiteSpace(title), new ValidTitle(title), "Title cannot be empty")
      : Result.Success<ValidTitle, string>(new ValidTitle(Maybe.None));
}