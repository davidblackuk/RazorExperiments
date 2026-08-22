using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Wyrm.Models;

namespace Wyrm.Components.Designer;

public partial class AssociationPropertyTypeGrid : ComponentBase
{
    [Parameter] public AssociationType? AssociationType { get; set; }
    [Parameter] public List<AssociationPropertyType>? PropertyTypes { get; set; }
    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public int? SelectedPropertyTypeId { get; set; }
    [Parameter] public EventCallback<int> OnPropertyTypeSelected { get; set; }
    [Parameter] public EventCallback<int> OnEditRequested { get; set; }
    [Parameter] public EventCallback<int> OnDeleteRequested { get; set; }

    private int? _openActionsMenuId;

    private Task HandleRowClick(GridRowEventArgs<AssociationPropertyType> args) =>
        OnPropertyTypeSelected.InvokeAsync(args.Item.Id);

    private string GetRowClass(AssociationPropertyType propertyType) =>
        SelectedPropertyTypeId == propertyType.Id ? "table-active" : string.Empty;

    private void ToggleActionsMenu(int propertyTypeId) =>
        _openActionsMenuId = _openActionsMenuId == propertyTypeId ? null : propertyTypeId;

    private void CloseActionsMenu() => _openActionsMenuId = null;
}
