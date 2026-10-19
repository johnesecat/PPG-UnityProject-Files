using System.Collections;
using System.Collections.Generic;

namespace NaughtyAttributes
{
	public interface IDropdownList : IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
	}
}
