using System;
using System.IO;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Events.Edir
{
	public class MonitorEventRequest : LdapExtendedOperation
	{
		static MonitorEventRequest()
		{
			try
			{
				LdapExtendedResponse.register("2.16.840.1.113719.1.27.100.80", Type.GetType("Novell.Directory.Ldap.Events.Edir.MonitorEventResponse", throwOnError: true));
			}
			catch (TypeLoadException)
			{
			}
			catch (Exception)
			{
			}
			try
			{
				LdapIntermediateResponse.register("2.16.840.1.113719.1.27.100.81", Type.GetType("Novell.Directory.Ldap.Events.Edir.EdirEventIntermediateResponse", throwOnError: true));
			}
			catch (TypeLoadException)
			{
			}
			catch (Exception)
			{
			}
		}

		public MonitorEventRequest(EdirEventSpecifier[] specifiers)
			: base("2.16.840.1.113719.1.27.100.79", null)
		{
			if (specifiers == null)
			{
				throw new ArgumentException("PARAM_ERROR");
			}
			MemoryStream memoryStream = new MemoryStream();
			LBEREncoder enc = new LBEREncoder();
			Asn1Sequence asn1Sequence = new Asn1Sequence();
			try
			{
				asn1Sequence.add(new Asn1Integer(specifiers.Length));
				Asn1Set asn1Set = new Asn1Set();
				bool flag = false;
				for (int i = 0; i < specifiers.Length; i++)
				{
					Asn1Sequence asn1Sequence2 = new Asn1Sequence();
					asn1Sequence2.add(new Asn1Integer((int)specifiers[i].EventType));
					asn1Sequence2.add(new Asn1Enumerated((int)specifiers[i].EventResultType));
					if (i == 0)
					{
						flag = specifiers[i].EventFilter != null;
						if (flag)
						{
							setID("2.16.840.1.113719.1.27.100.84");
						}
					}
					if (flag)
					{
						if (specifiers[i].EventFilter == null)
						{
							throw new ArgumentException("Filter cannot be null,for Filter events");
						}
						asn1Sequence2.add(new Asn1OctetString(specifiers[i].EventFilter));
					}
					else if (specifiers[i].EventFilter != null)
					{
						throw new ArgumentException("Filter cannot be specified for non Filter events");
					}
					asn1Set.add(asn1Sequence2);
				}
				asn1Sequence.add(asn1Set);
				asn1Sequence.encode(enc, memoryStream);
			}
			catch (Exception)
			{
				throw new LdapException("ENCODING_ERROR", 83, null);
			}
			setValue(SupportClass.ToSByteArray(memoryStream.ToArray()));
		}
	}
}
