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

}
