using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.DTOs.PayOSDTOs
{
	public record ConfirmWebhookDTO(
		string webhook_url
	);
}
