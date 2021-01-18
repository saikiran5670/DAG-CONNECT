using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace net.atos.daf.ct2.accountservice
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public IConfiguration Configuration { get; }
        public GreeterService(ILogger<GreeterService> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            try
            {
                return Task.FromResult(new HelloReply
                {
                    Message = "Hello " + request.Name
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new HelloReply
                {
                    Message = "Exception " + ex.Message
                });
            }
        }

        public override Task<ConnectionStringResponse> ConnectionStringKey(ConnectionStringRequest request, ServerCallContext context)
        {
            try
            {
                var connectionString = Configuration.GetConnectionString("ConnectionString");
                var response = new ConnectionStringResponse();
                response.Message = connectionString;

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ConnectionStringResponse
                {
                    Message = "Exception " + ex.Message
                });
            }
        }

        public override Task<ConnectionStringResponse> ConnectionStringHardCode(ConnectionStringRequest request, ServerCallContext context)
        {
            try
            {
                var connectionString = Configuration.GetConnectionString("ConnectionStringHardCode");
                var response = new ConnectionStringResponse();
                response.Message = connectionString;

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ConnectionStringResponse
                {
                    Message = "Exception " + ex.Message
                });
            }
        }

    }
}
