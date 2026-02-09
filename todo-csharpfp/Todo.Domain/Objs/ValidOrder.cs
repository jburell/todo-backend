using CSharpFunctionalExtensions;

namespace Todo.Domain.Objs;

public record ValidOrder
{
  public Maybe<int> Order { get; }
  
  private ValidOrder(Maybe<int> order) => Order = order;
  
  public static Result<ValidOrder, string> Create(int? order) =>
    order != null 
      ? Result.SuccessIf<ValidOrder, string>(order is >= 0, new ValidOrder(order.Value), "Order cannot be negative or null")
      : Result.Success<ValidOrder, string>(new ValidOrder(Maybe.None));
}