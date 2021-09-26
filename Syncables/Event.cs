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
	public class EventObjectCreated : EventObject
	{
		public EventObjectCreated( Syncable obj ) : base(obj)
		{
		}

		public override string ToString()
		{
			return $"Created {Object.GetType().Name} {Object}";
		}
	}

	public class EventObjectDestroyed : EventObject
	{
		public EventObjectDestroyed( Syncable obj ) : base(obj)
		{
		}

		public override string ToString()
		{
			return $"Destroyed {Object}";
		}
	}

}
