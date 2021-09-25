using System;
using System.Linq;
using System.Collections.Generic;

namespace Syncables
{
	public class ExtSyncList<T> : ISyncList<T>	 where T:IEquatable<T>
	{
		public ExtSyncList( Func<List<T>> readInt, Action<List<T>> writeInt, Func<List<T>> readExt, Action<List<T>> writeExt, Action<List<T>> onExtChanged )
		{
			_readInt = readInt;
			_writeInt = writeInt;

			_readExt = readExt;
			_writeExt = writeExt;

			OnExtChanged = onExtChanged;
		}
		
		public Action<List<T>> OnExtChanged { get; set; }
		
		public void Sync()
		{
			// read from internal source
			var curr = _readInt();
			if( !Identical( _prev, curr ) )
			{
				// write to ext source
				_writeExt( curr );

				_prev = curr.ToList(); // shallow clone
			}
			else
			{
				// read from external source
				curr = _readExt();

				// changed from ext source?
				if( !Identical( _prev, curr ) )
				{
					if( OnExtChanged != null )
						OnExtChanged( curr );

					// write to internal
					_writeInt( curr );
					_prev = curr.ToList();
				}
			}
		}

		static bool Identical( IEnumerable<T> baseline, IEnumerable<T> current )
		{
			if( baseline == null && current == null ) return true;
			if( baseline == null && current != null ) return false;
			if( baseline != null && current == null ) return false;
			return Enumerable.SequenceEqual( baseline, current );
		}


		private Func<List<T>> _readInt;
		private Action<List<T>> _writeInt;
		private Func<List<T>> _readExt;
		private Action<List<T>> _writeExt;

		private List<T> _prev;
	}
}
