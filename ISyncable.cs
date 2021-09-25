using System;
using System.Collections.Generic;

namespace Syncables
{
	public interface ISyncable
	{
		// Reads value from the external source
		// Compares with the last Set value
		// What if the change comes from both sources, internal end external? Internal wins...
		// Fires on change 
		void Sync(); 
	}

	public interface ISyncPrim<T> : ISyncable where T:IEquatable<T>
	{
		// Called when value changed from the external source (like some kind of network shared objects)
		Action<T> OnExtChanged { get; set; }
	}

	public interface ISyncList<T> : ISyncable where T:IEquatable<T>
	{
		// Called when value changed from the external source (like some kind of network shared objects)
		// Action arguments: added, removed, modified
		Action<List<T>> OnExtChanged { get; set; }
	}

}
