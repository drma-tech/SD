using System.Linq.Expressions;
using Blazorise;
using Microsoft.AspNetCore.Components;

namespace SD.WEB.Modules.Profile;

public partial class FieldSelect<TValue, TEnum> : FormBase<TValue, FieldSelect<TValue, TEnum>>
    where TEnum : struct, Enum, IConvertible
{
    [Parameter] public string? CssIcon { get; set; }
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool Multiple { get; set; }
    [Parameter] public bool ShowGroup { get; set; }
    [Parameter] public bool ShowSwitch { get; set; }
    [Parameter] public bool ShowHelper { get; set; } = true;
    [Parameter] public bool ShowDescription { get; set; } = true;
    [Parameter] public string? HelpLink { get; set; }
    [Parameter] public string? CustomInfo { get; set; }
    [Parameter] public bool Visible { get; set; } = true;

    [Parameter] public EventCallback ButtonClicked { get; set; }
    [Parameter] public object? ButtonCssIcon { get; set; }
    [Parameter] public string? ButtonTitle { get; set; }

    [Parameter] public TValue? SelectedValue { get; set; }
    [Parameter] public EventCallback<TValue> SelectedValueChanged { get; set; }

    [Parameter] public IReadOnlyList<TEnum>? SelectedValues { get; set; }
    [Parameter] public EventCallback<IReadOnlyList<TEnum>> SelectedValuesChanged { get; set; }

    private string? Description => For?.GetCustomAttribute().Description;

    [Parameter] public Func<EnumObject, object> Order { get; set; } = o => o.Value;

    protected Task UpdateDataSelect(Expression<Func<TValue>>? @for)
    {
        return ModalService.Show<ProfileDataSelect<TEnum>>("",
            x =>
            {
                x.Add(x => x.HasGroup, ShowGroup);
                x.Add(x => x.SelectedValues, SelectedValues);
                x.Add(x => x.SelectedValuesChanged, SelectedValuesChanged);
                x.Add(x => x.Order, Order);
                x.Add(x => x.Title, @for?.GetCustomAttribute().Name);
            },
            new ModalInstanceOptions
            {
                UseModalStructure = false,
                Centered = true,
                Size = ModalSize.Default
            });
    }
}