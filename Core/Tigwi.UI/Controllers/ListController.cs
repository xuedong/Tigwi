﻿#region copyright
// Copyright (c) 2012, TIGWI
// All rights reserved.
// Distributed under  BSD 2-Clause license
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tigwi.UI.Controllers
{
    using System.Net;

    using Tigwi.Storage.Library;
    using Tigwi.UI.Models;

    public class ListController : HomeController
    {
        public ListController(IStorage storage)
            : base(storage)
        {
        }

        /// <summary>
        /// Shows the messages posted in the list.
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        public ActionResult Details(Guid listId)
        {
            var list = this.Storage.Lists.Find(listId);

            return this.View(list);
        }

        /// <summary>
        /// Show the members of a list.
        /// </summary>
        /// <param name="listName"></param>
        /// <returns></returns>
        public ActionResult Members(string listName)
        {
            throw new NotImplementedException("ListController.Members");
        }

        /// <summary>
        /// Show an interface for the creation of a new list associated with the active account.
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            throw new NotImplementedException("ListController.Create");
        }

        /// <summary>
        /// Actually creates or edit the list.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(EditListViewModel editList, int edit)
        {
            if (editList.AccountIds == null)
            {
                ModelState.AddModelError("Members", "The list must contain at least one member.");
                return this.View("_EditListModal", editList);
            }
            else
            {
                // TODO: This is NOT correct. There should be *TWO* distinct methods Edit and Create.
                //TODO : Check whether or not currentAccount is authorized to edit this list
                IListModel list = null;
                list = edit == 0
                           ? this.Storage.Lists.Create(
                               this.CurrentAccount, editList.ListName, editList.ListDescription, !editList.ListPublic)
                           : this.Storage.Lists.Find(editList.ListId);
                if (list == null)
                    throw new HttpException((int)HttpStatusCode.NotFound, "The list doesn't exists.");
                try
                {
                    list.Name = editList.ListName;
                    list.Description = editList.ListDescription;
                    list.IsPrivate = !editList.ListPublic;
                    list.Members.Clear();
                    foreach (var member in editList.AccountIds)
                    {
                        IAccountModel account = this.Storage.Accounts.Find(member);
                        list.Members.Add(account);
                    }
                    this.Storage.SaveChanges();
                    return this.RedirectToAction("Index", "Home");
                }
                catch (Models.Storage.AccountNotFoundException ex)
                {
                    if (edit == 0)
                        this.Storage.Lists.Delete(list);
                    throw new HttpException((int)HttpStatusCode.NotFound, ex.Message);
                }
                catch (Tigwi.Storage.Library.IsPersonnalList ex)
                {
                    return this.RedirectToAction("Index", "Home", new { error = ex.Message });
                }
            }
        }

        /// <summary>
        /// Makes the active account follow the given list.
        /// Idempotent.
        /// </summary>
        /// <returns>The resulting view.</returns>
        [HttpPost]
        public ActionResult Follow(Guid id)
        {
            var list = this.Storage.Lists.Find(id);
            CurrentAccount.AllFollowedLists.Add(list);
            this.Storage.SaveChanges();
            return this.RedirectToAction("Details", "List", new { listId = id });
        }

        /// <summary>
        /// Makes the active account follow the given list.
        /// Idempotent.
        /// </summary>
        /// <returns>Json object.</returns>
        [HttpPost]
        public ActionResult FollowList(Guid id)
        {
            var list = this.Storage.Lists.Find(id);
            CurrentAccount.AllFollowedLists.Add(list);
            this.Storage.SaveChanges();
            return Json(new {Name=list.Name});
        }

        /// <summary>
        /// Stops the active account from following list id
        /// Idempotent.
        /// </summary>
        /// <returns>The resulting view.</returns>
        [HttpPost]
        public ActionResult UnfollowList(Guid id)
        {
            var list = this.Storage.Lists.Find(id);
            CurrentAccount.AllFollowedLists.Remove(list);
            this.Storage.SaveChanges();
            return Json(new { Name = list.Name });
        }

        /// <summary>
        /// delete a list
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteList(Guid id)
        {
            //TODO check whether or not it all went according to plan ...
            var list = this.Storage.Lists.Find(id);
            if (list.Owner.Id == CurrentAccount.Id)
            {
                try
                {
                    this.Storage.Lists.Delete(this.Storage.Lists.Find(id));
                }
                catch (Tigwi.Storage.Library.IsPersonnalList ex)
                {
                    return this.RedirectToAction("Index", "Home", new { error = ex.Message });
                }
            }
            return this.RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult GetList(Guid listId)
        {
            var list = this.CurrentAccount.AllFollowedLists.First(l => l.Id == listId);
            if (list == null)
            {
                return this.RedirectToAction("Index", "Home", new { error = "This list does not exist anymore." });
            }

            return
                Json(
                    new
                        {
                            Name = list.Name,
                            Descr = list.Description,
                            Public = !list.IsPrivate,
                            Members = list.Members.Select(account => account.Name)
                        });
        }
    }
}
