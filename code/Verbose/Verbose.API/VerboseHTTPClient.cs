using System;
using System.Net.Http;

namespace Verbose.API
{
    public abstract class VerboseHTTPClient : IDisposable
    {
        // For locking so we don't make multiple clients
        private static object _resourceLock = new object();
        // The HTTP client itself
        private static volatile HttpClient _client;

        protected static HttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    lock (_resourceLock)
                    {
                        if (_client == null)
                        {
                            // Make a client if we don't have one already
                            _client = new HttpClient();
                        }
                    }
                }

                return _client;
            }
        }

        /// <summary>
        /// For disposing of the HTTPClient
        /// </summary>
        public void Dispose()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_client != null)
                {
                    _client.Dispose();
                }

                _client = null;
            }
        }
    }
}
