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
			// Lấy thông tin user để cá nhân hóa phản hồi
			var userInfo = await GetUserContextAsync(userId);

			var websiteInfo = @"
ClickFlow là một nền tảng Affiliate Network thông minh, kết nối Advertiser & Publisher với công nghệ AI tiên tiến.

=== TÍNH NĂNG CHÍNH ===

1. Quản Lý Chiến Dịch Thông Minh
   - Tạo và quản lý Campaigns với đa dạng gói dịch vụ
   - Theo dõi click, conversion và doanh thu realtime
   - Dashboard analytics chi tiết với biểu đồ trực quan
   - Hệ thống báo cáo tự động và xuất dữ liệu
   - Lấy link, tạo sub ID để theo dõi hiệu quả từng kênh (Facebook, TikTok, blog, website, YouTube...)

2. Theo dõi Traffic & Analytics
   - Thống kê chi tiết: lượt click, conversion từ từng Publisher
   - Phân tích theo nguồn traffic, thiết bị, khu vực địa lý, thời gian
   - Hệ thống tracking minh bạch, chống gian lận
   - Báo cáo hiệu suất theo thời gian thực

3. Diễn đàn Cộng đồng
   - Tương tác giữa Advertiser và Publisher
   - Chia sẻ kinh nghiệm, mẹo chạy chiến dịch hiệu quả
   - Q&A, giải đáp thắc mắc
   - Thông báo ưu đãi, cập nhật nhanh chóng

4. Tin tức & Xu hướng Thị trường
   - Cập nhật xu hướng Affiliate Marketing mới nhất
   - Phân tích hành vi người dùng, insights ngành
   - Hỗ trợ tối ưu chiến lược marketing

5. Khóa học & Tài liệu Hướng dẫn
   - Kho học liệu từ cơ bản đến nâng cao
   - Case study thực tế, tips tối ưu thu nhập
   - Hướng dẫn chi tiết cho người mới bắt đầu
   - Video tutorials, webinars định kỳ

=== HƯỚNG DẪN SỬ DỤNG ===

🔹 Xem & Tham gia Chiến dịch:
   - Vào mục CHIẾN DỊCH → Chọn chiến dịch phù hợp → Click ĐĂNG KÝ NGAY
   - Sau khi tham gia: theo dõi traffic, lấy link affiliate, gửi feedback

🔹 Tạo Chiến dịch mới (Advertiser):
   - TỔNG QUAN → TẠO MỚI CHIẾN DỊCH (góc phải trên)
   - Điền đầy đủ thông tin → Chờ admin phê duyệt

🔹 Quản lý Tài chính:
   • Nạp tiền: Hồ sơ → Ví cá nhân → Nạp tiền → Chọn số tiền → Quét QR thanh toán
   • Rút tiền: Hồ sơ → Ví cá nhân → Rút tiền → Nhập số tiền
     (Lưu ý: Cập nhật thông tin ngân hàng trước. Tiền về tài khoản trong 24h)

🔹 Theo dõi Hiệu suất:
   - Mục TRUY CẬP: xem traffic chi tiết
   - Dashboard analytics: theo dõi KPIs, ROI

=== ĐỐI TƯỢNG NGƯỜI DÙNG ===
📌 PUBLISHER (Nhà tiếp thị): Kiếm tiền từ việc quảng bá sản phẩm
📌 ADVERTISER (Nhà quảng cáo): Tìm partner để mở rộng thị trường

=== LIÊN HỆ HỖ TRỢ ===
📞 Hotline: +84 852 279 3879
📧 Email: clickflow.connect@gmail.com
🕐 Hỗ trợ 24/7

=== TẠI SAO CHỌN CLICKFLOW? ===
✅ Thu nhập không giới hạn với hoa hồng hấp dẫn
✅ Hỗ trợ chuyên nghiệp từ đội ngũ experts
✅ Đa dạng chiến dịch từ nhiều ngành hàng
✅ Công nghệ AI tiên tiến, tracking chính xác
✅ Giao diện thân thiện, dễ sử dụng
✅ Cộng đồng năng động, chia sẻ kinh nghiệm

=== AFFILIATE MARKETING LÀ GÌ? ===
Là mô hình tiếp thị hiệu quả, nơi affiliates quảng bá sản phẩm/dịch vụ và nhận hoa hồng từ mỗi giao dịch thành công. Mô hình win-win cho tất cả các bên.

