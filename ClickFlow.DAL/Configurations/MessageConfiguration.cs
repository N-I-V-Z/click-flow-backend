using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClickFlow.DAL.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).UseIdentityColumn();

            builder.Property(m => m.ConversationId).IsRequired();
            builder.Property(m => m.SenderId).IsRequired();
            builder.Property(m => m.Text).IsRequired(false);
            builder.Property(m => m.Type).IsRequired();
            builder.Property(m => m.FileUrl).IsRequired(false);
            builder.Property(m => m.SentAt).IsRequired(false);
            builder.Property(m => m.IsRead).HasDefaultValue(false);

            builder.HasOne(m => m.Conversation)
                .WithMany(a => a.Messages)
                .HasForeignKey(m => m.ConversationId);
            builder.HasOne(m => m.Sender)
                .WithMany(a => a.Messages)
                .HasForeignKey(m => m.SenderId);
        }
    }
}
