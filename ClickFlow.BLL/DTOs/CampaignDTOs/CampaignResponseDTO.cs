using ClickFlow.BLL.DTOs.AdvertiserDTOs;
using ClickFlow.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.DTOs.CampaignDTOs
{
    public class CampaignResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OriginURL { get; set; }
        public int Budget { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public TypePay TypePay { get; set; }
        public Industry TypeCampaign { get; set; }
        public CampaignStatus Status { get; set; }
        public int? Commission { get; set; }
        public int? Percents { get; set; }
        public string Image { get; set; }
        public int? AdvertiserId { get; set; }
        public AdvertiserResponseDTO? Advertiser { get; set; }
    }
}