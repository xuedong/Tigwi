#region copyright
// Copyright (c) 2012, TIGWI
// All rights reserved.
// Distributed under  BSD 2-Clause license
#endregion
namespace Tigwi.UI.Models
{
    using System;

    /// <summary>
    /// The interface specification for a post model.
    /// </summary>
    public interface IPostModel
    {
        #region Public Properties

        /// <summary>
        /// Gets the post's content.
        /// </summary>
        string Content { get; }

        /// <summary>
        /// Gets the post's ID.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the DateTime at which the post was created.
        /// </summary>
        DateTime PostDate { get; }

        /// <summary>
        /// Gets the author of the post.
        /// </summary>
        IAccountModel Poster { get; }

        #endregion
    }
}