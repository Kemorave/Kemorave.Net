
using System.Threading.Tasks;

namespace Kemorave.Net.Api.DB
{
    public abstract class ApiControllerBase<Result, Model> where Model : IDBModel
    {
        protected ApiControllerBase(ApiConfigration configration, string uri, object tag = null)
        {
            Configration = configration ?? throw new System.ArgumentNullException(nameof(configration));
            Uri = uri ?? throw new System.ArgumentNullException(nameof(uri));
            Tag = tag;
        }

        protected Api.ApiConfigration Configration { get; }

        protected string Uri { get; } = string.Empty;
        public object Tag { get; set; }
        public abstract Task<Result> InsertItem(Model model);
        public abstract Task<Result> UpdateItemAsync(Model model);
        public abstract Task<Model> GetItemAsync(int id);
        public abstract Task<Result> DeleteItemAsync(int id);
        public abstract Task<System.Collections.Generic.IEnumerable<Model>> GetAllItemsAsync();
    }
}