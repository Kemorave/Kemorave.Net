using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kemorave.Net.Api.DB
{
    public class ApiController<Model> : ApiControllerBase<DBActionResult, Model> where Model : class, IDBModel, new()
    {
        public bool JsonResultSupport { get; }

        public ApiController(ApiConfigration configration, string uri, bool jsonResult, object tag = null) : base(configration, uri, tag)
        {
            JsonResultSupport = jsonResult;
        }

        /// <summary>
        /// Deletes item from database by id
        /// </summary>
        /// <param name="id">item id</param>
        /// <returns>Operation result</returns>
        public override async Task<DBActionResult> DeleteItemAsync(int id)
        {
            try
            {
                System.Net.Http.HttpResponseMessage res = await Configration.HttpClient.DeleteAsync(GetDeleteUri(id));
                res.EnsureSuccessStatusCode();
                if (JsonResultSupport)
                {
                    return DBActionResult.GetDBResult(await res.Content.ReadAsStringAsync());
                }
                return new DBActionResult(ApiConfigration.DeleteCode);
            }
            catch (Exception e)
            {
                return new DBActionResult(ApiConfigration.DeleteCode, e);
            }
        }
        /// <summary>
        /// Gets all table items
        /// </summary>
        /// <param name="id">item id</param>
        /// <returns>Operation result</returns>
        public override async Task<Model> GetItemAsync(int id)
        {

            System.Net.Http.HttpResponseMessage res = await Configration.HttpClient.GetAsync(GetByIDUri(id));
            res.EnsureSuccessStatusCode();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Model>>(await res.Content.ReadAsStringAsync()).FirstOrDefault();

        }
        /// <summary>
        /// Adds item to table
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Operation result</returns>
        public override async Task<DBActionResult> InsertItem(Model model)
        {
            try
            {
                System.Net.Http.HttpResponseMessage res = await Configration.HttpClient.PostAsync(GetPostUri(model), GetPostContent(model));
                res.EnsureSuccessStatusCode();
                if (JsonResultSupport)
                {
                    return DBActionResult.GetDBResult(await res.Content.ReadAsStringAsync());
                }
                return new DBActionResult(ApiConfigration.InsertCode) { };
            }
            catch (Exception e)
            {
                return new DBActionResult(ApiConfigration.InsertCode, e);
            }
        }
        public override async Task<DBActionResult> UpdateItemAsync(Model model)
        {
            try
            {
                System.Net.Http.HttpResponseMessage res = await Configration.HttpClient.PutAsync(GetPutUri(model), GetPutContent(model));
                res.EnsureSuccessStatusCode();
                if (JsonResultSupport)
                {
                    return DBActionResult.GetDBResult(await res.Content.ReadAsStringAsync());
                }
                return new DBActionResult(ApiConfigration.UpdateCode) { };
            }
            catch (Exception e)
            {
                return new DBActionResult(ApiConfigration.UpdateCode, e);
            }
        }
        public override async Task<IEnumerable<Model>> GetAllItemsAsync()
        {
            System.Net.Http.HttpResponseMessage res = await Configration.HttpClient.GetAsync(GetAllItemsUri());
            res.EnsureSuccessStatusCode();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Model>>(await res.Content.ReadAsStringAsync());
        }

        #region Overrides


        #endregion
        #region WEB/prefix
        protected virtual string GetPostUri(Model model)
        {
            return Uri;
        }

        protected virtual string GetPutUri(Model model)
        {
            return Uri;
        }

        protected virtual string GetAllItemsUri()
        {
            return Uri;
        }

        protected virtual HttpContent GetPutContent(Model model)
        {
            return GetPostContent(model);
        }

        protected virtual HttpContent GetPostContent(Model model)
        {
            return ModelToJsonHttpContent(model);
        }

        protected HttpContent ModelToJsonHttpContent(Model model)
        {
            return new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(model));
        }

        protected virtual string GetDeleteUri(int id)
        {
            return GetByIDUri(id);
        }

        protected virtual string GetByIDUri(int id)
        {
            if (!Uri.EndsWith("//"))
            {
                return $"{Uri}//{id}";
            }
            return $"{Uri}{id}";
        }

        #endregion

    }
}