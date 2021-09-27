using System;
using System.Collections.Generic;

namespace Syncables
{
	public interface IListSyncer<T> : ISyncer where T:IEquatable<T>
	{
		// Called when value changed from the external source (like some kind of network shared objects)
		// Action arguments: added, removed, modified
		Action<List<T>> OnExtChanged { get; set; }
	}

}
