using System;
using System.Collections.Generic;
using System.Text;

namespace Kemorave.Net.Api.DB
{
    public abstract class DBControllerBase<Result, Model> where Model : IDBModel
    {
        protected DBControllerBase(ApiConfigration configration, string uri, object tag=null)
        {
            Configration = configration ?? throw new ArgumentNullException(nameof(configration));
            Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            Tag = tag;
        }

        protected Api.ApiConfigration Configration { get; }

        protected string Uri { get; } = string.Empty;
        public object Tag { get; set; }
        public abstract Result InsertItem(Model model);
        public abstract Result UpdateItem(Model model);
        public abstract Result FindItem(int id);
        public abstract Result DeleteItem(int id);
        public abstract IReadOnlyList<Model> GetAllItems();
        public virtual string GetInsertUri(object obj)
        {
            if (obj == null)
            {
                return Uri;
            }
            return null;
        }
        public virtual string GetDeleteUri(object obj) => GetUpdateUri(obj);
        public virtual string GetUpdateUri(object obj)
        {
            if (obj == null)
            {
                return Uri;
            }
            if (obj is IDBModel dB)
            {
                return Uri + "/" + dB.ID;
            }
            return null;
        }
    }
}