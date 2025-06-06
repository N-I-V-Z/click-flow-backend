using ClickFlow.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.DTOs.CampaignDTOs
{
    public class CampaignResponseForPublisherDTO : CampaignResponseDTO
    {
        public CampaignParticipationStatus? PublisherStatus { get; set; } 
    }
}
