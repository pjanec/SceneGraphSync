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


			var nm = new Net.Manager();
			var adapt = new NetAdaptor( nm );
			var n1 = adapt.CreateNode("1");
			//n.Name = "hi!";
			var o1 = adapt.GetNode( n1 ); 

			n1.Sync();
			n1.Children = new List<Node>();
			n1.Components.Add( adapt.CreateComponent("C1") );
			n1.Sync();
			n1.Children = new List<Node>() { adapt.CreateNode("2") };
			n1.Sync();
			o1.Children = new List<Net.Node>() { new Net.Node() { Name = "3" } };
			n1.Sync();

			// simulate node creation from a network; parented to node "1"
			var o2 = nm.CreateObject<Net.Node>();
			o2.Name = "4";
			o2.Parent = o1;
			var c2 = nm.CreateObject<Net.Component>();
			c2.Name = "C2";
			o2.Components.Add( c2 );
			Tick(adapt); // fires the net object creation callback - the native object should be created
			var n2 = adapt.GetNode( o2 ); 

			o1.Components.Clear();
			Tick(adapt); // component should be missing on n1

		}

		static void Tick(NetAdaptor adapt)
		{
			adapt.Tick();
			while(true)
			{
				var ev = adapt.PopEvent();
				if( ev == null ) break;
				if( ev is NetAdaptor.EventNodeCreated )	Console.WriteLine($"{ev.GetType().Name} {((NetAdaptor.EventNodeCreated)ev).Node.Name}");
				if( ev is NetAdaptor.EventNodeDestroyed ) Console.WriteLine($"{ev.GetType().Name} {((NetAdaptor.EventNodeDestroyed)ev).Node.Name}");
				if( ev is NetAdaptor.EventComponentCreated ) Console.WriteLine($"{ev.GetType().Name} {((NetAdaptor.EventComponentCreated)ev).Component.Name}");
				if( ev is NetAdaptor.EventComponentDestroyed ) Console.WriteLine($"{ev.GetType().Name} {((NetAdaptor.EventComponentDestroyed)ev).Component.Name}");
			}
		}
	}
}
