using ClickFlow.BLL.DTOs.EmailDTOs;
using ClickFlow.BLL.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.Services.Interfaces
{
    public interface IEmailService
    {
        public BaseResponse SendEmail(EmailDTO emailDTO);
    }
}
