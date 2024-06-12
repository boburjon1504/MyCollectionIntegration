using MyCollection.Domain.Common.Auditables;
using MyCollection.Domain.Enums;

namespace MyCollection.Domain.Entities;
public class JiraTicket : Entity
{
    public string Url { get; set; } = default!;

    public TicketStatus Status { get; set; } = TicketStatus.Opened;

    public Guid UserId  { get; set; }

    public string? AssigneeId { get; set; }

    public string? Collection { get; set; }

    public string Summary { get; set; }

    public string? Link { get; set; }

    public string Description { get; set; }
}
