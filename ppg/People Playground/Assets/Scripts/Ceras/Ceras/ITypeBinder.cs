using System;

namespace Ceras
{
	public interface ITypeBinder
	{
		string GetBaseName(Type type);

		Type GetTypeFromBase(string baseTypeName);

		Type GetTypeFromBaseAndArguments(string baseTypeName, params Type[] genericTypeArguments);
	}
}
