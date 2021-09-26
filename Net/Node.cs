using System.Collections.Generic;

namespace Net
{
	public class Node : Object
	{
		public string Name;
		public Node Parent;
		public List<Node> Children = new List<Node>();
		public List<Component> Components = new List<Component>();
	}
}
