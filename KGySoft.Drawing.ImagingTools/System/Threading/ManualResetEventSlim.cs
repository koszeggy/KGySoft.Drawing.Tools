#if NET35

// ReSharper disable once CheckNamespace
namespace System.Threading
{
    internal sealed class ManualResetEventSlim : IDisposable
    {
        #region Fields

        private readonly object syncRoot = new object();

        private bool isDisposed;

        #endregion
        
        #region Properties
        
        internal bool IsSet { get; private set; }

        #endregion

        #region Methods

        #region Public Methods

        public void Dispose()
        {
            lock (syncRoot)
            {
                if (isDisposed)
                    return;
                isDisposed = true;
                IsSet = true;
                Monitor.PulseAll(syncRoot);
            }
        }

        #endregion

        #region Internal Methods

        internal void Set()
        {
            lock (syncRoot)
            {
                if (isDisposed)
                    return;
                IsSet = true;
                Monitor.PulseAll(syncRoot);
            }
        }

        internal void Wait()
        {
            lock (syncRoot)
            {
                if (isDisposed)
                    return;
                while (!IsSet)
                    Monitor.Wait(syncRoot);
            }
        }

        #endregion

        #endregion
    }
}

#endif