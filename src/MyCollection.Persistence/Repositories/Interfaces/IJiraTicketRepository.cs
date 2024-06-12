using MyCollection.Domain.Entities;

namespace MyCollection.Persistence.Repositories.Interfaces;
public interface IJiraTicketRepository
{
    IQueryable<JiraTicket> Get(bool asNoTracking = true);

    ValueTask<JiraTicket?> GetByIdAsync(Guid id, bool asNoTracking = true, CancellationToken cancellationToken = default);

    ValueTask<JiraTicket> CreateAsync(JiraTicket jiraTicket, bool saveChanges = true, CancellationToken cancellationToken = default);
}
