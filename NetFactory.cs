using System;
using System.Linq;

namespace SceneGraphSync
{
	public class NetFactory
	{
		Net.Manager _nm;
		UInt32 _instIdCounter = 1;

		NetCoupler<Node, Net.Node> _nodeCoupler = new NetCoupler<Node, Net.Node>();
		NetCoupler<Component, Net.Component> _componentCoupler = new NetCoupler<Component, Net.Component>();

		public NetFactory( Net.Manager bm )
		{
			_nm = bm;
		}

		public Node CreateNode(string name = "")
		{
			var o = _nm.CreateObject<Net.Node>( _instIdCounter++ );
			o.Name = name;
			return CreateNode( o );
		}

		public Node CreateNode( Net.Node o )
		{
			var n = GetOrCreateNode( o );

			n.AddSyncer( new Syncables.ExtSyncPrim<string>(
				() => n.Name,
				( x ) => n.Name = x,
				() => o.Name,
				( x ) => o.Name = x,
				( x ) => Console.WriteLine( $"Ext chnaged: {x}" )
			) );

			n.AddSyncer( new Syncables.ExtSyncPrim<Node>(
				() => n.Parent,
				( x ) => n.Parent = x,
				() => GetOrCreateNode( o.Parent ),
				( x ) => o.Parent = GetExtNode( n.Parent ),
				( x ) => Console.WriteLine( $"Ext chnaged: {x}" )
			) );

			n.AddSyncer( new Syncables.ExtSyncList<Node>(
				() => n.Children,
				( x ) => n.Children = x,
				() => (from i in o.Children select GetOrCreateNode( i )).ToList(),
				( x ) => o.Children = (from i in x select GetExtNode( i )).ToList(),
				( x ) => Console.WriteLine( $"Children Ext chnaged: {String.Join( ",", from i in x select i == null ? "<null>" : i.Name )}" )
			) );

			n.AddSyncer( new Syncables.ExtSyncList<Component>(
				() => n.Components,
				( x ) => n.Components = x,
				() => (from i in o.Components select GetOrCreateComponent(i)).ToList(),
				( x ) => o.Components = (from i in x select GetExtComponent(i)).ToList(),
				( x ) => Console.WriteLine( $"Components Ext chnaged: {String.Join( ",", from i in x select i==null?"<null>":i.Name )}" )
			) );

			return n;
		}

		public void DestroyNode( Node node )
		{
			var netNode = _nodeCoupler.Remove( node );
			_nm.DestroyObject( netNode );
		}

		public Net.Node GetExtNode( Node node )
		{
			return _nodeCoupler.Find( node );
		}

		Node GetOrCreateNode( Net.Node o )
		{
			if( o == null ) return null;

			var n = _nodeCoupler.Find( o );
			if( n == null )
			{
				n = new Node();
				_nodeCoupler.Add( n, o );

				n.Name = o.Name;
				n.Parent = GetOrCreateNode( o.Parent );
				n.Children = (from i in o.Children select GetOrCreateNode(i)).ToList();
			}

			return n;
		}

		public Component CreateComponent(string name = "")
		{
			var o = _nm.CreateObject<Net.Component>( _instIdCounter++ );
			o.Name = name;
			return CreateComponent( o );
		}

		public Component CreateComponent( Net.Component o )
		{
			var n = GetOrCreateComponent( o );

			n.AddSyncer( new Syncables.ExtSyncPrim<string>(
				() => n.Name,
				( x ) => n.Name = x,
				() => o.Name,
				( x ) => o.Name = x,
				( x ) => Console.WriteLine( $"Comp.Name Ext chnaged: {x}" )
			) );

			n.AddSyncer( new Syncables.ExtSyncPrim<string>(
				() => n.Content,
				( x ) => n.Content = x,
				() => o.Content,
				( x ) => o.Content = x,
				( x ) => Console.WriteLine( $"Comp.Content Ext chnaged: {x}" )
			) );

			return n;
		}

		public void DestroyComponent( Component comp )
		{
			var net = _componentCoupler.Remove( comp );
			_nm.DestroyObject( net );
		}

		public Net.Component GetExtComponent( Component comp )
		{
			return _componentCoupler.Find( comp );
		}

		Component GetOrCreateComponent( Net.Component o )
		{
			if( o == null ) return null;

			var n = _componentCoupler.Find( o );
			if( n == null )
			{
				n = new Component();
				_componentCoupler.Add( n, o );

				// copy all fields here
				n.Name = o.Name;
				n.Content = o.Content;
			}

			return n;
		}

	}
}
