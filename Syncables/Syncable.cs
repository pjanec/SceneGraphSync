using System.Collections.Generic;

namespace Syncables
{

	public class Syncable : ISyncable
	{
		protected List<ISyncable> _syncers;

		public void AddSyncer( ISyncable syncer )
		{
			if( _syncers == null ) _syncers = new List<ISyncable>();

			_syncers.Add( syncer );
		}

		public void Sync()
		{
			if( _syncers == null ) return;

			foreach( var s in _syncers )
			{
				s.Sync();
			}
		}
	}

}
