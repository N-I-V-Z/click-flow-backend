namespace ClickFlow.DAL.Entities
{
    public class CoursePublisher
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; }
        public int? Rate { get; set; }
    }
}
