using ClickFlow.BLL.DTOs;
using ClickFlow.BLL.DTOs.CampaignDTOs;
using ClickFlow.BLL.DTOs.TrafficDTOs;
using ClickFlow.BLL.Helpers.Config;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace ClickFlow.BLL.Services.Implements
{
	public class AIWebsiteService : IAIWebsiteService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly AIWebsiteConfiguration _aiWebsiteConfiguration;
		private readonly IUserDetailService _userDetailService;
		private readonly IWalletService _walletService;
		private readonly ICampaignService _campaignService;

		public AIWebsiteService(
			AIWebsiteConfiguration aIWebsiteConfiguration,
			IHttpClientFactory httpClientFactory,
			IUserDetailService userDetailService,
			IWalletService walletService,
			ICampaignService campaignService)
		{
			_httpClientFactory = httpClientFactory;
			_aiWebsiteConfiguration = aIWebsiteConfiguration;
			_userDetailService = userDetailService;
			_walletService = walletService;
			_campaignService = campaignService;
		}

		public async Task<string> GetWebsiteAIResponseAsync(int userId, string question)
		{
			var userInfo = await GetUserContextAsync(userId);

			var prompt = $@"Bạn là AI Assistant chuyên nghiệp của ClickFlow - nền tảng affiliate network hàng đầu Việt Nam.

=== THÔNG TIN USER ===
{userInfo}

=== CÂU HỎI ===
{question}

=== VỀ CLICKFLOW ===
ClickFlow là nền tảng Affiliate Network thông minh kết nối Advertiser & Publisher với AI tiên tiến.

TÍNH NĂNG CHÍNH:
1. Quản Lý Chiến Dịch: Tạo/quản lý campaigns, theo dõi click/conversion realtime, dashboard analytics, báo cáo tự động, lấy link/tạo sub ID
2. Traffic & Analytics: Thống kê chi tiết từ Publisher, phân tích theo nguồn/thiết bị/địa lý/thời gian, tracking minh bạch chống gian lận
3. Diễn đàn Cộng đồng: Tương tác Advertiser-Publisher, chia sẻ kinh nghiệm, Q&A, thông báo ưu đãi
4. Tin tức & Xu hướng: Cập nhật affiliate marketing, phân tích hành vi user, insights ngành
5. Khóa học & Tài liệu: Học liệu cơ bản→nâng cao, case study, tips tối ưu, video tutorials

HƯỚNG DẪN SỬ DỤNG:
🔹 Tham gia Campaign: CHIẾN DỊCH → Chọn campaign → ĐĂNG KÝ NGAY → theo dõi traffic/lấy link
🔹 Tạo Campaign: TỔNG QUAN → TẠO MỚI CHIẾN DỊCH → điền thông tin → chờ duyệt
🔹 Nạp tiền: Hồ sơ → Ví cá nhân → Nạp tiền → quét QR
🔹 Rút tiền: Hồ sơ → Ví cá nhân → Rút tiền (cập nhật info ngân hàng trước, 24h xử lý)
🔹 Theo dõi: Mục TRUY CẬP xem traffic, Dashboard analytics theo dõi KPIs/ROI

ĐỐI TƯỢNG: 
📌 PUBLISHER: Kiếm tiền từ quảng bá sản phẩm
📌 ADVERTISER: Tìm partner mở rộng thị trường

ĐỐI TÁC CHÍNH:
1. CarePaws
 Nền tảng đặt dịch vụ chăm sóc thú cưng và đăng ký khám thú y.
 🔗 Website: https://carepaws.site/
2. Perhue
 Nền tảng xác định tông màu cá nhân, hỗ trợ người dùng hiểu rõ màu sắc phù hợp với ngoại hình và cá tính của mình.
 🔗 Website: https://perhue.io.vn/
3. CodeGrow
 CodeGrow là nền tảng học lập trình trực tuyến tích hợp công nghệ AI để cá nhân hóa lộ trình học. Nền tảng kết nối người học với mentor, đồng thời cung cấp môi trường thực hành qua các dự án mô phỏng.
 🔗 Website: https://codegrow.vercel.app/customer
4. Artline
 Nền tảng quản lý quy trình sản xuất dành riêng cho ngành sáng tạo. Artline hợp nhất toàn bộ quy trình sản xuất—đặc biệt là giai đoạn tiền sản xuất—thành một không gian số trực quan, linh hoạt và dễ quản lý.
 🔗 Website: https://artline-creative.com/
5. E-Bridge
 Website quản lý sự kiện dành cho sinh viên, giúp việc tổ chức và tham gia các hoạt động học tập, ngoại khóa trở nên dễ dàng. Với giao diện đơn giản, E-Bridge kết nối các bạn trẻ, tạo cơ hội giao lưu và phát triển kỹ năng.
 🔗 Website: https://ebridge.vn/

ƯU ĐIỂM: Thu nhập không giới hạn, hỗ trợ 24/7, đa dạng campaigns, AI tracking chính xác, giao diện thân thiện, cộng đồng năng động

LIÊN HỆ: Hotline +84 852 279 3879 | Email clickflow.connect@gmail.com

QUY TẮC TRẢ LỜI:
1. Cá nhân hóa theo thông tin user (role/mục tiêu)
2. Trả lời ngắn gọn 2-3 câu, chính xác, thân thiện
3. Dùng emoji/bullet points khi cần, đề xuất next steps
4. Không liên quan ClickFlow/affiliate → 'Xin lỗi, tôi chỉ hỗ trợ các vấn đề liên quan đến ClickFlow và Affiliate Marketing.'
5. Thông tin nâng cao thêm: '(Thông tin từ AI ClickFlow - vui lòng kiểm tra chi tiết quan trọng)'
6. Chỉ show thông tin user khi được hỏi, xưng bạn-tôi";

			return await SendRequestToAI(prompt);
		}

		private async Task<string> GetUserContextAsync(int userId)
		{
			try
			{
				var user = await _userDetailService.GetUserDetailByUserId(userId);
				if (user == null) return "Người dùng mới, chưa có thông tin chi tiết.";

				var context = new StringBuilder();

				// Thông tin cơ bản
				context.AppendLine($"- Tên: {user.User.FullName ?? "Chưa cập nhật"}");
				context.AppendLine($"- Email: {user.User.Email}");
				context.AppendLine($"- Role: {GetUserRoleText(user.User.Role)}");

				// Thông tin tài chính
				var wallet = await _walletService.GetWalletByUserIdAsync(userId);
				context.AppendLine($"- Số dư ví: {wallet.Balance:N0} VNĐ");

				if (user.User.Role == Role.Publisher)
				{
					var campaigns = await _campaignService.GetCampaignsJoinedByPublisher(userId, 1, 5);
					var campaignDatas = new PagingDTO<CampaignResponseDTO>(campaigns);
					if (campaignDatas.Datas.Any())
					{
						var campaignInfo = string.Join(", ", campaignDatas.Datas.Select(x =>
							$"{x.Name} ({x.TypePay}, {x.Commission?.ToString() ?? x.Percents + "%"})"));
						context.AppendLine($"- Campaigns tham gia: {campaignInfo}");
					}
				}
				else if (user.User.Role == Role.Advertiser)
				{
					var campaigns = await _campaignService.GetCampaignsByAdvertiserId(userId, CampaignStatus.Activing, 1, 5);
					var campaignDatas = new PagingDTO<CampaignResponseDTO>(campaigns);
					if (campaignDatas.Datas.Any())
					{
						var campaignInfo = string.Join(", ", campaignDatas.Datas.Select(x =>
							$"{x.Name} ({x.TypePay}, {x.Commission?.ToString() ?? x.Percents + "%"})"));
						context.AppendLine($"- Campaigns đang chạy: {campaignInfo}");
					}
				}

				return context.ToString();
			}
			catch (Exception ex)
			{
				return "Không thể lấy thông tin chi tiết của người dùng.";
			}
		}

		private string GetUserRoleText(Role? userRole)
		{
			return userRole switch
			{
				Role.Publisher => "Publisher (Nhà tiếp thị)",
				Role.Advertiser => "Advertiser (Nhà quảng cáo)",
				_ => "Chưa xác định role"
			};
		}

		private async Task<string> SendRequestToAI(string prompt)
		{
			var requestBody = new
			{
				contents = new[] {
					new {
						parts = new object[] {
							new { text = prompt }
						}
					}
				},
				generationConfig = new
				{
					temperature = 0.7,
					topK = 40,
					topP = 0.95,
					maxOutputTokens = 1024,
				}
			};

			string apiUrl = $"{_aiWebsiteConfiguration.ApiLink}{_aiWebsiteConfiguration.ApiKey}";
			var httpClient = _httpClientFactory.CreateClient();
			httpClient.Timeout = TimeSpan.FromSeconds(30);

			var content = new StringContent(
				JsonConvert.SerializeObject(requestBody),
				Encoding.UTF8,
				"application/json");

			try
			{
				HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

				if (!response.IsSuccessStatusCode)
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					throw new Exception($"API request failed: {response.StatusCode} - {errorContent}");
				}

				string responseContent = await response.Content.ReadAsStringAsync();
				JObject jsonResponse = JObject.Parse(responseContent);
				string generatedText = jsonResponse["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

				if (string.IsNullOrEmpty(generatedText))
				{
					return "AI đang gặp sự cố. Liên hệ Hotline: +84 852 279 3879";
				}

				return generatedText;
			}
			catch (JsonException)
			{
				return "Lỗi xử lý phản hồi từ AI. Vui lòng thử lại.";
			}
			catch (TaskCanceledException)
			{
				return "AI phản hồi chậm, thử lại sau ít phút.";
			}
			catch
			{
				return "AI tạm thời không khả dụng. Liên hệ Support.";
			}
		}
	}
}