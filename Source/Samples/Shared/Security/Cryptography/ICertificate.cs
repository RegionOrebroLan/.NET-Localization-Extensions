namespace RegionOrebroLan.Security.Cryptography
{
	public interface ICertificate
	{
		#region Properties

		string FriendlyName { get; }
		string Issuer { get; }
		string SerialNumber { get; }
		string Subject { get; }
		string Thumbprint { get; }

		#endregion
	}
}