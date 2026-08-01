using Microsoft.AspNetCore.Components;

namespace Wyrm.Components.Explorer;

public partial class RepositoryToolbar : ComponentBase
{
    [Parameter] public bool CanDelete { get; set; }
    [Parameter] public EventCallback OnAddRequested { get; set; }
    [Parameter] public EventCallback OnDeleteRequested { get; set; }
}
