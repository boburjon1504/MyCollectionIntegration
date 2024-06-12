using MyCollection.Domain.Entities;
namespace MyCollection.Web.HttpClientsServices;

public interface IJiraClientService
{
    ValueTask<string> CreateUserAccountAsync(User user);
    ValueTask<string> CreateJiraTicketAsync(JiraTicket ticket);
}
