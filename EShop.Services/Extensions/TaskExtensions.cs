using System.Reactive.Threading.Tasks;

namespace EShop.Services.Extensions;

public static class TaskExtensions
{
	public static Task<T> GuardIsNotNull<T>(this Task<T?> task) => task.ToObservable().GuardIsNotNull().ToTask();
}