using Synapse.HttpClientCls.Helpers;
using Synapse.Shared.Options;

namespace Synapse.Service.Tasking.HttpClients
{
    public class TaskingClsHttp : ClsHttp
    {
        public TaskingClsHttp(IdentityOptions identityOptions, string? clsEnvironmentUrl = null) : base(identityOptions, clsEnvironmentUrl)
        {
        }
    }
}
