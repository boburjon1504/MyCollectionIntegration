using Microsoft.EntityFrameworkCore;
using MyCollection.Domain.Entities;
using MyCollection.Persistence.DataContext;
using MyCollection.Persistence.Repositories.Interfaces;

namespace MyCollection.Persistence.Repositories;
public class JiraTicketRepository(CollectionDbContext dbContext) : EntityBaseRepository<JiraTicket>(dbContext), IJiraTicketRepository
{
    public IQueryable<JiraTicket> Get(bool asNoTracking = true)
    {
        var initialQuery = DbContext.Tickets;

        if (asNoTracking)
            initialQuery.AsNoTracking();

        return initialQuery;
    }

    public ValueTask<JiraTicket?> GetByIdAsync(Guid id, bool asNoTracking = true, CancellationToken cancellationToken = default)
    {
        return base.GetByIdAsync(id, cancellationToken);
    }

    public new ValueTask<JiraTicket> CreateAsync(JiraTicket jiraTicket, bool saveChanges, CancellationToken cancellationToken)
    {
        return base.CreateAsync(jiraTicket, saveChanges, cancellationToken);
    }
}
