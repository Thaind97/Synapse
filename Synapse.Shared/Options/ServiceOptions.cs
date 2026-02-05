using Synapse.Shared.Types;

namespace Synapse.Shared.Options
{
    public class ServiceOptions
    {
        public ServiceConfig? IdentityService { get; set; }
        public ServiceConfig? ApiGatewayService { get; set; }
    }
}
