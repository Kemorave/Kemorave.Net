using System;

namespace Kemorave.SQLite.Options
{
    public sealed class WhereConditon
    {
        private WhereConditon(string column, object value, string @operator, bool not = false)
        {
            Column = column ?? throw new ArgumentNullException(nameof(column));
            Value = value?.ToString() ?? throw new ArgumentNullException(nameof(value));
            if (string.IsNullOrWhiteSpace(Value))
                throw new ArgumentNullException("Value");
            Operator = @operator ?? throw new ArgumentNullException(nameof(@operator));
            Not = not;
        }
        static string Escape(string value)
        {
            return value.Replace("%", "\\%").Replace("_", "\\_").Replace("_", "\\_");
        }

        public static  WhereConditon IsEqual(string column, object value, bool not = false)
        {
            return new WhereConditon(column, value, "=", not);
        }
        
        public static WhereConditon IsLessThan(string column, object value, bool orEqual=false, bool not = false)
        {
            return new WhereConditon(column, value, $"{(orEqual ? "=" : string.Empty)}<", not);
        }
        public static WhereConditon IsGreaterThan(string column, object value, bool orEqual=false, bool not = false)
        {
            return new WhereConditon(column, value, $"{(orEqual ? "=" : string.Empty)}>", not);
        }
       
        public static WhereConditon NotEqual(string column, object value)
        {
            return new WhereConditon(column, value, "<>", false);
        }
        public static WhereConditon Like(string column, string value, bool not = false)
        {
            return new WhereConditon(column, $"%{Escape(value)}%  ESCAPE '\\'", "LIKE", not);
        }
        public static WhereConditon StartWith(string column, string value, bool not = false)
        {
            return new WhereConditon(column, $"{Escape(value)}%  ESCAPE '\\'", "LIKE", not);
        }
        public static WhereConditon EndsWith(string column, string value, bool not = false)
        {
            return new WhereConditon(column, $"%{Escape(value)}  ESCAPE '\\'", "LIKE", not);
        }
        public static WhereConditon IsIn(string column, object[] value, bool not = false)
        {
            if (value == null  )
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (  value?.Length == 0)
            {
                throw new ArgumentException($"Atleast one value is needed");
            }

            string flatValue = $"(?";
            if (value.Length > 1)
                for (int i = 1; i < value.Length; i++)
                {
                    flatValue += $",?";
                }
            flatValue += ")";
            return new WhereConditon(column, flatValue, "IN", not);
        }
        public static WhereConditon Between(string column, object min, object max, bool not = false)
        {
             return new WhereConditon(column, $"{min} AND {max}", "BETWEEN", not);
        }

        internal string GetCommand()
        {

            return $" {Column} {(Not ? "NOT" : string.Empty)} {Operator}  ? ";
        }
        public override string ToString()
        {
            return GetCommand();
        }
        public bool Not { get; }
        public string Column { get; }
        public string Value { get; }
        public string Operator { get; }
    }
}