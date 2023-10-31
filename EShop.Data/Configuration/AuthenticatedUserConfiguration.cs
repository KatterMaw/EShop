using EShop.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShop.Data.Configuration;

public sealed class AuthenticatedUserConfiguration : IEntityTypeConfiguration<AuthenticatedUser>
{
	public void Configure(EntityTypeBuilder<AuthenticatedUser> builder)
	{
		builder.OwnsOne(user => user.Password,
			passwordBuilder => 
			{
				passwordBuilder.Property(password => password.Hash).HasColumnName("PasswordHash");
				passwordBuilder.Property(password => password.Salt).HasColumnName("PasswordSalt");
			});
	}
}