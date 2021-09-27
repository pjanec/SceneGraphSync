namespace Syncables
{
	public interface ISyncable
	{
		// Checks for changes agains last stored value and update if there is a difference.
		// If forceExt==true then checks just the external source
		// Otherwise checks forst the internal source and then the external (internal wins if changes on both sources)
		void Sync( bool forceExt=false ); 
	}
}
