using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace net.atos.daf.ct2.authenticationservice
{
    public class GreeterTestService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterTestService> _logger;
        public GreeterTestService(ILogger<GreeterTestService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}
