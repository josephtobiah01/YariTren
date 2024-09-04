using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Core.Interfaces
{
    public interface IApiManager
    {
        Task<T> SendForResponseAsync<T>(string uriRequest, HttpMethod method, HttpContent content, CancellationToken cancellationToken = default);
        Task<T> PostForResponseAsync<T>(string uriRequest, HttpContent content, CancellationToken cancellationToken = default);
        Task<T> GetForResponseAsync<T>(string uriRequest, CancellationToken cancellationToken = default);
        Task<T> PutForResponseAsync<T>(string uriRequest, HttpContent content, CancellationToken cancellationToken = default);
    }
}
