using System;
using System.Collections.Generic;

namespace Net
{
	// emulates some kind of network-shared object repository
	public class Manager
	{
		public T CreateObject<T>() where T:Object, new()
		{
			var o = new T() { Uuid = Guid.NewGuid() };
			_deferredEvents.Add( () => { if( OnObjectCreated !=null ) OnObjectCreated(o);} );
			
			return o;
		}

		public void DestroyObject( Object o )
		{
			if( o == null ) return;
			_deferredEvents.Add( () => { if( OnObjectDestroyed !=null ) OnObjectDestroyed(o); } );
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
