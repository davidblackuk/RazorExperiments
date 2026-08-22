using Microsoft.AspNetCore.Components;
using Wyrm.Services;
using Wyrm.ViewModels;

namespace Wyrm.Components.Explorer;

public partial class InstanceDetail : ComponentBase
{
    [Parameter] public ExplorerInstanceDetailViewModel? Detail { get; set; }
    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public EventCallback OnEditRequested { get; set; }
    [Parameter] public EventCallback OnDeleteRequested { get; set; }

    [Parameter] public List<AssociatedObjectRow>? AssociatedObjects { get; set; }
    [Parameter] public bool AssociatedObjectsLoading { get; set; }
    [Parameter] public EventCallback OnAddAssociationRequested { get; set; }
    [Parameter] public EventCallback<int> OnDeleteAssociationRequested { get; set; }
}
