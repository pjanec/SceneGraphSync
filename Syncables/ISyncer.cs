namespace Syncables
{
	public interface ISyncer : ISyncable
	{
		IManager SyncManager { get; } // identifies who created this syncer
	}
}