=== BẢO MẬT THÔNG TIN ===
🔒 Thu thập: Thông tin cá nhân, tài khoản, dữ liệu kỹ thuật, hành vi tương tác
🔒 Sử dụng: Cải thiện dịch vụ, phân tích, gửi thông báo, bảo mật
🔒 Chia sẻ: Chỉ với đối tác tin cậy, theo yêu cầu pháp luật
";

			var prompt = $@"
Bạn là AI Assistant chuyên nghiệp của ClickFlow - nền tảng affiliate network hàng đầu Việt Nam.

=== THÔNG TIN NGƯỜI DÙNG ===
{userInfo}

=== CÂU HỎI ===
{question}

=== THÔNG TIN NỀN TẢNG ===
{websiteInfo}

=== HƯỚNG DẪN TRẢ LỜI ===
1. 🎯 Ưu tiên cá nhân hóa dựa trên thông tin user (role, mục tiêu)
2. 💡 Trả lời ngắn gọn, chính xác, thân thiện và actionable
3. 📋 Sử dụng bullet points, emoji để dễ đọc khi cần thiết
4. 🔄 Nếu có thể, đề xuất next steps phù hợp với user
5. ❌ Nếu câu hỏi không liên quan ClickFlow/affiliate: 'Xin lỗi, tôi chỉ hỗ trợ các vấn đề liên quan đến ClickFlow và Affiliate Marketing.'
6. ⚠️ Với thông tin ngoài kiến thức cơ bản về affiliate, thêm disclaimer: '(Thông tin từ AI ClickFlow - vui lòng kiểm tra lại các chi tiết quan trọng)'

=== CÁC ĐỐI TÁC CHÍNH ===
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

=== PHẠM VI HỖ TRỢ ===
✅ Tất cả tính năng ClickFlow
✅ Hướng dẫn sử dụng platform
✅ Chiến lược affiliate marketing
✅ Tối ưu hiệu suất campaigns
✅ Xây dựng kênh traffic (Publisher)
✅ Quản lý chiến dịch (Advertiser)
✅ Vấn đề thanh toán, tài chính
✅ Troubleshooting kỹ thuật cơ bản

Hãy trả lời theo góc độ consultant chuyên nghiệp, tập trung vào giá trị thực tế cho User!
";

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
					// Một số chiến dịch đang tham gia
					var campaigns = await _campaignService.GetCampaignsJoinedByPublisher(userId, 1, 10);
					var campaignDatas = new PagingDTO<CampaignResponseDTO>(campaigns);
					var campaignInfo = string.Join(", ", campaignDatas.Datas.Select(x =>
						$"Campaign Name: {x.Name}, Loại: {x.TypePay}, Hoa hồng: {x.Commission?.ToString() ?? x.Percents + "%"}"));
					context.AppendLine($"- Một số chiến dịch đang tham gia: {campaignInfo}");
				}
				else if (user.User.Role == Role.Advertiser)
				{
					// Một số chiến dịch đang chạy
					var campaigns = await _campaignService.GetCampaignsByAdvertiserId(userId, CampaignStatus.Activing, 1, 10);
					var campaignDatas = new PagingDTO<CampaignResponseDTO>(campaigns);
					var campaignInfo = string.Join(", ", campaignDatas.Datas.Select(x =>
						$"Campaign Name: {x.Name}, Loại: {x.TypePay}, Hoa hồng: {x.Commission?.ToString() ?? x.Percents + "%"}")); 
					context.AppendLine($"- Một số chiến dịch đang được chạy: {campaignInfo}");
				}

				return context.ToString();
			}
			catch (Exception ex)
			{
				throw new Exception($"Không lấy được thông tin chi tiết của người dùng.");
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

			// Set timeout
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
					return "Xin lỗi, hiện tại AI đang gặp sự cố. Vui lòng thử lại sau hoặc liên hệ Hotline: +84 852 279 3879";
				}

				return generatedText;
			}
			catch (JsonException)
			{
				return "Lỗi xử lý phản hồi từ AI. Vui lòng thử lại sau.";
			}
			catch (TaskCanceledException)
			{
				return "AI đang phản hồi chậm, vui lòng thử lại sau ít phút.";
			}
			catch (Exception ex)
			{
				return "Hệ thống AI tạm thời không khả dụng. Vui lòng liên hệ Support để được hỗ trợ trực tiếp.";
			}
		}
	}
}