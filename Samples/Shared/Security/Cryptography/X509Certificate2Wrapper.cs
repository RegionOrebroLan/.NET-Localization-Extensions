using System;
using System.Security.Cryptography.X509Certificates;

namespace Shared.Security.Cryptography
{
	public class X509Certificate2Wrapper : ICertificate
	{
		#region Constructors

		public X509Certificate2Wrapper(X509Certificate2 certificate)
		{
			this.Certificate = certificate ?? throw new ArgumentNullException(nameof(certificate));
		}

		#endregion

		#region Properties

		protected internal virtual X509Certificate2 Certificate { get; }
		public virtual string FriendlyName => this.Certificate.FriendlyName;
		public virtual string Issuer => this.Certificate.Issuer;
		public virtual string SerialNumber => this.Certificate.SerialNumber;
		public virtual string Subject => this.Certificate.Subject;
		public virtual string Thumbprint => this.Certificate.Thumbprint;

		#endregion

		#region Methods

		public static X509Certificate2Wrapper FromX509Certificate2(X509Certificate2 certificate)
		{
			return certificate;
		}

		#region Implicit operator

		public static implicit operator X509Certificate2Wrapper(X509Certificate2 certificate)
		{
			return certificate != null ? new X509Certificate2Wrapper(certificate) : null;
		}

		#endregion

		#endregion
	}
}