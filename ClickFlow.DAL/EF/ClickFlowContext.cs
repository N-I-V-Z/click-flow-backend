using ClickFlow.DAL.Configurations;
using ClickFlow.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClickFlow.DAL.EF
{
	public partial class ClickFlowContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
	{

		public ClickFlowContext(DbContextOptions<ClickFlowContext> options)
		: base(options)
		{

		}

		#region DbSet
		public DbSet<Advertiser> Advertisers { get; set; }
		public DbSet<Campaign> Campaigns { get; set; }
		public DbSet<Feedback> Feedbacks { get; set; }
		public DbSet<Publisher> Publishers { get; set; }
		public DbSet<CampaignParticipation> CampaignParticipations { get; set; }
		public DbSet<Traffic> Traffics { get; set; }
		public DbSet<Transaction> Transactions { get; set; }
		public DbSet<UserDetail> UserDetail { get; set; }
		public DbSet<Wallet> Wallets { get; set; }
		public DbSet<Report> Reports { get; set; }
		public DbSet<PaymentMethod> PaymentMethods { get; set; }
		public DbSet<Course> Courses { get; set; }
		public DbSet<CoursePublisher> CoursePublishers { get; set; }
		public DbSet<Video> Videos { get; set; }
		public DbSet<Post> Posts { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Like> Likes { get; set; }
		public DbSet<Conversation> Conversations { get; set; }
		public DbSet<Message> Messages { get; set; }
		public DbSet<Conversion> Conversions { get; set; }
		public DbSet<Plan> Plans { get; set; }
		public DbSet<UserPlan> UserPlans { get; set; }
		#endregion

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema("dbo");
			base.OnModelCreating(modelBuilder);
			modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
			modelBuilder.ApplyConfiguration(new AdvertiserConfiguration());
			modelBuilder.ApplyConfiguration(new CampaignConfiguration());
			modelBuilder.ApplyConfiguration(new FeedbackConfiguration());
			modelBuilder.ApplyConfiguration(new PaymentMethodConfiguration());
			modelBuilder.ApplyConfiguration(new PublisherConfiguration());
			modelBuilder.ApplyConfiguration(new CampaignParticipationConfiguration());
			modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
			modelBuilder.ApplyConfiguration(new ReportConfiguration());
			modelBuilder.ApplyConfiguration(new TrafficConfiguration());
			modelBuilder.ApplyConfiguration(new TransactionConfiguration());
			modelBuilder.ApplyConfiguration(new UserDetailConfiguration());
			modelBuilder.ApplyConfiguration(new WalletConfiguration());
			modelBuilder.ApplyConfiguration(new CourseConfiguration());
			modelBuilder.ApplyConfiguration(new CoursePublisherConfiguration());
			modelBuilder.ApplyConfiguration(new VideoConfiguration());
			modelBuilder.ApplyConfiguration(new PostConfiguration());
			modelBuilder.ApplyConfiguration(new CommentConfiguration());
			modelBuilder.ApplyConfiguration(new LikeConfiguration());
			modelBuilder.ApplyConfiguration(new ConversationConfiguration());
			modelBuilder.ApplyConfiguration(new MessageConfiguration());
			modelBuilder.ApplyConfiguration(new ConversionConfiguration());
			modelBuilder.ApplyConfiguration(new PlanConfiguration());
			modelBuilder.ApplyConfiguration(new UserPlanConfiguration());

			modelBuilder.Entity<IdentityUserLogin<int>>(entity =>
			{
				entity.ToTable("UserLogin");
				entity.HasKey(l => new { l.LoginProvider, l.ProviderKey });
			});

			modelBuilder.Entity<IdentityUserToken<int>>(entity =>
			{
				entity.ToTable("UserToken");
				entity.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
			});

			modelBuilder.Entity<IdentityRole<int>>(entity =>
			{
				entity.ToTable("Roles");
			});

			modelBuilder.Entity<IdentityUserRole<int>>(entity =>
			{
				entity.ToTable("UserRoles");
				entity.HasKey(ur => new { ur.UserId, ur.RoleId });
			});


		}
	}

}
