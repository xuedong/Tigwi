﻿#region copyright
// Copyright (c) 2012, TIGWI
// All rights reserved.
// Distributed under  BSD 2-Clause license
#endregion
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tigwi.API.Models;

namespace Tigwi.API.Controllers
{
    public class AccountController : ApiController
    {

        //
        // GET: /account/messages/{accountName}/{number}
        // GET: /account/messages/name={accountName}/{number}
        // GET: /account/messages/id={accountId}/{number}
        public ActionResult Messages(string accountName, Guid? accountId, int number)
        {
            Answer output;

            try
            {
                if (accountId == null && accountName == null)
                {
                    output = new Answer(new Error("Account missing"));
                    Response.StatusCode = 400; // Bad Request
                }
                else
                {
                    var realId = accountId ?? Storage.Account.GetId(accountName);

                    // get lasts messages from account accoutName
                    var personalListId = Storage.List.GetPersonalList(realId);
                    var listMsgs = Storage.Msg.GetListsMsgTo(new HashSet<Guid> {personalListId}, DateTime.Now, number);

                    // convert, looking forward XML serialization
                    var listMsgsOutput = new Messages(listMsgs, Storage);
                    output = new Answer(listMsgsOutput);
                }
            }

            catch (Exception exception)
            {
                // Result is an non-empty error XML element
                output = new Answer(HandleError(exception));
            }

            return Serialize(output);
        }

        
        //
        // GET: /account/taggedmessages/{accountName}/{number}
        // GET: /account/taggedmessages/name={accountName}/{number}
        // GET: /account/taggedmessages/id={accountId}/{number}
        public ActionResult TaggedMessages(string accountName, Guid? accountId, int number)
        {
            Answer output;

            try
            {
                if (accountId == null && accountName == null)
                {
                    output = new Answer(new Error("Account missing"));
                    Response.StatusCode = 400; // Bad Request
                }
                else
                {
                    var realId = accountId ?? Storage.Account.GetId(accountName);

                    // check if the user is authenticated and has rights
                    var authentication = Authorized(realId);
                    if (authentication.HasRights)
                    {
                        // get lasts messages from user name
                        var listMsgs = Storage.Msg.GetTaggedTo(realId, DateTime.Now, number);

                        // convert, looking forward XML serialization
                        var listMsgsOutput = new Messages(listMsgs, Storage);
                        output = new Answer(listMsgsOutput);
                    }
                    else
                        output = new Answer(new Error(authentication.ErrorMessage()));
                }
            }

            catch (Exception exception)
            {
                // Result is an non-empty error XML element
                output = new Answer(HandleError(exception));
            }

            return Serialize(output);
        }


        //
        // GET : /account/subscriberaccounts/{accountName}/{number}
        // GET: /account/subscriberaccounts/name={accountName}/{number}
        // GET: /account/subscriberaccounts/id={accountId}/{number}
        public ActionResult SubscriberAccounts(string accountName, Guid? accountId, int number)
        {
            Answer output;

            try
            {
                if (accountId == null && accountName == null)
                {
                    output = new Answer(new Error("Account missing"));
                    Response.StatusCode = 400; // Bad Request
                }
                else
                {
                    var realId = accountId ?? Storage.Account.GetId(accountName);

                    // get lasts followers of user name 's list
                    var followingLists = Storage.List.GetFollowingLists(realId);
                    var hashFollowers = new HashSet<Guid>();
                    foreach (var followingList in followingLists)
                    {
                        hashFollowers.UnionWith(Storage.List.GetFollowingAccounts(followingList));
                    }

                    // Get as many subscribers as possible (maximum: number)
                    var size = Math.Min(hashFollowers.Count, number);
                    var accountListToReturn = AccountsFromGuidCollection(hashFollowers, size, Storage);

                    output = new Answer(accountListToReturn);
                }
            }
            catch (Exception exception)
            {
                // Result is an non-empty error XML element
                output = new Answer(HandleError(exception));
            }

            return Serialize(output);
        }

        
        //
        // GET : /account/subscribedaccounts/{accountName}/{number}
        // GET: /account/subscribedaccounts/name={accountName}/{number}
        // GET: /account/subscribedaccounts/id={accountId}/{number}
        public ActionResult SubscribedAccounts(string accountName, Guid? accountId, int number)
        {
            Answer output;

            try
            {
                if (accountId == null && accountName == null)
                {
                    output = new Answer(new Error("Account missing"));
                    Response.StatusCode = 400; // Bad Request
                }
                else
                {
                    var realId = accountId ?? Storage.Account.GetId(accountName);

                    // we check if the user is authenticated and authorized to know whether to show private lists
                    var authentication = Authorized(realId);
                    if (authentication.Failed)
                        output = new Answer(new Error(authentication.ErrorMessage()));
                    else
                    {
                        // get the public lists followed by the given account
                        var followedLists = Storage.List.GetAccountFollowedLists(realId, authentication.HasRights);
                        var accountsInLists = new HashSet<Guid>();
                        foreach (var followedList in followedLists)
                        {
                            accountsInLists.UnionWith(Storage.List.GetAccounts(followedList));
                        }

                        // Get as many subscriptions as possible (maximum: numberOfSubscriptions)
                        var size = Math.Min(accountsInLists.Count, number);
                        var accountListToReturn = AccountsFromGuidCollection(accountsInLists, size, Storage);

                        output = new Answer(accountListToReturn);
                    }
                }
            }
            catch (Exception exception)
            {
                // Result is an non-empty error XML element
                output = new Answer(HandleError(exception));
            }

            return Serialize(output);
        }


