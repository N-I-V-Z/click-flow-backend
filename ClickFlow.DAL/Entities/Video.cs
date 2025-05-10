namespace ClickFlow.DAL.Entities
{
	public class Video
	{
		public int Id { get; set; }
		public string Link { get; set; }
		public string Title { get; set; }
		public string Duration { get; set; }
		public int CourseId { get; set; }
		public Course Course { get; set; }
		public ICollection<QuestionAndAnswer> QuestionAndAnswers { get; set; }
	}
}
