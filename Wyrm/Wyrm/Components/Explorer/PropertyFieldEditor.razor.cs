using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Wyrm.ViewModels;

namespace Wyrm.Components.Explorer;

public partial class PropertyFieldEditor : ComponentBase
{
    [Parameter, EditorRequired] public required PropertyFieldInput Field { get; set; }

    private void OnInput(ChangeEventArgs e) => Field.RawValue = e.Value?.ToString();
}
