using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;

namespace net.atos.daf.ct2.notificationservice.Services
{
    class GreeterImpl : Greeter.GreeterBase
    {
        // Server side handler of the SayHello RPC
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply { Message = "Hello " + request.Name });
        }
    }
}
