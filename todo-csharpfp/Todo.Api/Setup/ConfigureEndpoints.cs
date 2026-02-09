using System.Net.Mime;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.ViewModel;
using Todo.Pipelines;

namespace Todo.Api.Setup;

using static TypedResults;
using IResult = Microsoft.AspNetCore.Http.IResult;
using Dto = Pipelines.Dto;
using Domain = Domain.Objs;

public static class ConfigureEndpoints
{
  public static void AddApiEndpoints(WebApplication app)
  {
    var services = app.Services;
    var root = app.MapGroup("/api/v1/todo");
    var pipe = services.GetRequiredService<TodoPipelines>();

    root.MapGet("/", IResult ([FromQuery] string? id) =>
      id is null
        ? pipe
          .GetAll()
          .Map(todos => todos.Select(ViewModelMapper.ToViewModel))
          .Match(
            Ok,
            IResult (_) => InternalServerError("Unknown") // TODO: Error mapping
          )
        : pipe
          .Get(id)
          .Map<Maybe<Domain.Todo>, ViewModel.Todo?, string>(maybeTodo => maybeTodo.HasValue 
            ? ViewModelMapper.ToViewModel(maybeTodo.Value) 
            : null)
          .Match(
            Ok,
            IResult (_) => InternalServerError("Unknown")))
      .Produces<ViewModel.Todo[]>()
      .Produces<ViewModel.Todo>()
      .WithName("Get Todo");

    root.MapPost("/", IResult ([FromBody] Dto.Todo todo) =>
      pipe
        .AddTodo(todo)
        .Map(ViewModelMapper.ToViewModel)
        .Match(
          Ok,
          IResult (_) => InternalServerError("Unknown")))
      .Accepts<Dto.Todo>(false, MediaTypeNames.Application.Json)
      .Produces<ViewModel.Todo>()
      .WithName("Post Todo");

    root.MapPatch("/", IResult ([FromQuery] string id, [FromBody] Dto.Todo todo) =>
      pipe
        .UpdateTodo(id, todo)
        .Map<Maybe<Domain.Todo>, ViewModel.Todo?, string>(maybeTodo => maybeTodo.HasValue 
          ? ViewModelMapper.ToViewModel(maybeTodo.Value) 
          : null)
        .Match(
          Ok,
          IResult (_) => InternalServerError("Unknown")))
      .Accepts<Dto.Todo>(false, MediaTypeNames.Application.Json)
      .Produces<ViewModel.Todo>()
      .WithName("Patch Todo");

    root.MapDelete("/", IResult ([FromQuery] string? id) =>
      id is null
        ? pipe.DeleteAll()
          .Match(
            Ok,
            IResult (_) => InternalServerError("Unknown"))
        : pipe
          .Delete(id)
          .Match(
            Ok,
            IResult (_) => InternalServerError("Unknown")))
      .WithName("Delete Todo");
  }
}