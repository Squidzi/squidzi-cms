using Squidzi.Models.JavascriptModels;
using System.Threading.Tasks;

namespace Squidzi.Services.Managers.Contracts
{
    public interface IMetaTagsManager
    {
        Task SetMetaTags(SetMetaTagsRequest request);
    }
}
