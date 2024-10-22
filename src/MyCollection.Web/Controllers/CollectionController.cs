﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCollection.Application.Interfaces;
using MyCollection.Domain.Entities;
using MyCollection.Web.Helpers;
using MyCollection.Web.Models;

namespace MyCollection.Web.Controllers;
public class CollectionController(IUserService userService, 
    ICollectionService collectionService, 
    IImgService imgService, IContextRequest request) : Controller
{

    [HttpGet]
    public async ValueTask<IActionResult> GetCollection(string userName, string collectionName)
    {
        var user = await request.GetRequestedUserAsync();

        var collection = await collectionService.Get()
            .Include(c => c.Owner)
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Name.Equals(collectionName));

        ViewBag.Collection = collection;

        return View(new ModelForView
        {
            User = user,
            ProfileImg = new ProfileImg { UserId = user.Id }
        });
    }

    [HttpGet]
    public async ValueTask<IActionResult> Get()
    {
        var collections = await collectionService.Get().Include(c => c.Owner).ToListAsync();
        User user;

        if (User.Identity.IsAuthenticated)
        {
            user = await userService.GetByIdAsync(Guid.Parse(User.Claims.FirstOrDefault(c => c.Type.Equals("UserId")).Value));
        }
        else
        {
            user = new User();
        }

        ViewBag.Collections = collections;

        return View(new ModelForView
        {
            User = user,
            ProfileImg = new ProfileImg { UserId = user.Id }
        });
    }

    [Authorize]
    [HttpPost]
    public async ValueTask<IActionResult> Create(Collection collection)
    {
        collection.Id = Guid.NewGuid();

        collection.ImgPath = await imgService.SaveImgAsync(collection.ImgForm, collection.Id);
        var newCollection = await collectionService.CreateAsync(collection, saveChanges: true, cancellationToken: HttpContext.RequestAborted);

        User user = await userService.GetByIdAsync(Guid.Parse(User.Claims.FirstOrDefault(c => c.Type.Equals("UserId")).Value));

        ViewBag.Collections = newCollection;

        return RedirectToAction("Get");
    }

}
