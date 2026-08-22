using Microsoft.AspNetCore.Components;
using Wyrm.Models;

namespace Wyrm.Components.Designer;

public partial class AssociationPropertyTypeDetail : ComponentBase
{
    [Parameter] public AssociationPropertyType? PropertyType { get; set; }
    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public EventCallback OnEditRequested { get; set; }
    [Parameter] public EventCallback OnDeleteRequested { get; set; }
}
