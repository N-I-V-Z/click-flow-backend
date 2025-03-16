using ClickFlow.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.DTOs.CampaignDTOs
{
    public class CampaignUpdateDTO : CampaignCreateDTO
    {
        public int Id { get; set; }
    }
}

