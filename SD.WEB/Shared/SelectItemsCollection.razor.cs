using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SD.WEB.Shared
{
    public partial class SelectItemsCollection
    {
        [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }

        [Parameter] public ICollection<Collection> ItemsCollection { get; set; } = [];
        [Parameter] public ISet<string> SelectedItems { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        [Parameter] public EventCallback<ISet<string>> SelectedItemsChanged { get; set; }

        private void CheckedChanged(string? id, bool value)
        {
            if (id == null) return;

            if (value)
                SelectedItems.Add(id);
            else
                SelectedItems.Remove(id);
        }

        private async Task SelectAll()
        {
            foreach (var item in ItemsCollection.Where(p => p != null && p.id.NotEmpty()))
            {
                SelectedItems.Add(item.id!);
            }

            await Confirm();
        }

        private async Task Confirm()
        {
            try
            {
                await SelectedItemsChanged.InvokeAsync(SelectedItems);
                MudDialog?.Close();
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
            }
        }

        public void HideModal()
        {
            MudDialog?.Close();
        }
    }
}