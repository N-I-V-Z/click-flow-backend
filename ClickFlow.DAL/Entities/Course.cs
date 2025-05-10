namespace ClickFlow.DAL.Entities
{
	public class Course
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public DateTime? CreateAt { get; set; }
		public DateTime? UpdateAt { get; set; }
		public int CreateById { get; set; }
		public ApplicationUser CreateBy { get; set; }
		public int Price { get; set; }
		public string LessonLearned { get; set; }
		public string Content { get; set; }
		public string Description { get; set; }
		public ICollection<CoursePublisher>? CoursePublishers { get; set; }
		public ICollection<Video>? Videos { get; set; }
	}
}