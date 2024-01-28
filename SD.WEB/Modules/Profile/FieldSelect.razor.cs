using Blazorise;
using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;

namespace SD.WEB.Modules.Profile
{
    public partial class FieldSelect<TValue, TEnum> : FormBase<TValue, FieldSelect<TValue, TEnum>> where TEnum : struct, Enum, IConvertible
    {
        [Parameter] public string? CssIcon { get; set; }
        [Parameter] public bool Required { get; set; } = false;
        [Parameter] public bool Multiple { get; set; } = false;
        [Parameter] public bool ShowGroup { get; set; } = false;
        [Parameter] public bool ShowSwitch { get; set; } = false;
        [Parameter] public bool ShowHelper { get; set; } = true;
        [Parameter] public bool ShowDescription { get; set; } = true;
        [Parameter] public string? HelpLink { get; set; }
        [Parameter] public string? CustomInfo { get; set; }
        [Parameter] public bool Visible { get; set; } = true;

        [Parameter] public EventCallback ButtomClicked { get; set; }
        [Parameter] public object? ButtomCssIcon { get; set; }
        [Parameter] public string? ButtomTitle { get; set; }

        [Parameter] public TValue? SelectedValue { get; set; }
        [Parameter] public EventCallback<TValue> SelectedValueChanged { get; set; }

        [Parameter] public IReadOnlyList<TEnum>? SelectedValues { get; set; }
        [Parameter] public EventCallback<IReadOnlyList<TEnum>> SelectedValuesChanged { get; set; }

        private string? Description => For?.GetCustomAttribute()?.Description;

        [Parameter] public Func<EnumObject, object> Order { get; set; } = o => o.Value;

        //protected Task UpdateDataHelp(Expression<Func<TValue>> For)
        //{
        //    return ModalService.Show<ProfileDataHelp<TValue, TEnum>>(For.GetCustomAttribute()?.Name,
        //        x =>
        //        {
        //            x.Add(x => x.HasGroup, ShowGroup);
        //        },
        //        new ModalInstanceOptions()
        //        {
        //            UseModalStructure = true,
        //            Centered = true,
        //            Size = ModalSize.Default,
        //        });
        //}

        protected Task UpdateDataSelect(Expression<Func<TValue>>? For)
        {
            return ModalService.Show<ProfileDataSelect<TValue, TEnum>>("",
                x =>
                {
                    x.Add(x => x.HasGroup, ShowGroup);
                    x.Add(x => x.SelectedValues, SelectedValues);
                    x.Add(x => x.SelectedValuesChanged, SelectedValuesChanged);
                    x.Add(x => x.Order, Order);
                    x.Add(x => x.Title, For?.GetCustomAttribute()?.Name);
                },
                new ModalInstanceOptions()
                {
                    UseModalStructure = false,
                    Centered = true,
                    Size = ModalSize.Default,
                });
        }
    }
}