using SD.WEB.Modules.List.Core;

namespace SD.WEB.Modules.List.Interface
{
    public interface IMediaListApi
    {
        Task<(HashSet<MediaDetail> list, bool lastPage)> 
            GetList(HashSet<MediaDetail> currentList, MediaType? type = null, Dictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1);
    }
}
