using System;

namespace Kemorave.SQLite.Options
{
    public sealed class WhereConditon
    {
        private WhereConditon(string column, object value, string @operator, bool not = false)
        {
            HasParameters = true;
            Column = column ?? throw new ArgumentNullException(nameof(column));
            Value = value?.ToString() ?? throw new ArgumentNullException(nameof(value));
             Operator = @operator ?? throw new ArgumentNullException(nameof(@operator));
            Not = not;
        }
        private WhereConditon(string ownCommand,object value)
        {
            _Command = ownCommand;
            Value = value?.ToString() ;
            HasParameters = Value != null;
        }
        static string Escape(string value)
        {
            if (value == null)
            {
                return value;
            }

            return  value.Replace("%", "\\%").Replace("_", "\\_") ;
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

            return new WhereConditon($" {column} {(not ? "NOT" : string.Empty)}  LIKE ?  ESCAPE '\\'",$"%{Escape(value)}%" );
        }
        public static WhereConditon StartWith(string column, string value, bool not = false)
        {
            return new WhereConditon($" {column} {(not ? "NOT" : string.Empty)}  LIKE ?  ESCAPE '\\'", $"{Escape(value)}%");
        }
        public static WhereConditon EndsWith(string column, string value, bool not = false)
        {
            return new WhereConditon($" {column} {(not ? "NOT" : string.Empty)}  LIKE ?  ESCAPE '\\'",  $"%{Escape(value)}");
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

            string flatValue = $"({value[0]}";
            if (value.Length > 1)
                for (int i = 1; i < value.Length; i++)
                {
                    flatValue += $",{value[i]}";
                }
            flatValue += ")";
            return new WhereConditon($" {column} {(not ? "NOT" : string.Empty)} IN  {flatValue} ", null);
        }
        public static WhereConditon Between(string column, string min, string max, bool not = false)
        {
             return new WhereConditon( $"{column}  {(not ? "NOT" : string.Empty)} BETWEEN {min} AND {max}",null);
        }

        internal string GetCommand()
        { 
            if(string.IsNullOrEmpty(_Command))
            return $" {Column} {(Not ? "NOT" : string.Empty)} {Operator}  ? ";
            return _Command;
        }
        public override string ToString()
        {
            return GetCommand();
        } 
        string _Command;
        public bool Not { get; }
        public bool HasParameters { get;   }  
        public string Column { get; }
        public string Value { get; }
        public string Operator { get; }
    }
}