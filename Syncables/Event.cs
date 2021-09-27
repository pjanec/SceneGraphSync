namespace Syncables
{
	public class Event { }

	public class EventObject : Event
	{
		public Syncable Object;
		public EventObject( Syncable obj )
		{
			Object = obj;
		}

	}
	public class EventObjectRegistered : EventObject
	{
		public EventObjectRegistered( Syncable obj ) : base(obj)
		{
		}

		public override string ToString()
		{
			return $"Registered {Object.GetType().Name} {Object}";
		}
	}

	public class EventObjectUnregistered : EventObject
	{
		public EventObjectUnregistered( Syncable obj ) : base(obj)
		{
		}

		public override string ToString()
		{
			return $"Unregistered {Object}";
		}
	}

}
