using Kemorave.SQLite.SQLiteAttribute;

namespace Kemorave.SQLite.Options
{
    public sealed class IncludeOptions<Model, IncludeModel> : SelectOptions<IncludeModel>,IIncludeOptions<Model, IncludeModel> where Model :  IDBModel, new() where IncludeModel :  IDBModel, new()

    { 
          
        public IncludeOptions(string forigenKey=null, string orderBy = null, string[] atributes = null, Where where = null, int? limit = null, int? offset = null) : base(orderBy, atributes, where, limit, offset)
        { 
            IncludeTable = TableAttribute.GetTableName(typeof(IncludeModel));

            ForigenKey = forigenKey;
            if (string.IsNullOrEmpty(ForigenKey))
            {
                ForigenKey = Table += "Id";
            }
        }

        public override string ToString()
        {
            return string.Format( GetCommand(),"Id");
        }
        public override string GetCommand()
        {
          
            string atributes = "*";
            if (Attributes?.Length > 0)
            {
                atributes = $"{Attributes[0]}";
                if (Attributes?.Length > 1)
                    for (int i = 1; i < Attributes.Length; i++)
                    {
                        atributes += $",{Attributes[i]}";
                    }
            }
            string cmd = $"SELECT {(DISTINCT ? "DISTINCT" : string.Empty)} {atributes} FROM {IncludeTable} {(Where?.GetCommand() == null ? "WHERE" : $"{Where} AND ")} {ForigenKey} in (SELECT {(ItemID==null?"Id": ItemID.Value.ToString())} FROM {Table})";
            return cmd;
        }
        internal long? ItemID { get; set; }
        public string IncludeTable { get; set; }
        public string ForigenKey { get; private set; }
    }
}