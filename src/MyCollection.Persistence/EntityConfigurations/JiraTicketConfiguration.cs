using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyCollection.Domain.Entities;

namespace MyCollection.Persistence.EntityConfigurations;
internal class JiraTicketConfiguration : IEntityTypeConfiguration<JiraTicket>
{
    public void Configure(EntityTypeBuilder<JiraTicket> builder)
    {
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(t => t.UserId);
    }
}
