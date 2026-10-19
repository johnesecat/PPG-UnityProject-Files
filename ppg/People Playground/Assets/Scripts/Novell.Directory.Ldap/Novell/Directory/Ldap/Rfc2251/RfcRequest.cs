namespace Novell.Directory.Ldap.Rfc2251
{
	public interface RfcRequest
	{
		RfcRequest dupRequest(string base_Renamed, string filter, bool reference);

		string getRequestDN();
	}
}
