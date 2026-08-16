using Microsoft.JSInterop;

namespace SD.WEB.Core.Javascript
{
    public class SliderJs(IJSRuntime js) : JsModuleBase(js, "./js/slider.js")
    {
        public Task InitLists(string id, CancellationToken cancellationToken, int? size = null, bool refresh = false) => InvokeVoid("slider.initLists", cancellationToken, id, size, refresh);

        public Task InitTrailers(string id, CancellationToken cancellationToken) => InvokeVoid("slider.initTrailers", cancellationToken, id);
    }
}