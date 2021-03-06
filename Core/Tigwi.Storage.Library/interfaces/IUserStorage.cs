﻿#region copyright
// Copyright (c) 2012, TIGWI
// All rights reserved.
// Distributed under  BSD 2-Clause license
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tigwi.Storage.Library
{
    public interface IUserStorage
    {
        /// <summary>
        /// Get a user id given its login
        /// </summary>
        /// <exception cref="UserNotFound"> if no user has this login</exception>
        Guid GetId(string login);

        /// <summary>
        /// get info related to the given user
        /// </summary>
        /// <exception cref="UserNotFound">if no user has this ID</exception>
        IUserInfo GetInfo(Guid userId);

        /// <summary>
        /// Set the infos related to the given user
        /// </summary>
        /// <exception cref="UserNotFound">if no user has this ID</exception>
        void SetInfo(Guid userId, string email, Guid mainAccountId);

        /// <summary>
        /// get the accounts where the user can post
        /// </summary>
        /// <exception cref="UserNotFound">if no user has this ID</exception>
        HashSet<Guid> GetAccounts(Guid userId);

        /// <summary>
        /// create a user
        /// </summary>
        /// <exception cref="UserAlreadyExists">if the login is already used</exception>
        Guid Create(string login, string email, Byte[] password);
       
        /// <summary>
        /// delete a user
        /// doesn't do anything if the user doesn't exists
        /// </summary>
        /// <exception cref="UserIsAdmin">To avoid deleting an account</exception>
        void Delete(Guid userId);

        /// <summary>
        /// Get a user id given an openid uri
        /// </summary>
        /// <exception cref="UserNotFound"> if no user has this openid uri</exception>
        Guid GetIdByOpenIdUri(string openIdUri);

        /// <summary>
        /// Associate an user to an openid uri
        /// </summary>
        /// <exception cref="UserNotFound"> if no user has this id</exception>
        /// <exception cref="OpenIdUriDuplicated"> if an user already has this openid uri</exception>
        void AssociateOpenIdUri(Guid userId, string openIdUri);

        /// <summary>
        /// List the openid uri associated to an user
        /// </summary>
        /// <exception cref="UserNotFound"> if not user has this id</exception>
        HashSet<string> ListOpenIdUris(Guid userId);

        /// <summary>
        /// Deassociate an openid uri from an user
        /// </summary>
        /// <exception cref="UserNotFound"> if no user has this id</exception>
        void DeassociateOpenIdUri(Guid userId, string openIdUri);

        /// <summary>
        /// Get a user id by api key
        /// </summary>
        /// <exception cref="UserNotFound"> if no user uses this api key</exception>
        Guid GetIdByApiKey(Guid apiKey);

        /// <summary>
        /// Create a new API key for the given user
        /// </summary>
        /// <exception cref="UserNotFound"> if no user has this id</exception>
        Guid GenerateApiKey(Guid userId, string applicationName);

        /// <summary>
        /// Lists the API keys associated to a given user
        /// </summary>
        /// <exception cref="UserNotFound"> if not user has this id</exception>
        Dictionary<Guid, string> ListApiKeys(Guid userId);

        /// <summary>
        /// Deactivate an API key
        /// </summary>
        /// <exception cref="UserNotFound"> if no user has this id</exception>
        void DeactivateApiKey(Guid userId, Guid apiKey);

        /// <summary>
        /// To be used to check a user password
        /// </summary>
        /// <exception cref="UserNotFound"></exception>
        Byte[] GetPassword(Guid userId);

        /// <summary>
        /// Change a user password
        /// </summary>
        /// <exception cref="UserNotFound"></exception>
        void SetPassword(Guid userId, Byte[] password);
    }
}
