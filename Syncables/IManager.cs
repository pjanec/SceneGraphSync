namespace Syncables
{
	// Manages creation/destruction of objects and their syncing.
	public interface IManager : ISyncable
	{
		T Create<T>() where T:Syncables.Syncable;
		void Destroy<T>(T obj) where T:Syncables.Syncable;
		Event PopEvent();
	}

}
