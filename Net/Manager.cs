using System;
using System.Collections.Generic;

namespace Net
{
	// emulates some kind of network-shared object repository
	public class Manager
	{
		public void PublishObject( Object o )
		{
			o.Uuid = Guid.NewGuid();
			_deferredEvents.Add( () => { if( OnObjectCreated !=null ) OnObjectCreated(o);} );
			//if( OnObjectCreated !=null ) OnObjectCreated(o);
		}

		public void UnpublishObject( Object o )
		{
			if( o == null ) return;
			_deferredEvents.Add( () => { if( OnObjectDestroyed !=null ) OnObjectDestroyed(o); } );
			//if( OnObjectDestroyed !=null ) OnObjectDestroyed(o);
		}

		public Action<Object> OnObjectCreated;
		public Action<Object> OnObjectDestroyed;

		List<Action> _deferredEvents = new List<Action>();

		public void Tick()
		{
			foreach( var eventAction in _deferredEvents )
			{
				eventAction();
			}
		}

		public List<Object> GetObjecstOfType<T>() where T:Object
		{
			return new List<Object>();
		}


	}
}
