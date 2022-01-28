using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kemorave.Net.Api
{
    public class ApiController<Model> : IController<Model> where Model : IModel
    {

        public ApiController(ClientConfigration configration, string baseUrl = null)
        {
            Configration = configration ?? throw new ArgumentNullException(nameof(configration));
            BaseUrl = baseUrl ?? configration.HttpClient.BaseAddress + "/" + nameof(Model).ToLower() + "/";
        }

        public ClientConfigration Configration { get; }
        public string BaseUrl { get; }
        public async Task<string> Create(Model model)
        {
            return await Task.Run(async () =>
            {
                HttpResponseMessage res = await Configration.HttpClient.PostAsync(BaseUrl + "create", NetUtil.GetJsonContent(model));

                return await res.GetContent();
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="files"></param>
        /// <exception cref="ResponceExeption"/>
        /// <exception cref="System.Net.Http.HttpRequestException"/>
        /// <returns></returns>
        public async Task<string> Create(Model model, IEnumerable<string> files)
        {
            return await Task.Run(async () =>
            {
                using (HttpRequestMessage message = new HttpRequestMessage())
                using (MultipartFormDataContent content = new MultipartFormDataContent())
                {
                    content.Add(NetUtil.GetJsonContent(model));
                    foreach (string file in files)
                    {
                        FileStream filestream = new FileStream(file, FileMode.Open);
                        string fileName = System.IO.Path.GetFileName(file);
                        content.Add(new StreamContent(filestream), "file", fileName);
                    }
                    message.Method = HttpMethod.Post;
                    message.Content = content;
                    message.RequestUri = new Uri(BaseUrl + "create");
                    HttpResponseMessage res = await Configration.HttpClient.SendAsync(message);
                    return await res.GetContent();
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        ///   /// <exception cref="ResponceExeption"/>
        /// <exception cref="System.Net.Http.HttpRequestException"/>
        /// <returns></returns>
        public async Task Delete(long id)
        {
            await Task.Run(async () =>
           {
               HttpResponseMessage res = await Configration.HttpClient.DeleteAsync(BaseUrl + "delete/" + id);

               return await res.GetContent();
           });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="models"></param>
        ///   /// <exception cref="ResponceExeption"/>
        /// <exception cref="System.Net.Http.HttpRequestException"/>

        /// <returns></returns>
        public async Task DeleteMulti(IEnumerable<Model> models)
        {
            await Task.Run(async () =>
            {
                HttpResponseMessage res = await Configration.HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, BaseUrl + "delete/multi")

                { Content = new StringContent("{\"ids\":\"[" + models.Select(m => m.Id.ToString()).Aggregate((s, a) => s + "," + a) + "]\"}") });
                await res.GetContent();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="from"></param>
        /// <param name="where"></param>
        ///   /// <exception cref="ResponceExeption"/>
        /// <exception cref="System.Net.Http.HttpRequestException"/>

        /// <returns></returns>
        public async Task<List<Model>> FindAll(long offset, int limit = 30, FindDirection from = FindDirection.Next, string where = null)
        {
            return await Task.Run(async () =>
            {
                HttpResponseMessage res = await Configration.HttpClient.PostAsync(BaseUrl + "find/all/from/" + offset, NetUtil.GetJsonContent("{ \"from\":\"" + from + "\",\"limit\":" + limit + (string.IsNullOrEmpty(where) ? string.Empty : ",\"where\":" + where) + "}"));
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Model>>(await res.GetContent());

            });
        }

        /// <summary>
        /// 
        /// </summary>
        ///   /// <exception cref="ResponceExeption"/>
        /// <exception cref="System.Net.Http.HttpRequestException"/>

        /// <returns></returns>
        public async Task<List<Model>> FindAll()
        {
            return await Task.Run(async () =>
            {
                HttpResponseMessage res = await Configration.HttpClient.GetAsync(BaseUrl + "find/all");
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Model>>(await res.GetContent());

            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        ///   /// <exception cref="ResponceExeption"/>
        /// <exception cref="System.Net.Http.HttpRequestException"/>

        /// <returns></returns>
        public async Task<Model> FindById(long id)
        {
            return await Task.Run(async () =>
            {
                HttpResponseMessage res = await Configration.HttpClient.GetAsync(BaseUrl + "find/" + id);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Model>(await res.GetContent());
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="attribute"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="from"></param>
        ///   /// <exception cref="ResponceExeption"/>
        /// <exception cref="System.Net.Http.HttpRequestException"/>

        /// <returns></returns>
        public async Task<List<Model>> Search(string value, string attribute, long offset = 0, int limit = 30, FindDirection from = FindDirection.Next)
        {
            if (string.IsNullOrEmpty(attribute))
            {
                throw new ArgumentException("message", nameof(attribute));
            }

            return await FindAll(offset, limit, from, $"{{\"{attribute}\":{value}}}");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        ///   /// <exception cref="ResponceExeption"/>
        /// <exception cref="System.Net.Http.HttpRequestException"/>

        /// <returns></returns>
        public async Task Update(Model model)
        {
            await Task.Run(async () =>
            {
                HttpResponseMessage res = await Configration.HttpClient.PutAsync(BaseUrl + "update/" + model.Id, NetUtil.GetJsonContent(model));
                await res.GetContent();
            });
        }
    }
}