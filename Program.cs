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


			var bm = new Net.Manager();
			var bf = new NetFactory( bm );
			var n = bf.CreateNode("1");
			//n.Name = "hi!";
			var o = bf.GetExtNode( n ); 

			n.Sync();
			n.Children = new List<Node>();
			n.Sync();
			n.Children = new List<Node>() { bf.CreateNode("2") };
			n.Sync();
			o.Children = new List<Net.Node>() { new Net.Node() { Name = "3" } };
			n.Sync();

		}
	}
}
