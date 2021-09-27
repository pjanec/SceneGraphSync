using System;
namespace Syncables
{
	// Manages registration for syncing
	// Signals all registrations/unregistartion (important if they come from the other party)
	public interface IManager : ISyncable
	{
		void Register<T>( T obj ) where T:Syncable; // registers the object for synchronization; reports via OnRegistered
		void Unregister<T>(T obj) where T:Syncable;	// unregisters the object from synchronization; reports via OnUnregistered

		Action<Syncable> OnRegistered { get; set; }
		Action<Syncable> OnUnregistered { get; set; }
	}

}
