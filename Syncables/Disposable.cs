using System;
using System.Collections.Generic;
using System.Text;

namespace Syncables
{
    public class Disposable : IDisposable
    {
        public Disposable()
        {
        }

        ~Disposable()
        {
            Dispose(false);
        }

        protected virtual void Dispose( bool disposing )
        {

        }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                if( !JustDisposing )
                {
                    JustDisposing = true;
                    Dispose(true);
                    IsDisposed = true;
                }
            }
        }

		public bool JustDisposing { get; private set; }


    }
}
