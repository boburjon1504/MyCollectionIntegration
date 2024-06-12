using MyCollection.Domain.Entities;

namespace MyCollection.Application.Interfaces;
public interface IJiraTicketService
{
    IQueryable<JiraTicket> Get(bool asNoTracking = true);

    ValueTask<JiraTicket?> GetByIdAsync(Guid id, bool asNoTracking = true, CancellationToken cancellationToken = default);

    ValueTask<JiraTicket> CreateAsync(JiraTicket jiraTicket, bool saveChanges = true, CancellationToken cancellationToken = default);
}
