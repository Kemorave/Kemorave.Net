using Kemorave.Net.Api;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemorave.Net
{
    public interface IController<Model> where Model : IModel
    {
        Task<string> Create(Model model);
        Task Delete(long id);
        Task Update(Model model);
        Task<Model> FindById(long id);
        Task<List<Model>> FindAll();
    }
}