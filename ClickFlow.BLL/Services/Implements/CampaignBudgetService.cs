using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.Services.Implements
{
    public class CampaignBudgetService : ICampaignBudgetService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CampaignBudgetService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CheckAndStopCampaignIfBudgetExceededAsync(int campaignId)
        {
            var campaignRepo = _unitOfWork.GetRepo<Campaign>();
            var campaign = await campaignRepo.GetSingleAsync(new QueryBuilder<Campaign>()
                .WithPredicate(c => c.Id == campaignId)
                .Build());

            if (campaign != null && campaign.RemainingBudget <= 0)
            {
                campaign.Status = CampaignStatus.Completed;
                await campaignRepo.UpdateAsync(campaign);
                await _unitOfWork.SaveChangesAsync();
            }
        }

    }
}
