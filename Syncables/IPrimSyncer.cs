using System;

namespace Syncables
{
	public interface IPrimSyncer<T> : ISyncable where T:IEquatable<T>
	{
		// Called when value changed from the external source (like some kind of network shared objects)
		Action<T> OnExtChanged { get; set; }
	}

}
