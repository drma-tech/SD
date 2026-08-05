using Microsoft.AspNetCore.Components;

namespace SD.WEB.Shared
{
    public partial class Slider<T>
    {
        [Parameter][EditorRequired] public IEnumerable<T> List { get; set; } = [];
        [Parameter][EditorRequired] public RenderFragment<T> ChildContent { get; set; } = null!;

        [Parameter] public string? LoadingHeight { get; set; } = "100px";

        private RenderControlState<IEnumerable<T>> Actions { get; set; } = new(list => list == null || list.Empty());
        private readonly string _id = $"{Guid.NewGuid()}";

        private bool refreshed;

        protected override async Task OnParametersSetAsync()
        {
            await Actions.FinishLoading.Invoke(List);
            refreshed = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                await Actions.FinishLoading.Invoke(List);
                await JsRuntime.Slider().InitLists(_id, CancellationToken.None, size: null, refreshed);
                refreshed = false;
            }
            catch (Exception ex)
            {
                await Actions.ShowError(ex.Message);
            }
        }
    }
}