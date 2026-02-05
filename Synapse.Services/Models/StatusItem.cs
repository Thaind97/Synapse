using System.Collections.Generic;

namespace Synapse.Services.Models
{
    public class StatusItem
    {
        public string Title { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public List<StatusDetail> Details { get; set; } = new();
    }
}
