using System;
using System.Collections.Generic;
using System.Linq;

namespace SceneGraphSync
{
	public class NetAdaptor : Syncables.IManager
	{
		public Action<Syncables.Syncable> OnRegistered { get; set; }
		public Action<Syncables.Syncable> OnUnregistered { get; set; }

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

		public void Sync( bool forceExt=false )
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

			// fire create/destroy events
			while(true)
			{
				var ev = PopEvent();
				if( ev == null ) break;
				if( ev is Syncables.EventObjectRegistered )
				{
					if( OnRegistered != null )
						OnRegistered( ((Syncables.EventObjectRegistered)ev).Object );
				}
				else
				if( ev is Syncables.EventObjectUnregistered )
				{
					if( OnUnregistered != null )
						OnUnregistered( ((Syncables.EventObjectUnregistered)ev).Object );
				}
			}

		}

		public void Register<T>( T obj ) where T:Syncables.Syncable
		{
			if(typeof(T) == typeof(Node))
			{
				CreateNodeOnNetAndRegister( obj as Node );
			}
			else
			if(typeof(T) == typeof(Component))
			{
				CreateComponentOnNetAndRegister( obj as Component );
			}
			else
			{
				throw new System.Exception($"Type {obj.GetType().FullName} not suported for syncing");
			}
		}

		public void Unregister<T>( T obj ) where T:Syncables.Syncable
		{
			if(typeof(T) == typeof(Node))
			{
				UnregisterNode(obj as Node);
			}
			else
			if(typeof(T) == typeof(Component))
			{
				UnregisterComponent(obj as Component); 
			}
			else
			{
				throw new System.Exception($"Type {obj.GetType().FullName} not suported for syncing");
			}
		}


