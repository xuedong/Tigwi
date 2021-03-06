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
    using Tigwi.Storage.Library;
    using Tigwi.UI.Models;
    using Tigwi.UI.Models.Storage;

    public class PostController : HomeController
    {
        public PostController(IStorage storage)
            : base(storage)
        {
        }

        /// <summary>
        /// Tags (favorites) a post.
        /// </summary>
        /// <returns></returns>
        public ActionResult Tag(Guid postId)
        {
            var currentAccount = this.CurrentAccount;

            // Check for connection
            if (currentAccount == null)
            {
                // TODO: redirect to login/sign up page
                throw new Exception("Must be connected.");
            }
            throw new Exception("fuck");
            /*
            // Actually tag the post
            var post = this.Storage.Posts.Find(postId);
            post.TagBy(currentAccount);

            return this.View(post);*/
        }

        /// <summary>
        /// Untag (unfavorites) a post.
        /// </summary>
        /// <returns></returns>
        public ActionResult UnTag()
        {
            throw new NotImplementedException("PostController.UnTag");
        }

        /// <summary>
        /// Write a new post.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Write(WritePostViewModel post)
        {
            if (ModelState.IsValid)
            {
                this.Storage.Posts.Create(CurrentAccount, post.Content);
                this.Storage.SaveChanges();
                return this.RedirectToAction("Index","Home");

            }
            //
            throw new NotImplementedException(CurrentAccount.Name + "  " + post.Content);
            return this.View();
        }

        /// <summary>
        /// Deletes a post.
        /// </summary>
        /// <returns></returns>
        public ActionResult Delete()
        {
            throw new NotImplementedException("PostController.Delete");
        }

        /// <summary>
        /// Reply to a  post. The difference with <c>Write</c> is that it could be better integrated in the UI
        /// (see twitter).
        /// </summary>
        /// <returns></returns>
        public ActionResult Reply()
        {
            throw new NotImplementedException("PostController.Reply");
        }

        /// <summary>
        /// Repost something poster by another user (Retweets in Twitter).
        /// </summary>
        /// <returns></returns>
        /// TODO: Find another name. The term "Repost" has a negative connotation.
        public ActionResult Repost()
        {
            throw new NotImplementedException("PostController.Repost");
        }
    }
}
