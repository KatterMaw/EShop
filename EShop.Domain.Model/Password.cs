using CommunityToolkit.Diagnostics;
using Isopoh.Cryptography.Argon2;
using Serilog;

namespace EShop.Domain.Model;

public sealed class Password
{
	private static string CreateSalt(byte length)
	{
		const string allowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		var result = new string(Enumerable.Range(0, length)
			.Select(_ => allowedCharacters[Random.Shared.Next(allowedCharacters.Length)]).ToArray());
		Log.ForContext<Password>().Debug("Generated salt: {Salt}", result);
		return result;
	}
	
	public Password(string password)
	{
		Salt = CreateSalt(4);
		Hash = Argon2.Hash(password + Salt);
		Guard.IsTrue(Verify(password));
	}

	public string Hash { get; private set; }
	public string Salt { get; private set; }

	public bool Verify(string password) => Argon2.Verify(Hash, password + Salt);

	private Password(string hash, string salt)
	{
		Hash = hash;
		Salt = salt;
	}
}