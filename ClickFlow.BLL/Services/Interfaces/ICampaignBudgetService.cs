using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.Services.Interfaces
{
    public interface ICampaignBudgetService
    {
        Task CheckAndStopCampaignIfBudgetExceededAsync(int campaignId);
    }
}
