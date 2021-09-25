using System;
using System.Collections.Generic;

namespace Net
{
	public class Object
	{
		public Guid Uuid;
	}

	public class Node : Object
	{
		public string Name;
		public Node Parent;
		public List<Node> Children = new List<Node>();
		public List<Component> Components = new List<Component>();
	}

	public class Component : Object
	{
		public string Name;
		public string Content;
	}

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


	}
}
