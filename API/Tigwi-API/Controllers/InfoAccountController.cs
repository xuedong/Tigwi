﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Xml.Serialization;
using StorageLibrary;
using Tigwi_API.Models;

namespace Tigwi_API.Controllers
{
    public class InfoAccountController : ApiController
    {

        //
        // GET: /infoaccount/messages/{accountName}/{numberOfMessages}

        public ActionResult Messages(string accountName, int numberOfMessages)
        {
            // TODO : give the actual connexion informations
            IStorage storage = new StorageTmp(); // connexion
            ContentResult result;

            try
            {
                // get lasts messages from user name
                var accountId = storage.Account.GetId(accountName);
                var personalListId = storage.List.GetPersonalList(accountId);
                var listMsgs = storage.Msg.GetListsMsgTo(new HashSet<Guid> { personalListId }, DateTime.Now, numberOfMessages);

                // convert, looking forward XML serialization
                var listMsgsOutput = new Messages(listMsgs, storage);

                // a stream is needed for serialization
                var stream = new MemoryStream();

                (new XmlSerializer(typeof(Messages))).Serialize(stream, listMsgsOutput);

                result = Content(stream.ToString());
            }
            catch (StorageLibException exception)
            {
                // Result is an non-empty error XML element
                var stream = new MemoryStream();
                (new XmlSerializer(typeof(Error))).Serialize(stream, new Error(exception.Code.ToString()));
                result = Content(stream.ToString());
            }

            return result;
        }

        //
        // GET : /infoaccount/subscribers/{accountName}/{numberOfSubscribers}

        public ActionResult Subscribers(string accountName, int numberOfSubscribers)
        {
            IStorage storage = new StorageTmp(); // connexion
            ContentResult result;

            try
            {
                var accountId = storage.Account.GetId(accountName);

                // get lasts followers of user name 's list
                var followingLists = storage.List.GetFollowingLists(accountId);
                var hashFollowers = new HashSet<Guid>();
                foreach (var followingList in followingLists)
                {
                    hashFollowers.UnionWith(storage.List.GetFollowingAccounts(followingList));
                }

                // Get as many subscribers as possible (maximum: numberOfSubscibers)
                var size = Math.Min(hashFollowers.Count, numberOfSubscribers);
                var accountListToReturn = BuildAccountListFromGuidCollection(hashFollowers, size, storage);

                // a stream is needed for serialization
                var stream = new MemoryStream();

                (new XmlSerializer(typeof(Accounts))).Serialize(stream, accountListToReturn);

                result = Content(stream.ToString());
            }
            catch (StorageLibException exception)
            {
                // Result is an non-empty error XML element
                var stream = new MemoryStream();
                (new XmlSerializer(typeof(Error))).Serialize(stream, new Error(exception.Code.ToString()));
                result = Content(stream.ToString());
            }

            return result;
        }

        private ContentResult SubscriptionsEitherPublicOrAll(string accountName, int numberOfSubscriptions, bool withPrivate)
        {
            IStorage storage = new StorageTmp(); // connexion
            ContentResult result;

            try
            {
                // get the public lists followed by the given account

                var accountId = storage.Account.GetId(accountName);
                var followedLists = storage.List.GetAccountFollowedLists(accountId, withPrivate);
                var accountsInLists = new HashSet<Guid>();
                foreach (var followedList in followedLists)
                {
                    accountsInLists.UnionWith(storage.List.GetAccounts(followedList));
                }

                // Get as many subscriptions as possible (maximum: numberOfSubscriptions)
                var size = Math.Min(accountsInLists.Count, numberOfSubscriptions);
                var accountListToReturn = BuildAccountListFromGuidCollection(accountsInLists, size, storage);

                // a stream is needed for serialization
                var stream = new MemoryStream();

                (new XmlSerializer(typeof(Accounts))).Serialize(stream, accountListToReturn);

                result = Content(stream.ToString());
            }
            catch (StorageLibException exception)
            {
                // Result is an non-empty error XML element
                var stream = new MemoryStream();
                (new XmlSerializer(typeof(Error))).Serialize(stream, new Error(exception.Code.ToString()));
                result = Content(stream.ToString());
            }

            return result;
        }


        //
        // GET : /infoaccount/publicsubscriptions/{accountName}/{numberOfSubscriptions}

        public ActionResult PublicSubscriptions(string accountName, int numberOfSubscriptions)
        {
            return SubscriptionsEitherPublicOrAll(accountName, numberOfSubscriptions, false);
        }


        //
        // GET : /infoaccount/subscriptions/{accountName}/{numberOfSubscriptions}
        // [authorize]
        public ActionResult Subscriptions(string accountName, int numberOfSubscriptions)
        {
            return SubscriptionsEitherPublicOrAll(accountName, numberOfSubscriptions, true);
        }


        
        private ActionResult SubscribedListsEitherPublicOrAll(string accountName, int numberofLists, bool withPrivate)
        {
            //TODO: implement this
            throw new NotImplementedException();
        }

        
        //
        // GET : /infoaccount/subscribedpubliclists/{accountName}/{numberOfLists}

        public ActionResult SubscribedPublicLists(string accountName, int numberofLists)
        {
            return SubscribedListsEitherPublicOrAll(accountName, numberofLists, false);
        }

        //
        // GET : /infoaccount/subscribedlists/{accountName}/{numberOfLists}

        //[Authorize]
        public ActionResult SubscribedLists(string accountName, int numberofLists)
        {
            return SubscribedListsEitherPublicOrAll(accountName, numberofLists, true);
        }



        private ActionResult OwnedListsEitherPublicOrAll(string accountName, int numberOfList, bool withPrivate)
        {
            //TODO: implement this
            throw new NotImplementedException();
        }


        //
        // GET : infoaccount/ownedpubliclists/{accountName}/{numberOfLists}

        public ActionResult OwnedPublicLists(string accountName, int numberOfList)
        {
            return OwnedListsEitherPublicOrAll(accountName, numberOfList, false);
        }

        //
        // GET : /infoaccount/ownedlists/{accountName}/{numberOfLists}

        //[Authorize]
        public ActionResult OwnedLists(string accountName, int numberOfList)
        {
            return OwnedListsEitherPublicOrAll(accountName, numberOfList, true);
        }

    }
}