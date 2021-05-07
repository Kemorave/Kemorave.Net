namespace Kemorave.SQLite
{
    
        public enum SQLiteActions
        {/// <summary>
         /// Nothing will happen, this is default sction
         /// </summary>
            NO_ACTION,
            /// <summary>
            /// Value will be sett to <see cref="null"/>
            /// </summary>
            SET_NULL,
            /// <summary>
            /// Value will be set to <see cref="ColumnInfo.DefaultValue"/> 
            /// </summary>
            SET_DEFAULT,
            /// <summary>
            /// Does not allow you to change or delete values in the parent key of the parent table.
            /// </summary>
            RESTRICT,
            /// <summary>
            /// Propagates the changes from the parent table to the child table when you update or delete the parent key.
            /// </summary>
            CASCADE
        }
    
}