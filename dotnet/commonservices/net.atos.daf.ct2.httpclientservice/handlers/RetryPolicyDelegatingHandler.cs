﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.httpclientservice.handlers
{
    public class RetryPolicyDelegatingHandler : DelegatingHandler
    {
        private readonly int _maximumAmountOfRetries = 3;

        public RetryPolicyDelegatingHandler(int maximumAmountOfRetries)
            : base()
        {
            _maximumAmountOfRetries = maximumAmountOfRetries;
        }

        public RetryPolicyDelegatingHandler(HttpMessageHandler innerHandler,
          int maximumAmountOfRetries)
      : base(innerHandler)
        {
            _maximumAmountOfRetries = maximumAmountOfRetries;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            for (int i = 0; i < _maximumAmountOfRetries; i++)
            {
                response = await base.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
            }
            return response;
        }
    }
}
