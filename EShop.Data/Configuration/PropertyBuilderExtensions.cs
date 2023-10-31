using FlakeId;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShop.Data.Configuration;

public static class PropertyBuilderExtensions
{
	public static PropertyBuilder<Id> HasNumberConversion(this PropertyBuilder<Id> builder) =>
		builder.HasConversion<long>(id => id, number => new Id(number));
}