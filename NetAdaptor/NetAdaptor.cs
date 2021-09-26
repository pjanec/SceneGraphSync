using System;
using System.Collections.Generic;
using System.Linq;

namespace SceneGraphSync
{
	public class NetAdaptor : Syncables.IManager
	{
		Net.Manager _nm;

		NetCoupler<Node, Net.Node> _nodeCoupler = new NetCoupler<Node, Net.Node>();
		NetCoupler<Component, Net.Component> _componentCoupler = new NetCoupler<Component, Net.Component>();

		Queue<Syncables.Event> _events = new Queue<Syncables.Event>();


		public NetAdaptor( Net.Manager bm )
		{
			_nm = bm;
			_nm.OnObjectCreated += HandleNetObjectCreated;
			_nm.OnObjectDestroyed += HandleNetObjectDestroyed;

			// initialize from existing objects on the network

			foreach( var o in _nm.GetObjecstOfType<Net.Node>() )
			{
				HandleNetObjectCreated( o );
			}

			foreach( var o in _nm.GetObjecstOfType<Net.Component>() )
			{
				HandleNetObjectCreated( o );
			}
		}

		public void Sync()
		{
			// ticks the network
			_nm.Tick();

			// ticks the sync on all the native nodes
			foreach( var native in _nodeCoupler.GetNatives() )
			{
				native.Sync();
			}

			// ticks the sync on all the native components
			foreach( var native in _componentCoupler.GetNatives() )
			{
				native.Sync();
			}
		}

		public T Create<T>() where T:Syncables.Syncable
		{
			if(typeof(T) == typeof(Node)) return CreateNode() as T;
			if(typeof(T) == typeof(Component)) return CreateComponent() as T; 
			return null;
		}

		public void Destroy<T>(T obj) where T:Syncables.Syncable
		{
			if(typeof(T) == typeof(Node)) DestroyNode(obj as Node);
			if(typeof(T) == typeof(Component)) DestroyComponent(obj as Component); 
		}


		// Read events periodically in order to find out what object was created/destroyed
		public Syncables.Event PopEvent()
		{
			if( _events.Count == 0 ) return null;
			return _events.Dequeue();
		}

		void PushEvent( Syncables.Event ev )
		{
			_events.Enqueue( ev );
		}

		void HandleNetObjectCreated( Net.Object net )
		{
			if( net is Net.Node )
			{
				var node = net as Net.Node;
				// we need to ignore the event if the net object already exists (the event might come later when object is already created)
				GetOrCreateNode( node );
				Console.WriteLine( $"Net Node created {node.Name}" );

			}
			else
			if( net is Net.Component )
			{
				var comp = net as Net.Component;
				// we need to ignore the event if the net object already exists (the event might come later when object is already created)
				GetOrCreateComponent( comp );
				Console.WriteLine( $"Net Component created {comp.Name}" );
			}
		}

		void HandleNetObjectDestroyed( Net.Object net )
		{
			if( net is Net.Node )
			{
				var node = net as Net.Node;
				var native = _nodeCoupler.Find( net as Net.Node );
				DestroyNode( native );
				Console.WriteLine( $"Net Node destroyed {node.Name}" );
			}
			else
			if( net is Net.Component )
			{
				var comp = net as Net.Component;
				var native = _componentCoupler.Find( net as Net.Component );
				DestroyComponent( native );
				Console.WriteLine( $"Net Component destroyed {comp.Name}" );
			}
		}

		public Node CreateNode(string name = "")
		{
			var o = _nm.CreateObject<Net.Node>();
			o.Name = name;
			var n = CreateNode( o );
			PushEvent( new Syncables.EventObjectCreated(n) );
			return n;
		}

