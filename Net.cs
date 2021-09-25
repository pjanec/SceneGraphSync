using System;
using System.Collections.Generic;

namespace Net
{
	public class Object
	{
		public UInt32 InstanceId { get; set; }
	}

	public class Node : Object
	{
		public string Name;
		public Node Parent;
		public List<Node> Children = new List<Node>();
		public List<Component> Components = new List<Component>();
	}

	public class Component : Object
	{
		public string Name;
		public string Content;
	}

	public class Manager
	{
		public T CreateObject<T>( UInt32 id ) where T:Object, new()
		{
			return new T() { InstanceId = id };
		}


		public void DestroyObject( Object obj )
		{
		}

		//public T FindObject<T>( UInt32 id ) where T:Object
		//{
		//	return null;
		//}
	}
}
