using SD.Shared.Modal;
using SD.Shared.Modal.Enum;
using SD.WEB.Core;

namespace SD.WEB.Services
{
    public interface IMediaListService
    {
        Task PopulateListMedia(HttpClient http, IStorageService storage, Settings settings,
            HashSet<MediaDetail> list_media, MediaType type, int qtd = 9, Dictionary<string, string> ExtraParameters = null);
    }
}