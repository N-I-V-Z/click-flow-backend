using ClickFlow.BLL.Helpers.Config;
using ClickFlow.BLL.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace ClickFlow.BLL.Services.Implements
{
	public class AIWebsiteService : IAIWebsiteService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly AIWebsiteConfiguration _aiWebsiteConfiguration;
		public AIWebsiteService(AIWebsiteConfiguration aIWebsiteConfiguration, IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
			_aiWebsiteConfiguration = aIWebsiteConfiguration;
		}
		public async Task<string> GetWebsiteAIResponseAsync(string question)
		{
			var websiteInfo = @"
ClickFlow là một nền tảng Affiliate Network thông minh, kết nối Advertiser & Publisher với công nghệ AI tiên tiến. Website hỗ trợ các tính năng chính như:

1. Quản Lý Chiến Dịch Thông Minh
   - Tạo và quản lý campaigns với đa dạng gói dịch vụ
   - Theo dõi click, conversion và doanh thu 
   - Dashboard analytics chi tiết với biểu đồ trực quan
   - Hệ thống báo cáo tự động và xuất dữ liệu
   - Lấy link, tạo sub ID để theo dõi hiệu quả từng kênh (Facebook, Tiktok, blog,…). Theo dõi số click, đơn hàng và thu nhập.

2. Theo dõi traffic
   - Thống kê lượt click, conversion đến từ từng publisher, nguồn traffic, thiết bị, khu vực địa lý.
   - Hệ thống tracking minh bạch, hỗ trợ chống gian lận.

3. Diễn đàn cộng đồng
   - Kênh tương tác hai chiều giữa advertiser và publisher: chia sẻ mẹo chạy chiến dịch hiệu quả, giải đáp câu hỏi, thông báo ưu đãi nhanh chóng.

4. Tin tức & Thị trường
   - Cập nhật xu hướng thị trường affiliate, hành vi người dùng, chỉ số ngành.
   - Hỗ trợ advertiser tối ưu chiến lược.

5. Khóa học & Hướng dẫn
   - Kho học liệu về affiliate marketing từ cơ bản đến nâng cao.
   - Cung cấp tips, case study và hướng dẫn tối ưu thu nhập.
   - Phù hợp với cả người mới bắt đầu.
Chỉ trả lời câu hỏi, không hỏi role người dùng.
- Xem thông tin chiến dịch ở mục chiến dịch, tại đây bấm ĐĂNG KÝ NGAY nếu bạn muốn tham gia chiến dịch này. 
- Sau khi tham gia chiến dịch có thể theo dõi traffic, lấy link affiliate hay gửi feedback (vào phần chiến dịch, click vào chiến dịch để xem những phần nội dung đó).
- Nạp tiền: vào phần Hồ sơ -> Ví cá nhân -> Nạp tiền -> Chọn số tiền mà bạn muốn thanh toán, tại đây bạn sẽ được chuyển đến trang thanh toán quét QR để hoàn tất giao dịch.
- Rút tiền: vào phần Hồ sơ -> Ví cá nhân -> Rút tiền -> Chọn số tiền mà bạn muốn rút. Lưu ý bạn phải cập nhật thông tin ngân hàng trước khi rút tiền, số tiền sẽ được chuyển vào tài khoản của bạn trong 24 giờ kể từ khi thực hiện rút tiền. Nếu có thắc mắc hoặc sai sót hãy liên hệ qua hotline hoặc email.
- Tạo mới chiến dịch tại TỔNG QUAN, tại đây bên phải góc trên cùng có nút TẠO MỚI CHIẾN DỊCH. Hãy click vào, điền các trường thông tin liên quan để tạo mới chiến dịch. Lưu ý chiến dịch tạo xong sẽ đợi admin kiểm tra và phê duyệt.
- Xem chiến dịch tại mục CHIẾN DỊCH
- Xem traffic tại mục TRUY CẬP
- Mục DIỄN ĐÀN để xem diễn đàn
- Nạp tiền: vào phần Hồ sơ -> Ví cá nhân -> Nạp tiền -> Chọn số tiền mà bạn muốn thanh toán, tại đây bạn sẽ được chuyển đến trang thanh toán quét QR để hoàn tất giao dịch.

Đối tượng người dùng:
- Nhà tiếp thị (PUBLISHER)
- Nhà quảng cáo (ADVERTISER)

Trang web hỗ trợ trải nghiệm thân thiện, giao diện mới hiện đại.  
Số hotline: +84 852 279 3879  
Email liên hệ: clickflow.connect@gmail.com

Tại sao nên tham gia Click Flow?
- Thu nhập không giới hạn: Với các chiến dịch đa dạng và mức hoa hồng hấp dẫn, bạn có thể tự do mở rộng thu nhập theo khả năng của mình.
- Hỗ trợ chuyên nghiệp: Đội ngũ chuyên gia luôn sẵn sàng hỗ trợ bạn với các chiến lược tiếp thị hiệu quả và công cụ tối ưu hóa chiến dịch.
- Đa dạng chiến dịch: Lựa chọn từ hàng trăm chiến dịch của các ngành hàng khác nhau, phù hợp với sở thích và đối tượng khách hàng mục tiêu của bạn.

Affiliate Marketing là gì?
Affiliate Marketing là hình thức tiếp thị dựa trên hiệu quả, nơi các nhà phân phối (affiliates) quảng bá sản phẩm/dịch vụ của nhà cung cấp và nhận hoa hồng khi có giao dịch thành công.

Chính Sách Bảo Mật

Thông tin chúng tôi thu thập:
- Thông tin cá nhân: Họ tên, email, số điện thoại, địa chỉ.
- Thông tin tài khoản: Giao dịch, mã giới thiệu, hiệu suất chiến dịch.
- Dữ liệu kỹ thuật: IP, thiết bị, cookie, trình duyệt.
- Dữ liệu tương tác: Hành vi duyệt web, thời gian, click.

Cách chúng tôi sử dụng thông tin:
- Cải thiện dịch vụ và hiệu quả chiến dịch.
- Phân tích hành vi người dùng.
- Gửi thông báo, ưu đãi (nếu đăng ký nhận).
- Ngăn chặn gian lận và bảo mật thông tin.

Chia sẻ thông tin:
- Chúng tôi không bán thông tin cá nhân, nhưng có thể chia sẻ:
  - Với đối tác giúp vận hành dịch vụ.
  - Khi có yêu cầu từ pháp luật.
  - Cho mục đích phân tích, nghiên cứu.

Nếu câu hỏi không liên quan đến website affiliate network, thanh toán hoặc hệ thống ClickFlow, hãy trả lời:  
'Xin lỗi, tôi chỉ có thể hỗ trợ các câu hỏi liên quan đến nền tảng affiliate network ClickFlow.'
";

			var prompt = $@"
Bạn là một AI hỗ trợ khách hàng cho nền tảng affiliate network ClickFlow.

Câu hỏi từ người dùng:
{question}

Thông tin về website:
{websiteInfo}

Hãy trả lời ngắn gọn, chính xác và thân thiện. Nếu câu hỏi không liên quan đến dịch vụ ClickFlow, hãy từ chối trả lời một cách lịch sự. Bạn có thể trả lời không cần bám sát 100% câu từ tôi đưa nhưng phải trên nguyên tắc đúng theo ý nghĩa. Các câu hỏi liên quan khái niệm về affiliate network vẫn được trả lời. Có thể tư vấn cho người dùng về affiliate marketing về việc xây kênh chạy chiến dịch như thế nào nếu họ là Publisher, còn Advertiser bạn có thể tư vấn cho họ miễn là chủ đề liên quan đến affiliate. Nếu thông tin ngoài không liên quan cái tôi đưa bạn nhưng nó liên quan affiliate thì bạn phải kèm theo đây là thông tin của trợ lí AI ClickFlow cung cấp thông tin có thể sai. Kiểm tra kĩ những thông tin quan trọng. Hãy xem xét ở góc độ tư vấn
";

			return await SendRequestToAI(prompt);
		}

		private async Task<string> SendRequestToAI(string prompt)
		{
			var requestBody = new
			{
				contents = new[] { new { parts = new object[] { new { text = prompt } } } }
			};

			string apiUrl = $"{_aiWebsiteConfiguration.ApiLink}{_aiWebsiteConfiguration.ApiKey}";
			var httpClient = _httpClientFactory.CreateClient();
			var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

			HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"API request failed: {response.StatusCode}");
			}

			string responseContent = await response.Content.ReadAsStringAsync();
			try
			{
				JObject jsonResponse = JObject.Parse(responseContent);
				string generatedText = jsonResponse["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
				return generatedText;
			}
			catch (JsonException jEx)
			{
				return "Lỗi xử lý phản hồi từ AI.";
			}
		}
	}
}
