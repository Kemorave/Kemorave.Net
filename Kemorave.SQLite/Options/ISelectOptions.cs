namespace Kemorave.SQLite.Options
{
    internal interface ISelectOptions<Model>
    {
        string[] Attributes { get; set; }
        bool DISTINCT { get; set; }
        int? Limit { get; set; }
        string Table { get; }
        int? Offset { get; set; }
        string OrderBy { get; set; } 
        string GetCommand();
        Where Where { get; set; }
    }
}