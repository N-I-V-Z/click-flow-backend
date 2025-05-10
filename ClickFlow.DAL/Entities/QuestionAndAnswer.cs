namespace ClickFlow.DAL.Entities
{
	public class QuestionAndAnswer
	{
		public int Id { get; set; }
		public string Question { get; set; }
		public string Answer { get; set; }
		public DateTime? Timestamp { get; set; }
		public int VideoId { get; set; }
		public int PublisherId { get; set; }
		public Video Video { get; set; }
		public Publisher Publisher { get; set; }
	}
}
