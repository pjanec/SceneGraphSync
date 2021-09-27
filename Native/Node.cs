using System;
using System.Collections.Generic;

namespace SceneGraphSync
{

	public class Node : Syncables.Syncable, IEquatable<Node>
	{
		public string Name = string.Empty;
		public List<Node> Children { get; set; }
		public Node Parent { get; set; }
		public List<Component> Components { get; set; }

		public Node()
		{
		}

		public Node( string name )
		{
			Name = name;
		}

		public bool Equals( Node other )
		{
			return Object.ReferenceEquals(this, other);
		}

		public override string ToString()
		{
			return $"{Name}";
		}
	}
}
