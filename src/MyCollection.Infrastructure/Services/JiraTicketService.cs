using MyCollection.Application.Interfaces;
using MyCollection.Domain.Entities;
using MyCollection.Persistence.Repositories.Interfaces;

namespace MyCollection.Infrastructure.Services;
public class JiraTicketService(IJiraTicketRepository repository) : IJiraTicketService
{
    public IQueryable<JiraTicket> Get(bool asNoTracking = true)
    {
        return repository.Get(asNoTracking);
    }
    public ValueTask<JiraTicket?> GetByIdAsync(Guid id, bool asNoTracking = true, CancellationToken cancellationToken = default)
    {
        return repository.GetByIdAsync(id, asNoTracking, cancellationToken);
    }
    public ValueTask<JiraTicket> CreateAsync(JiraTicket jiraTicket, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        return repository.CreateAsync(jiraTicket, saveChanges, cancellationToken);
    }
}
