namespace Tigwi.UI.Models.Storage
{
    using System;

    using StorageLibrary;

    public class ListRepository : StorageEntityRepository<StorageListModel>, IListRepository
    {
        #region Constructors and Destructors

        public ListRepository(IStorage storage, IStorageContext storageContext)
            : base(storage, storageContext)
        {
        }

        #endregion

        public StorageListModel Create(StorageAccountModel account, string name, string description, bool isPrivate)
        {
            try
            {
                var id = this.Storage.List.Create(account.Id, name, description, isPrivate);
                return this.Find(id);
            }
            catch (AccountNotFound ex)
            {
                throw new AccountNotFoundException(account.Name, ex);
            }
        }

        public void Delete(StorageListModel list)
        {
            this.Storage.List.Delete(list.Id);
            this.EntitiesMap.Remove(list.Id);
        }

        public StorageListModel Find(Guid listId)
        {
            StorageListModel list;
            if (!this.EntitiesMap.TryGetValue(listId, out list))
            {
                // TODO
                list = null;
                this.EntitiesMap.Add(listId, list);
            }

            return list;
        }
    }
}