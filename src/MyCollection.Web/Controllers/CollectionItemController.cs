using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCollection.Application.Interfaces;
using MyCollection.Domain.Entities;
using MyCollection.Infrastructure.Services;
using MyCollection.Web.Helpers;
using MyCollection.Web.Models;
using System.Security.Claims;

namespace MyCollection.Web.Controllers;
public class CollectionItemController(IUserService userService,
    ICollectionService collectionService,
    ICollectionItemService collectionItemService,
    IImgService imgService,
    IContextRequest request, ILikeService likeService) : Controller
{
    [HttpGet]
    public async ValueTask<IActionResult> GetCollectionItem(string userName, string collectionName, string itemName)
    {
        var user = await request.GetRequestedUserAsync();

        var item = await collectionItemService
            .Get()
            .Include(i => i.Collection)
            .Include(i => i.Owner)
            .Include(i => i.Likes)
            .Include(i => i.Comments)
            .ThenInclude(c => c.Sender)
            .FirstOrDefaultAsync(i => i.Name.Equals(itemName));

        ViewBag.Item = item;

        return View(new ModelForView
        {
            User = user,
            ProfileImg = new ProfileImg { UserId = user.Id }
        });
    }

    [HttpGet]
    public async ValueTask<IActionResult> Get()
    {
        var items = await collectionItemService.Get().Include(i => i.Owner).Include(i => i.Collection).ToListAsync();
        User user = await request.GetRequestedUserAsync();
        ViewBag.Items = items;

        return View(new ModelForView
        {
            User = user,
            ProfileImg = new ProfileImg { UserId = user.Id }
        });
    }
    [Authorize]
    [HttpPost]
    public async ValueTask<IActionResult> Create(CollectionItem collectionItem)
    {
        await collectionService
            .Get()
            .Where(c => c.Id == collectionItem.Id)
            .ExecuteUpdateAsync(c => c.SetProperty(i => i.ItemsCount, i => i.ItemsCount + 1));

        collectionItem.Id = Guid.NewGuid();

        collectionItem.ImgPath = await imgService.SaveImgAsync(collectionItem.ImgForm, collectionItem.Id);
        collectionItem.CreatedDate = DateTimeOffset.UtcNow;
        var newCollection = await collectionItemService.CreateAsync(collectionItem, saveChanges: true, cancellationToken: HttpContext.RequestAborted);

        var user = await request.GetRequestedUserAsync();

        var item = await collectionItemService
            .Get()
            .Include(i => i.Collection)
            .Include(i => i.Owner)
            .FirstOrDefaultAsync(i => i.Id == collectionItem.Id);

        return RedirectToAction("GetCollection", "Collection", new { userName = item.Owner.UserName, collectionName = item.Collection.Name });
    }
    public async ValueTask<IActionResult> LikeItem(Guid userId, Guid itemId)
    {
        var like = await likeService.Get().FirstOrDefaultAsync(l => l.UserId == userId && l.ItemId == itemId);

        if (like is null)
        {
            await collectionItemService
                .Get()
                .Where(c=>c.Id == itemId)
                .ExecuteUpdateAsync(c => c.SetProperty(i => i.LikesCount, s => s.LikesCount + 1));
            await likeService.CreateAsync(new Like { UserId = userId, ItemId = itemId });
        }
        else
        {
            await collectionItemService
                .Get()
                .Where(c => c.Id == itemId)
                .ExecuteUpdateAsync(c => c.SetProperty(i => i.LikesCount, s => s.LikesCount - 1));
            await likeService.DeleteAsync(like);
        }

        var count = await likeService.Get().CountAsync();

        return Ok(count);
    }
}

