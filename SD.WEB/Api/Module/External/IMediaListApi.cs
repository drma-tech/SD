namespace SD.WEB.Api.Module.External;

public interface IMediaListApi
{
    Task<(ISet<MediaDetail> list, bool lastPage)>
        GetList(ISet<MediaDetail> currentList, RenderControlState<ISet<MediaDetail>>? actions, MediaType? type = null,
            IDictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1, CancellationToken cancellationToken = default);
}