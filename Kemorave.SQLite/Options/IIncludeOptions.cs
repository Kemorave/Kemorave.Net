namespace Kemorave.SQLite.Options
{
    internal  interface IIncludeOptions<Model,IncludeModel>:ISelectOptions<Model>
    {


        string IncludeTable { get; }
        string ForigenKey { get; }
    }
}
