using System.Collections.Generic;

namespace SceneGraphSync
{
	public class NetCoupler<TNative, TExt> where TNative:class where TExt:class
	{
		Dictionary<TExt, TNative> _extToNative = new Dictionary<TExt, TNative>();
		Dictionary<TNative, TExt> _nativeToExt = new Dictionary<TNative, TExt>();

		public void Add( TNative nat, TExt ext )
		{
			_extToNative[ext] = nat;
			_nativeToExt[nat] = ext;
		}

		public TExt Remove(	TNative nat )
		{
			var ext = Find( nat );
			if( ext != null )
			{
				_extToNative.Remove( ext );
			}
			return ext;
		}

		public TExt Find( TNative nat )
		{
			if( nat == null ) return null;

			if( _nativeToExt.TryGetValue( nat, out var ext ) )
			{
				return ext;
			}
			return null;
		}

		public TNative Find( TExt ext ) 
		{
			if( ext == null ) return null;

			if( _extToNative.TryGetValue( ext, out var nat ) )
			{
				return nat;
			}
			return null;
		}

		public IEnumerable<TNative> GetNatives()
		{
			return _nativeToExt.Keys;
		}
	}
}
