using System;
using System.Collections.Generic;

namespace SceneGraphSync
{
	public class Component : Syncables.Syncable, IEquatable<Component>
	{
		public string Name { get; set; }
		public string Content { get; set; }	// json

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
