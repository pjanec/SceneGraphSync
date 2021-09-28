using System.Collections.Generic;

namespace Syncables
{

	public class Syncable : Disposable, ISyncable
	{

		protected List<ISyncer> _syncers;

		public void AddSyncer( ISyncer syncer )
		{
			if( _syncers == null ) _syncers = new List<ISyncer>();

			_syncers.Add( syncer );
		}

		public void RemoveSyncer( ISyncer syncer )
		{
			_syncers.Remove( syncer );
		}

		public void RemoveSyncer( IManager mgr )
		{
			_syncers.RemoveAll( (s) => s.SyncManager == mgr );
		}


		public void Sync( bool forceExt=false )
		{
			if( _syncers == null ) return;

			foreach( var s in _syncers )
			{
				s.Sync( forceExt );
			}
		}
	}

}
