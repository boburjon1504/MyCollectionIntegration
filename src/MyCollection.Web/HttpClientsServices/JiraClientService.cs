using Microsoft.Extensions.Options;
using MyCollection.Domain.Common.Settings;
using MyCollection.Domain.Entities;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace MyCollection.Web.HttpClientsServices;

public class JiraClientService : IJiraClientService
{
    private readonly JiraSettings _settings;

    private readonly HttpClient _httpClient;
    public JiraClientService(IOptions<JiraSettings> options, HttpClient httpClient)
    {
        _settings = options.Value;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri($"{_settings.JiraInstance}");
        var authenticationString = $"{_settings.UserName}:{_settings.Token}";
        var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    }
    public async ValueTask<string> CreateJiraTicketAsync(JiraTicket ticket)
    {
        var requestBody = GetTicket(ticket);
        var response = await SendRequestAsync("rest/api/3/issue", requestBody);

        if (response.IsSuccessStatusCode)
        {
            string issueUrl = await GetTicketUrlAsync(response);

            Console.WriteLine("Created Jira ticket URL: " + issueUrl);

            return issueUrl;
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return errorMessage;
        }
    }

    private async Task<HttpResponseMessage> SendRequestAsync(string url, object requestBody)
    {
        var jsonPayload = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        return response;
    }

    private async Task<string> GetTicketUrlAsync(HttpResponseMessage response)
    {
        var responseBody = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<dynamic>(responseBody);

        var issueKey = responseObject.key;
        var issueUrl = $"{_settings.JiraInstance}/browse/{issueKey}";
        return issueUrl;
    }

    private static object GetTicket(JiraTicket ticket)
    {
        return new
        {
            fields = new
            {
                project = new { key = "KAN" },
                summary = ticket.Summary,
                description = new
                {
                    type = "doc",
                    version = 1,
                    content = new[]
                           {
                    new
                    {
                        type = "paragraph",
                        content = new[]
                        {
                            new
                            {
                                type = "text",
                                text = ticket.Description,
                            }
                        }
                    }
                }
                },
                issuetype = new { name = "Task" },
                assignee = new { id = ticket.AssigneeId },
                reporter = new { id = ticket.AssigneeId },
                customfield_10034 = "https://boburs-collection-2.azurewebsites.net/jira/get"
            }
        };
    }

    public async ValueTask<string> CreateUserAccountAsync(User user)
    {
        var url = "rest/api/3/user";
        var requestBody = GetJiraUserBody(user);
        var response = await SendRequestAsync(url, requestBody);
        var result = await response.Content.ReadAsStringAsync();

        var createdUser = JsonConvert.DeserializeObject<dynamic>(result);
        string accountId = createdUser.accountId;

        return accountId;
    }

    private static object GetJiraUserBody(User user)
    {
        return new
        {
            password = $"{user.Email}",
            emailAddress = $"{user.Email}",
            displayName = $"{user.FirstName} {user.LastName}",
            name = $"{user.UserName}",
            products = new List<string> { "jira-software" }
        };
    }
}