		// Read events periodically in order to find out what object was created/destroyed
		Syncables.Event PopEvent()
		{
			if (_events.Count == 0) return null;
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
				GetOrCreateNodeFromNet( node );
				Console.WriteLine( $"[Net] Created Node {node.Name}" );

			}
			else
			if( net is Net.Component )
			{
				var comp = net as Net.Component;
				// we need to ignore the event if the net object already exists (the event might come later when object is already created)
				GetOrCreateComponentFromNet( comp );
				Console.WriteLine( $"[Net] Created Component {comp.Name}" );
			}
		}

		void HandleNetObjectDestroyed( Net.Object net )
		{
			if( net is Net.Node )
			{
				var node = net as Net.Node;
				var native = _nodeCoupler.Find( net as Net.Node );
				UnregisterNode( native );
				Console.WriteLine( $"[Net] Destroyed Node {node.Name}" );
			}
			else
			if( net is Net.Component )
			{
				var comp = net as Net.Component;
				var native = _componentCoupler.Find( net as Net.Component );
				UnregisterComponent( native );
				Console.WriteLine( $"[Net] Destroyed Component {comp.Name}" );
			}
		}

		public Node CreateNodeOnNetAndRegister( Node native )
		{
			var net = new Net.Node();
			RegisterNodeFromNet( net, native, false ); // fills the fields from native
			_nm.PublishObject( net );
			PushEvent( new Syncables.EventObjectRegistered(native) );
			return native;
		}

		public Component CreateComponentOnNetAndRegister( Component native )
		{
			var net = new Net.Component();
			RegisterComponentFromNet( net, native, false ); // fills the fields from native
			_nm.PublishObject( net );
			PushEvent( new Syncables.EventObjectRegistered(native) {} );
			return native;

		}

		public void RegisterNodeFromNet( Net.Node net, Node native, bool forceExt )
		{
			_nodeCoupler.Add( native, net );

			List<Syncables.ISyncer> syncers = new List<Syncables.ISyncer>(5);
			 
			syncers.Add( new Syncables.PrimSyncer<string>(
				this,
				() => native.Name,
				( x ) => native.Name = x,
				() => net.Name,
				( x ) => net.Name = x,
				( x ) => Console.WriteLine( $"Node.Name Ext chnaged: {x}" )
			) );

			syncers.Add( new Syncables.PrimSyncer<Node>(
				this,
				() => native.Parent,
				( x ) => native.Parent = x,
				() => GetOrCreateNodeFromNet( net.Parent ),
				( x ) => net.Parent = GetNode( native.Parent ),
				( x ) => Console.WriteLine( $"Node.Parent Ext chnaged: {x}" )
			) );

			syncers.Add( new Syncables.ListSyncer<Node>(
				this,
				() => native.Children,
				( x ) => native.Children = x,
				() => (from i in net.Children select GetOrCreateNodeFromNet( i )).ToList(),
				( x ) => net.Children = (from i in x select GetNode( i )).ToList(),
				( x ) => Console.WriteLine( $"Node.Children Ext chnaged: {String.Join( ",", from i in x select i == null ? "<null>" : i.Name )}" )
			) );

			syncers.Add( new Syncables.ListSyncer<Component>(
				this,
				() => native.Components,
				( x ) => native.Components = x,
				() => (from i in net.Components select GetOrCreateComponentFromNet(i)).ToList(),
				( x ) => net.Components = (from i in x select GetComponent(i)).ToList(),
				( x ) => Console.WriteLine( $"Node.Components Ext chnaged: {String.Join( ",", from i in x select i==null?"<null>":i.Name )}" )
			) );

			// updates the native from net
			foreach( var i in syncers) i.Sync( forceExt );

			// install syncers to the syncable
			foreach( var i in syncers ) native.AddSyncer( i );
		}

		public Component RegisterComponentFromNet( Net.Component net, Component native, bool forceExt )
		{
			_componentCoupler.Add( native, net );

			List<Syncables.ISyncer> syncers = new List<Syncables.ISyncer>(5);

			syncers.Add( new Syncables.PrimSyncer<string>(
				this,
				() => native.Name,
				( x ) => native.Name = x,
				() => net.Name,
				( x ) => net.Name = x,
				( x ) => Console.WriteLine( $"Comp.Name Ext chnaged: {x}" )
			) );

			syncers.Add( new Syncables.PrimSyncer<string>(
				this,
				() => native.Content,
				( x ) => native.Content = x,
				() => net.Content,
				( x ) => net.Content = x,
				( x ) => Console.WriteLine( $"Comp.Content Ext chnaged: {x}" )
			) );

			// updates the native from net
			foreach( var i in syncers) i.Sync( forceExt );

			// install syncers to the syncable
			foreach( var i in syncers ) native.AddSyncer( i );

			return native;
		}


		public void UnregisterNode( Node native )
		{
			if( native == null ) return;

			var net = _nodeCoupler.Remove( native );

			if( net != null ) _nm.UnpublishObject( net );

			native.RemoveSyncer( this );

			PushEvent( new Syncables.EventObjectUnregistered(native) );
		}

		public void UnregisterComponent( Component native )
		{
			if( native == null ) return;

			var net = _componentCoupler.Remove( native );

			if( net != null ) _nm.UnpublishObject( net );

			native.RemoveSyncer( this );

			PushEvent( new Syncables.EventObjectUnregistered(native) );
		}


		public Net.Node GetNode( Node native )
		{
			return _nodeCoupler.Find( native );
		}

		public Net.Component GetComponent( Component native )
		{
			return _componentCoupler.Find( native );
		}


		public Node GetNode( Net.Node net )
		{
			return _nodeCoupler.Find( net );
		}

		public Component GetComponent( Net.Component net )
		{
			return _componentCoupler.Find( net );
		}


		Node GetOrCreateNodeFromNet( Net.Node net )
		{
			if( net == null ) return null;

			var native = _nodeCoupler.Find( net );
			if( native == null )
			{
				native = new Node();
				RegisterNodeFromNet( net, native, true ); // fill the fields from net
			}

			return native;
		}

		Component GetOrCreateComponentFromNet( Net.Component net )
		{
			if( net == null ) return null;

			var native = _componentCoupler.Find( net );
			if( native == null )
			{
				native = new Component();
				RegisterComponentFromNet( net, native, true );  // fill the fields from net
			}

			return native;
		}

	}
}
