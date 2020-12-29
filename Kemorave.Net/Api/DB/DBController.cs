using System;
using System.Collections.Generic;

namespace Kemorave.Net.Api.DB
{
    public class DBController<Model> : DBControllerBase<DBResult, Model> where Model : IDBModel
    {
        
        public DBController(ApiConfigration configration, string uri, object tag = null) : base(configration, uri, tag)
        {
        }

        public override DBResult DeleteItem(int id)
        {
            throw new NotImplementedException();
        }

        public override DBResult FindItem(int id)
        {
            throw new NotImplementedException();
        }

        public override IReadOnlyList<Model> GetAllItems()
        {
            throw new NotImplementedException();
        }

        public override DBResult InsertItem(Model model)
        {
            throw new NotImplementedException();
        }

        public override DBResult UpdateItem(Model model)
        {
            throw new NotImplementedException();
        }
    }
}