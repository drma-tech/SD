using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;

namespace SD.WEB.Modules.Profile;

public class FormBase<TValue> : ComponentBase
{
    [Parameter] public Expression<Func<TValue>>? For { get; set; }
    [Parameter] public bool Disabled { get; set; }
}