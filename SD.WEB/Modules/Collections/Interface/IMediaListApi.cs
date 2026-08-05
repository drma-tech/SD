namespace SD.WEB.Modules.Collections.Interface;

public interface IMediaListApi
{
    Task<(ICollection<MediaDetail> list, bool lastPage)>
        GetList(ICollection<MediaDetail> currentList, RenderControlState<ICollection<MediaDetail>>? actions, MediaType? type = null,
            IDictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1, CancellationToken cancellationToken = default);
}