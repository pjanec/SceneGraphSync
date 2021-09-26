using System;

namespace Syncables
{
	public class PrimSyncer<T> : IPrimSyncer<T>	 where T:IEquatable<T>
	{
		public PrimSyncer( Func<T> readInt, Action<T> writeInt, Func<T> readExt, Action<T> writeExt, Action<T> onExtChanged )
		{
			_readInt = readInt;
			_writeInt = writeInt;

			_readExt = readExt;
			_writeExt = writeExt;

			OnExtChanged = onExtChanged;
		}
		
		public Action<T> OnExtChanged { get; set; }
		
		public void Sync()
		{
			// read from internal source
			var curr = _readInt();
			if( !Identical( _prev, curr ) )
			{
				// write to ext source
				_writeExt( curr );

				_prev = curr;
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
					_prev = curr;
				}
			}
		}


		static bool Identical( T baseline, T current )
		{
			return !((current == null && baseline != null) || (current!=null && !current.Equals( baseline )));
		}

		private Func<T> _readInt;
		private Action<T> _writeInt;
		private Func<T> _readExt;
		private Action<T> _writeExt;

		private T _prev;
	}
}