        //
        // GET : /account/subscribedlists/{accountName}/{number}
        // GET: /account/subscribedlists/name={accountName}/{number}
        // GET: /account/subscribedlists/id={accountId}/{number}
        public ActionResult SubscribedLists(string accountName, Guid? accountId, int number)
        {
            Answer output;

            try
            {
                if (accountId == null && accountName == null)
                {
                    output = new Answer(new Error("Account missing"));
                    Response.StatusCode = 400; // Bad Request
                }
                else
                {
                    var realId = accountId ?? Storage.Account.GetId(accountName);

                    // we check if the user is authenticated and authorized to know whether to show private lists
                    var authentication = Authorized(realId);
                    if (authentication.Failed)
                        output = new Answer(new Error(authentication.ErrorMessage()));
                    else
                    {
                        // get the public lists followed by the given account
                        var followedLists = Storage.List.GetAccountFollowedLists(realId, authentication.HasRights);

                        // Get as many subscriptions as possible (maximum: numberOfSubscriptions)
                        var size = Math.Min(followedLists.Count, number);
                        var listsToReturn = ListsFromGuidCollection(followedLists, size, Storage);

                        output = new Answer(listsToReturn);
                    }
                }
            }
            catch (Exception exception)
            {
                // Result is an non-empty error XML element
                output = new Answer(HandleError(exception));
            }

            return Serialize(output);
        }


        //
        // GET : /account/subscriberlists/{accountName}/{number}
        // GET: /account/subscriberlists/name={accountName}/{number}
        // GET: /account/subscriberlists/id={accountId}/{number}
        public ActionResult SubscriberLists(string accountName, Guid? accountId, int number)
        {
            Answer output;

            try
            {
                if (accountId == null && accountName == null)
                {
                    output = new Answer(new Error("Account missing"));
                    Response.StatusCode = 400; // Bad Request
                }
                else
                {
                    var realId = accountId ?? Storage.Account.GetId(accountName);

                    // get lasts followers of user name 's list
                    var followingLists = Storage.List.GetFollowingLists(realId);

                    // Get as many subscribers as possible (maximum: number)
                    var size = Math.Min(followingLists.Count, number);
                    var accountListToReturn = AccountsFromGuidCollection(followingLists, size, Storage);

                    output = new Answer(accountListToReturn);
                }
            }
            catch (Exception exception)
            {
                // Result is an non-empty error XML element
                output = new Answer(HandleError(exception));
            }

            return Serialize(output);
        }


        //
        // GET : /account/ownedlists/{accountName}/{number}
        // GET: /account/ownedlists/name={accountName}/{number}
        // GET: /account/ownedlists/id={accountId}/{number}
        public ActionResult OwnedLists(string accountName, Guid? accountId, int number)
        {
            Answer output;

            try
            {
                if (accountId == null && accountName == null)
                {
                    output = new Answer(new Error("Account missing"));
                    Response.StatusCode = 400; // Bad Request
                }
                else
                {
                    var realId = accountId ?? Storage.Account.GetId(accountName);

                    // we check if the user is authenticated and authorized to know whether to show private lists
                    var authentication = Authorized(realId);
                    if (authentication.Failed)
                        output = new Answer(new Error(authentication.ErrorMessage()));
                    else
                    {
                        // get the public lists owned by the given account
                        var ownedLists = Storage.List.GetAccountOwnedLists(realId, authentication.HasRights);

                        // Get as many subscriptions as possible (maximum: numberOfSubscriptions)
                        var size = Math.Min(ownedLists.Count, number);
                        var listsToReturn = ListsFromGuidCollection(ownedLists, size, Storage);

                        output = new Answer(listsToReturn);
                    }
                }
            }
            catch (Exception exception)
            {
                // Result is an non-empty error XML element
                output = new Answer(HandleError(exception));
            }

            return Serialize(output);
        }


        //
        // GET : /account/maininfo/{accountName}
        // GET: /account/maininfo/name={accountName}
        // GET: /account/maininfo/id={accountId}
        public ActionResult MainInfo(string accountName, Guid? accountId)
        {
            Answer output;

            try
            {
                if (accountId == null && accountName == null)
                {
                    output = new Answer(new Error("Account missing"));
                    Response.StatusCode = 400; // Bad Request
                }
                else
                {
                    var realId = accountId ?? Storage.Account.GetId(accountName);

                    // get the informations of the given account
                    var accountInfo = Storage.Account.GetInfo(realId);
                    var accountToReturn = new Account(realId, accountInfo.Name, accountInfo.Description);
                    output = new Answer(accountToReturn);
                }
            }
            catch (Exception exception)
            {
                // Result is an non-empty error XML element
                output = new Answer(HandleError(exception));
            }

            return Serialize(output);
        }

    }
}
