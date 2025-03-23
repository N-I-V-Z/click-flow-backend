using ClickFlow.BLL.DTOs.PublisherDTOs;
using ClickFlow.BLL.DTOs.UserDetailDTOs;
using ClickFlow.DAL.Entities;
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
        //public Campaign? Campaign { get; set; }
        public int FeedbackerId { get; set; }
        public PublisherResponseDTO? Feedbacker { get; set; }

        public UserDetailResponseDTO? AvatarURL { get; set; }
    }
}
