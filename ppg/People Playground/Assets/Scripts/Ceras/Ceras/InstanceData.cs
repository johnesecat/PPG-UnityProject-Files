using System;
using System.Collections.Generic;
using Ceras.Helpers;
using Ceras.Resolvers;

namespace Ceras
{
	internal struct InstanceData
	{
		public TypeCache TypeCache;

		public ObjectCache ObjectCache;

		public IExternalRootObject CurrentRoot;

		public HashSet<Type> EncounteredSchemaTypes;
	}
}
