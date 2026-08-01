using Microsoft.AspNetCore.Components;

namespace Wyrm.Components.Designer;

public partial class DesignerToolbar : ComponentBase
{
    [Parameter] public EventCallback OnAddRequested { get; set; }
}