		public Node CreateNode( Net.Node net )
		{
			var native = new Node();
			_nodeCoupler.Add( native, net );

			native.AddSyncer( new Syncables.PrimSyncer<string>(
				() => native.Name,
				( x ) => native.Name = x,
				() => net.Name,
				( x ) => net.Name = x,
				( x ) => Console.WriteLine( $"Ext chnaged: {x}" )
			) );

			native.AddSyncer( new Syncables.PrimSyncer<Node>(
				() => native.Parent,
				( x ) => native.Parent = x,
				() => GetOrCreateNode( net.Parent ),
				( x ) => net.Parent = GetNode( native.Parent ),
				( x ) => Console.WriteLine( $"Ext chnaged: {x}" )
			) );

			native.AddSyncer( new Syncables.ListSyncer<Node>(
				() => native.Children,
				( x ) => native.Children = x,
				() => (from i in net.Children select GetOrCreateNode( i )).ToList(),
				( x ) => net.Children = (from i in x select GetNode( i )).ToList(),
				( x ) => Console.WriteLine( $"Children Ext chnaged: {String.Join( ",", from i in x select i == null ? "<null>" : i.Name )}" )
			) );

			native.AddSyncer( new Syncables.ListSyncer<Component>(
				() => native.Components,
				( x ) => native.Components = x,
				() => (from i in net.Components select GetOrCreateComponent(i)).ToList(),
				( x ) => net.Components = (from i in x select GetComponent(i)).ToList(),
				( x ) => Console.WriteLine( $"Components Ext chnaged: {String.Join( ",", from i in x select i==null?"<null>":i.Name )}" )
			) );

			// updates the native from net
			native.Sync();

			return native;
		}

		public void DestroyNode( Node native )
		{
			if( native == null ) return;
			var net = _nodeCoupler.Remove( native );
			if( net != null ) _nm.DestroyObject( net );

			PushEvent( new Syncables.EventObjectDestroyed(native) );
		}

		public Net.Node GetNode( Node native )
		{
			return _nodeCoupler.Find( native );
		}

		public Node GetNode( Net.Node net )
		{
			return _nodeCoupler.Find( net );
		}

		Node GetOrCreateNode( Net.Node net )
		{
			if( net == null ) return null;

			var n = _nodeCoupler.Find( net );
			if( n == null )
			{
				n = CreateNode( net );
			}

			return n;
		}

		public Component CreateComponent(string name = "")
		{
			var net = _nm.CreateObject<Net.Component>();
			net.Name = name;
			var native =  CreateComponent( net );
			PushEvent( new Syncables.EventObjectCreated(native) {} );
			return native;

		}

		public Component CreateComponent( Net.Component net )
		{
			var native = new Component();
			_componentCoupler.Add( native, net );

			native.AddSyncer( new Syncables.PrimSyncer<string>(
				() => native.Name,
				( x ) => native.Name = x,
				() => net.Name,
				( x ) => net.Name = x,
				( x ) => Console.WriteLine( $"Comp.Name Ext chnaged: {x}" )
			) );

			native.AddSyncer( new Syncables.PrimSyncer<string>(
				() => native.Content,
				( x ) => native.Content = x,
				() => net.Content,
				( x ) => net.Content = x,
				( x ) => Console.WriteLine( $"Comp.Content Ext chnaged: {x}" )
			) );

			// updated the native from net
			native.Sync();

			return native;
		}

		public void DestroyComponent( Component native )
		{
			var net = _componentCoupler.Remove( native );
			if( net != null ) _nm.DestroyObject( net );
			PushEvent( new Syncables.EventObjectDestroyed(native) );
		}

		public Net.Component GetComponent( Component native )
		{
			return _componentCoupler.Find( native );
		}

		public Component GetComponent( Net.Component net )
		{
			return _componentCoupler.Find( net );
		}

		Component GetOrCreateComponent( Net.Component net )
		{
			if( net == null ) return null;

			var native = _componentCoupler.Find( net );
			if( native == null )
			{
				native = CreateComponent( net );
			}

			return native;
		}
	}
}
