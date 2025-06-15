namespace ClickFlow.BLL.DTOs.PayOSDTOs
{
	public record PayOSWebhookResponse(
		int error,
		String message,
		object? data
	);
}
