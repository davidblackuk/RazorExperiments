using Microsoft.AspNetCore.Components;
using Wyrm.ViewModels;

namespace Wyrm.Components.Explorer;

public partial class InstanceDetail : ComponentBase
{
    [Parameter] public ExplorerInstanceDetailViewModel? Detail { get; set; }
    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public EventCallback OnEditRequested { get; set; }
    [Parameter] public EventCallback OnDeleteRequested { get; set; }
}
