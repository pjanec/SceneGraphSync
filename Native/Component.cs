using System;
using System.Collections.Generic;

namespace SceneGraphSync
{
	public class Component : Syncables.Syncable, IEquatable<Component>
	{
		public string Name = string.Empty;
		public string Content = string.Empty;	// json

		public Component()
		{
		}

		public Component( string name )
		{
			Name = name;
		}

		public bool Equals( Component other )
		{
			return Object.ReferenceEquals(this, other);
		}

		public override string ToString()
		{
			return $"{Name}";
		}
	}
}
