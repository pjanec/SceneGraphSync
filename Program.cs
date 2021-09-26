using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SceneGraphSync
{

	class Program
	{
		static void Main( string[] args )
		{
			//string extNodeName = null;
			//List<Node> extChildren = null;

			//var n = new Node();

			//n.AddSyncer( new Syncables.ExtSyncPrim<string>(
			//	() => n.Name,
			//	(x) => n.Name = x,
			//	() => extNodeName,
			//	(x) => { extNodeName=x; Console.WriteLine($"writing Node.Name to Ext: {x}"); },
			//	(x) => Console.WriteLine($"Ext chnaged: {x}")
			//) );

			//n.AddSyncer( new Syncables.ExtSyncList<Node>(
			//	() => n.Children,
			//	(x) => n.Children = x,
			//	() => extChildren,
			//	(x) => { extChildren=x; Console.WriteLine($"writing Node.Children to Ext: {String.Join(",", from i in x select i.Name)}"); },
			//	(x) => Console.WriteLine($"Ext chnaged: {String.Join(",", from i in x select i.Name)}")
			//) );

			//n.Sync();
			//n.Name = "ahoj";
			//n.Sync();
			//extNodeName = "nazdar";
			//n.Sync();
			//n.Name = "set from int";
			//extNodeName = "sent from ext";
			//n.Sync();

			//n.Sync();
			//n.Children = new List<Node>();
			//n.Sync();
			//n.Children = new List<Node>() { new Node() { Name="1"} };
			//n.Sync();
			//extChildren = new List<Node>() { new Node() { Name="2"} };
			//n.Sync();


			var nm = new Net.Manager();	 // network object repository manager
			var adapt = new NetAdaptor( nm ); // syncer between native and network objects
			var mgr = adapt as Syncables.IManager; // same but just as generic interface
			var n1 = mgr.Create<Node>(); n1.Name="1";
			//n.Name = "hi!";
			var o1 = adapt.GetNode( n1 ); 

			n1.Sync();
			n1.Children = new List<Node>();
			var c1 = mgr.Create<Component>(); c1.Name="C1";
			n1.Components.Add( c1 );
			n1.Sync();
			var n2 = mgr.Create<Node>(); n2.Name = "2";
			n1.Children = new List<Node>() { n2 };
			n1.Sync();
			var o3 = nm.CreateObject<Net.Node>(); o3.Name="3";
			o1.Children = new List<Net.Node>() { o3 };
			n1.Sync();

			// simulate node creation from a network; parented to node "1"
			var o2 = nm.CreateObject<Net.Node>(); o2.Name = "4"; o2.Parent = o1;
			var c2 = nm.CreateObject<Net.Component>(); c2.Name = "C2";
			o2.Components.Add( c2 );
			mgr.Sync(); // fires the net object creation callback - the native object should be created
			var n2_o2 = adapt.GetNode( o2 ); 

			o1.Components.Clear();
			Tick(mgr); // component should be missing on n1

		}

		static void Tick(Syncables.IManager syncMgr)
		{
			syncMgr.Sync();
			while(true)
			{
				var ev = syncMgr.PopEvent();
				if( ev == null ) break;
				Console.WriteLine($"{ev}");
			}
		}
	}
}
