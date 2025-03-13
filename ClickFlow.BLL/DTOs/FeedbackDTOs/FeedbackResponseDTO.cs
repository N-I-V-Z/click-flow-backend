using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.DTOs.FeedbackDTOs
{
    public class FeedbackResponseDTO
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int StarRate { get; set; }
        public DateTime Timestamp { get; set; }
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public int FeedbackerId { get; set; }
        public string FeedbackerName { get; set; }
    }
}
