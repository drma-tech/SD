using SD.WEB.Modules.Collections.Core;

namespace SD.WEB.Modules.Collections.Interface;

public interface IMediaListApi
{
    Task<(HashSet<MediaDetail> list, bool lastPage)>
        GetList(HashSet<MediaDetail> currentList, MediaType? type = null,
            Dictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1);
}