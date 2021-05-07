using Kemorave.SQLite.SQLiteAttribute;

namespace Kemorave.SQLite.Options
{
    public class SelectOptions<Model> : ISelectOptions<Model> where Model :  IDBModel, new()
    {
        internal SelectOptions( string orderBy = null, string[] atributes = null, Where where = null, int? limit = null, int? offset = null)
        {
            Attributes = atributes;
            Limit = limit;
            Offset = offset;
            Table = TableAttribute.GetTableName(typeof(Model));

            OrderBy = orderBy;
            Where = where;
        }
        public string[] Attributes { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
        public bool DISTINCT { get; set; }
        public string Table { get; set; }
        public Where Where { get; set; }
        public string OrderBy { get; set; }
        public override string ToString()
        {
            return GetCommand();
        }

        public virtual string GetCommand()
        {
            string cmd = string.Empty;
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
            cmd += $"SELECT {(DISTINCT ? "DISTINCT" : string.Empty)} {atributes} FROM {Table} {Where}";
            if (Limit > 0)
                cmd += $" LIMIT {Limit}";
            if (Offset > 0)
                cmd += $" OFFSET {Offset}";
            if (!string.IsNullOrEmpty(OrderBy))
                cmd += $" ORDER BY {OrderBy}";
            return cmd;
        }
    }
}
