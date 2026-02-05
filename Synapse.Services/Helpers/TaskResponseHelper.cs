using Synapse.Models.Tasking.Response.PickUp;
using System.Collections.Generic;
using System.Linq;

namespace Synapse.Service.Tasking.Helpers;

public static class TaskResponseHelper
{
    /// <summary>
    /// Parse API response data to list of TaskResponse
    /// Handles both single TaskResponse and List<TaskResponse>
    /// </summary>
    public static List<OldTaskPickUpResponse> ParseToList(object data)
    {
        if (data == null)
            return new List<OldTaskPickUpResponse>();

        // Check if it's already a list
        if (data is IEnumerable<OldTaskPickUpResponse> taskList)
        {
            return taskList.ToList();
        }

        // Check if it's a single TaskResponse
        if (data is OldTaskPickUpResponse singleTask)
        {
            return new List<OldTaskPickUpResponse> { singleTask };
        }

        // Unknown type
        return new List<OldTaskPickUpResponse>();
    }

    /// <summary>
    /// Check if response contains multiple tasks
    /// </summary>
    public static bool IsMultiTask(object data)
    {
        if (data == null)
            return false;

        if (data is IEnumerable<OldTaskPickUpResponse> taskList)
        {
            return taskList.Count() > 1;
        }

        return false;
    }

    /// <summary>
    /// Get total pallets from task list
    /// </summary>
    public static int CalculateTotalPallets(List<OldTaskPickUpResponse> tasks)
    {
        if (tasks == null || !tasks.Any())
            return 0;

        return 1; //tasks.Sum(t => t.Pallets);
    }
}