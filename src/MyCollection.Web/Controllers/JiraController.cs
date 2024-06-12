using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCollection.Application.Interfaces;
using MyCollection.Domain.Entities;
using MyCollection.Web.Helpers;
using MyCollection.Web.HttpClientsServices;
using MyCollection.Web.Models;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace MyCollection.Web.Controllers;
public class JiraController(IJiraTicketService jiraTicketService,
    IJiraClientService jiraClient, IContextRequest request,
    IHttpClientFactory httpClientFactory,
    IUserService userService,
    ICollectionService collectionService) : Controller
{
    [Authorize]
    public async ValueTask<IActionResult> Create()
    {
        var user = await request.GetRequestedUserAsync();
        var collections = await collectionService
            .Get()
            .ToListAsync();
        ViewBag.collections = collections;

        return View(new ModelForView { User = user });
    }

    [Authorize]
    [HttpPost]
    public async ValueTask<IActionResult> Create(JiraTicket ticket)
    {
        var user = await request.GetRequestedUserAsync();
        if (user.JiraAccountId is null)
        {
            user.JiraAccountId = await jiraClient.CreateUserAccountAsync(user);
            await userService.UpdateAsync(user);
        }

        ticket.AssigneeId = user.JiraAccountId;
        var ticketUrl = await jiraClient.CreateJiraTicketAsync(ticket);
        ticket.Url = ticketUrl;

        var newTicket = await jiraTicketService.CreateAsync(ticket);

        return RedirectToAction("Get");
    }
    
    [Authorize]
    public async ValueTask<IActionResult> CreateJiraUser()
    {
        var user = await request.GetRequestedUserAsync(); 
        if (user.JiraAccountId is null)
        {
            user.JiraAccountId = await jiraClient.CreateUserAccountAsync(user);
            await userService.UpdateAsync(user);
        }
        return RedirectToAction("Get", "Jira");
    }
    public async ValueTask<IActionResult> Get(int currentPage = 1)
    {
        var ticket = await jiraTicketService.Get().Skip((currentPage - 1) * 10).Take(10).ToListAsync();

        ViewBag.Tickets = ticket;
        ViewBag.CurrentPage = currentPage;
        var count = await jiraTicketService.Get().CountAsync();
        var isThereAnyPage = count - currentPage * 10 > 0;
        ViewBag.IsThereAnyPreviuePage = currentPage > 1;
        ViewBag.IsThereAnyPage = isThereAnyPage;
        var user = await request.GetRequestedUserAsync();


        return View(new ModelForView { User = user });
    }
}
