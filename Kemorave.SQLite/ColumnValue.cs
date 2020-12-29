namespace Kemorave.SQLite
{
    public class ColumnValue
    {
        public ColumnValue(string name, object result, int Indext)
        {
            ColumnName = name;
            Value = result;
            Index = Indext;
        }

        public int Index { get; }
        public string ColumnName { get; }
        public object Value { get; }
        public override string ToString()
        {
            string val = Value?.ToString();

            return val ?? "Null";
        }
    }
}