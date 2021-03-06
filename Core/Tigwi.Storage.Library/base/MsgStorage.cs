﻿#region copyright
// Copyright (c) 2012, TIGWI
// All rights reserved.
// Distributed under  BSD 2-Clause license
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tigwi.Storage.Library;
using Tigwi.Storage.Library.Utilities;
using Microsoft.WindowsAzure.StorageClient;
using System.Threading;

namespace Tigwi.Storage.Library
{
    public class MsgStorage : IMsgStorage
    {
        // We differ messages post to be sure they have been inserted were they should be in time
        // This is also necessary because of the bad time synchronisation in azure
        TimeSpan limitDateDiff = TimeSpan.FromSeconds(0);

        BlobFactory blobFactory;

        public MsgStorage(BlobFactory blobFactory)
        {
            this.blobFactory = blobFactory;
        }

        public List<IMessage> GetListsMsgFrom(HashSet<Guid> listsId, DateTime firstMsgTime, int msgNumber)
        {
            // TODO : add some parallization
            MessageSet messages = new MessageSet();
            foreach (Guid listId in listsId)
                messages.UnionWith(blobFactory.MListMessages(listId).GetMessagesFrom(firstMsgTime, msgNumber, new ListNotFound()));

            List<IMessage> msgList = new List<IMessage>(messages);
            if (msgList.Count > msgNumber)
                msgList.GetRange(0, msgNumber);

            return TruncateMessages(msgList);
        }

        public List<IMessage> GetListsMsgTo(HashSet<Guid> listsId, DateTime lastMsgTime, int msgNumber)
        {
            // TODO : add some parallization
            MessageSet messages = new MessageSet();
            lastMsgTime = TruncateDate(lastMsgTime);
            foreach (Guid listId in listsId)
                messages.UnionWith(blobFactory.MListMessages(listId).GetMessagesTo( lastMsgTime, msgNumber, new ListNotFound()));

            List<IMessage> msgList = new List<IMessage>(messages);
            if (msgList.Count > msgNumber)
                msgList.GetRange(0, msgNumber);

            return TruncateMessages(msgList);
        }

        public IMessage GetMessage(Guid msgId)
        {
            return blobFactory.MMessage(msgId).GetIfExists(new MessageNotFound());
        }

        public void Tag(Guid accountId, Guid msgId)
        {
            // retrive the message
            Message message = blobFactory.MMessage(msgId).GetIfExists(new MessageNotFound());

            // Tag it
            if (!blobFactory.MTaggedMessages(accountId).AddMessage(message))
                throw new AccountNotFound();
        }

        public void Untag(Guid accountId, Guid msgId)
        {
            // retrive the message to get its date
            Message message;
            try
            {
                message = blobFactory.MMessage(msgId).GetIfExists(new MessageNotFound());
            }
            catch { return; }

            // remove the message from tagged
            blobFactory.MTaggedMessages(accountId).RemoveMessage(message);
        }

        public List<IMessage> GetTaggedFrom(Guid accoundId, DateTime firstMsgDate, int msgNumber)
        {
            List<IMessage> msgs = new List<IMessage>(blobFactory.MTaggedMessages(accoundId).GetMessagesFrom(firstMsgDate, msgNumber, new AccountNotFound()));
            return TruncateMessages(msgs);
        }

        public List<IMessage> GetTaggedTo(Guid accountId, DateTime lastMsgDate, int msgNumber)
        {
            return new List<IMessage>(blobFactory.MTaggedMessages(accountId).GetMessagesTo(TruncateDate(lastMsgDate), msgNumber, new AccountNotFound()));
        }

        public Guid Post(Guid accountId, string content)
        {
            Guid messageId = Guid.NewGuid();
            Blob<Message> bMessage = blobFactory.MMessage(messageId);

            try
            {
                IAccountInfo accountInfo = blobFactory.AInfo(accountId).GetIfExists(new AccountNotFound());
                Guid personnalListId = blobFactory.LPersonnalList(accountId).GetIfExists(new AccountNotFound());
                Message message = new Message(messageId, accountId, accountInfo.Name, "", DateTime.Now, content);
                MsgSetBlobPack bPersonnalListMsgs = blobFactory.MListMessages(personnalListId);

                // Save the message
                bMessage.Set(message);
                if (!bPersonnalListMsgs.AddMessage(message))
                    throw new AccountNotFound();

                // Add in listMsg -- if a list is added during the foreach, then the message will be added by the addition of the list
                foreach (Guid listId in blobFactory.LFollowedByAll(accountId).GetIfExists(new AccountNotFound()))
                {
                    try { blobFactory.MListMessages(listId).AddMessage(message); }
                    catch { }
                }

                List<Message> lastmessages = blobFactory.MLastMessage().GetWithDefault(new List<Message>());
                lastmessages.Add(message);
                // TODO: Take does *NOT* modify lastmessages :-)
                lastmessages.Take(100);
                
                blobFactory.MLastMessage().Set(lastmessages);
            }
            catch {
                bMessage.Delete();
                throw;
            }


            // TODO : Add in accountMsg

            return messageId;
        }

        public Guid Copy(Guid accountId, Guid msgId)
        {
            Message message = blobFactory.MMessage(msgId).GetIfExists(new MessageNotFound());
            message.Content = message.Content + "\n\nretwiged by " + blobFactory.AInfo(accountId).GetIfExists(new AccountNotFound()).Name;
            message.Date = DateTime.Now;
            message.Id = Guid.NewGuid();
            Blob<Message> bMessage = blobFactory.MMessage(message.Id);
            try
            {
                Guid personnalListId = blobFactory.LPersonnalList(accountId).GetIfExists(new AccountNotFound());
                MsgSetBlobPack bPersonnalListMsgs = blobFactory.MListMessages(personnalListId);

                // Save the message
                bMessage.Set(message);
                if (!bPersonnalListMsgs.AddMessage(message))
                    throw new AccountNotFound();

                // Add in listMsg -- if a list is added during the foreach, then the message will be added by the addition of the list
                foreach (Guid listId in blobFactory.LFollowedByAll(accountId).GetIfExists(new AccountNotFound()))
                {
                    try { blobFactory.MListMessages(listId).AddMessage(message); }
                    catch { }
                }
            }
            catch { bMessage.Delete(); }
            return message.Id;
        }

        // NYI
        public void Remove(Guid id)
        {
            throw new NotImplementedException();
        }

        DateTime TruncateDate(DateTime d)
        {
            DateTime limitDate = DateTime.Now - limitDateDiff;
            return d < limitDate ? d : limitDate;
        }

        List<IMessage> TruncateMessages(List<IMessage> msgs)
        {
            // we remove msg to recents because their is no time synchronisation between azure VMs
            DateTime dateLimit = DateTime.Now - limitDateDiff;
            return msgs.TakeWhile(m => m.Date < dateLimit).ToList();    // TODO : improve performance
        }

        public List<IMessage> GetLastMessages()
        {
            return new List<IMessage>(this.blobFactory.MLastMessage().GetWithDefault(new List<Message>()));
        }
    }
}
