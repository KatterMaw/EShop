using EShop.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShop.Data.Configuration;

public sealed class AdministratorConfiguration : IEntityTypeConfiguration<Administrator>
{
	public void Configure(EntityTypeBuilder<Administrator> builder)
	{
		builder.ToTable("Administrators");
		builder.HasOne(user => user.User).WithOne().HasForeignKey<Administrator>("Id");
	}
}