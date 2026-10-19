using System.Security.Cryptography.X509Certificates;

namespace Novell.Directory.Ldap
{
	public delegate bool CertificateValidationCallback(X509Certificate certificate, int[] certificateErrors);
}
