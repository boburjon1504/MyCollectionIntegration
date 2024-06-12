using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyCollection.Application.Interfaces;
using MyCollection.Domain.Entities;

namespace MyCollection.Web.Hubs;

public class CommentHub(ICommentService commentService,IUserService userService, ICollectionItemService collectionItemService) : Hub
{
    public async Task JoinToGroup(string userId,string groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
    }

    public async Task SendComment(string userName,string imgUrl,string groupId, string message,string date)
    {
        var user = await userService.GetByUserNameAsync(userName);
        var itemId = Guid.Parse(groupId.Trim());
        var newComment = new Comment
        {
            ItemId = itemId,
            SenderId = user.Id,
            Message = message,
            SendAt = DateTimeOffset.UtcNow
        };

        await collectionItemService
            .Get()
            .Where(c => c.Id == itemId)
            .ExecuteUpdateAsync(c => c.SetProperty(i => i.CommentsCount, s => s.CommentsCount + 1));

        await commentService.CreateAsync(newComment,saveChanges: true,cancellationToken: Context.ConnectionAborted);

        await Clients.Group(groupId).SendAsync("RecieveMessage",userName, imgUrl, message,DateTime.Parse(date).ToString("dd.MM.yyyy"));
    }
}
