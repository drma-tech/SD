using SD.WEB.Modules.Suggestions.Core;

namespace SD.WEB.Modules.Suggestions.Interface
{
    public interface IMediaListApi
    {
        Task<(HashSet<MediaDetail> list, bool lastPage)>
            GetList(HashSet<MediaDetail> currentList, MediaType? type = null, Dictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1);
    }
}