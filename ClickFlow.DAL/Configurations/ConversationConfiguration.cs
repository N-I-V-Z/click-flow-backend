using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClickFlow.DAL.Configurations
{
	public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
	{
		public void Configure(EntityTypeBuilder<Conversation> builder)
		{
			builder.ToTable("Conversations");
			builder.HasKey(c => c.Id);
			builder.Property(c => c.Id).UseIdentityColumn();

			builder.Property(c => c.User1Id).IsRequired();
			builder.Property(c => c.User2Id).IsRequired();
			builder.Property(c => c.CreatedAt).IsRequired(false);

			builder.HasOne(c => c.User1)
				.WithMany(a => a.ConversationsAsUser1)
				.HasForeignKey(c => c.User1Id)
				.OnDelete(DeleteBehavior.Restrict);
			builder.HasOne(c => c.User2)
				.WithMany(c => c.ConversationsAsUser2)
				.HasForeignKey(c => c.User2Id)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
