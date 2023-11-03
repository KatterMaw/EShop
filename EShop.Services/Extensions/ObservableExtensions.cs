using System.Reactive.Linq;
using CommunityToolkit.Diagnostics;

namespace EShop.Services.Extensions;

public static class ObservableExtensions
{
	public static IObservable<T> GuardIsNotNull<T>(this IObservable<T?> observable) => observable.Select(GuardIsNotNull);

	private static T GuardIsNotNull<T>(T? item)
	{
		Guard.IsNotNull(item);
		return item;
	}
}