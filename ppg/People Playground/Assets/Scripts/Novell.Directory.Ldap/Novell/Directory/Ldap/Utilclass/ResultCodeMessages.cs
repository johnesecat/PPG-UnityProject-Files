using System.Resources;

namespace Novell.Directory.Ldap.Utilclass
{
	public class ResultCodeMessages : ResourceManager
	{
		internal static readonly object[][] contents = new object[59][]
		{
			new object[2] { "0", "Success" },
			new object[2] { "1", "Operations Error" },
			new object[2] { "2", "Protocol Error" },
			new object[2] { "3", "Timelimit Exceeded" },
			new object[2] { "4", "Sizelimit Exceeded" },
			new object[2] { "5", "Compare False" },
			new object[2] { "6", "Compare True" },
			new object[2] { "7", "Authentication Method Not Supported" },
			new object[2] { "8", "Strong Authentication Required" },
			new object[2] { "9", "Partial Results" },
			new object[2] { "10", "Referral" },
			new object[2] { "11", "Administrative Limit Exceeded" },
			new object[2] { "12", "Unavailable Critical Extension" },
			new object[2] { "13", "Confidentiality Required" },
			new object[2] { "14", "SASL Bind In Progress" },
			new object[2] { "16", "No Such Attribute" },
			new object[2] { "17", "Undefined Attribute Type" },
			new object[2] { "18", "Inappropriate Matching" },
			new object[2] { "19", "Constraint Violation" },
			new object[2] { "20", "Attribute Or Value Exists" },
			new object[2] { "21", "Invalid Attribute Syntax" },
			new object[2] { "32", "No Such Object" },
			new object[2] { "33", "Alias Problem" },
			new object[2] { "34", "Invalid DN Syntax" },
			new object[2] { "35", "Is Leaf" },
			new object[2] { "36", "Alias Dereferencing Problem" },
			new object[2] { "48", "Inappropriate Authentication" },
			new object[2] { "49", "Invalid Credentials" },
			new object[2] { "50", "Insufficient Access Rights" },
			new object[2] { "51", "Busy" },
			new object[2] { "52", "Unavailable" },
			new object[2] { "53", "Unwilling To Perform" },
			new object[2] { "54", "Loop Detect" },
			new object[2] { "64", "Naming Violation" },
			new object[2] { "65", "Object Class Violation" },
			new object[2] { "66", "Not Allowed On Non-leaf" },
			new object[2] { "67", "Not Allowed On RDN" },
			new object[2] { "68", "Entry Already Exists" },
			new object[2] { "69", "Object Class Modifications Prohibited" },
			new object[2] { "71", "Affects Multiple DSAs" },
			new object[2] { "80", "Other" },
			new object[2] { "81", "Server Down" },
			new object[2] { "82", "Local Error" },
			new object[2] { "83", "Encoding Error" },
			new object[2] { "84", "Decoding Error" },
			new object[2] { "85", "Ldap Timeout" },
			new object[2] { "86", "Authentication Unknown" },
			new object[2] { "87", "Filter Error" },
			new object[2] { "88", "User Cancelled" },
			new object[2] { "89", "Parameter Error" },
			new object[2] { "90", "No Memory" },
			new object[2] { "91", "Connect Error" },
			new object[2] { "92", "Ldap Not Supported" },
			new object[2] { "93", "Control Not Found" },
			new object[2] { "94", "No Results Returned" },
			new object[2] { "95", "More Results To Return" },
			new object[2] { "96", "Client Loop" },
			new object[2] { "97", "Referral Limit Exceeded" },
			new object[2] { "112", "TLS not supported" }
		};

		public object[][] getContents()
		{
			return contents;
		}
	}
}
