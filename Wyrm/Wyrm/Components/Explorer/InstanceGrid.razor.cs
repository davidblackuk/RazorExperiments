using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Wyrm.Models;
using Wyrm.ViewModels;

namespace Wyrm.Components.Explorer;

public partial class InstanceGrid : ComponentBase
{
    [Parameter] public ObjectType? ObjectType { get; set; }
    [Parameter] public List<ExplorerInstanceRow>? Rows { get; set; }
    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public int? SelectedInstanceId { get; set; }
    [Parameter] public EventCallback<int> OnInstanceSelected { get; set; }
    [Parameter] public EventCallback OnCreateRequested { get; set; }
    [Parameter] public EventCallback<int> OnEditRequested { get; set; }
    [Parameter] public EventCallback<int> OnDeleteRequested { get; set; }

    private Task HandleRowClick(GridRowEventArgs<ExplorerInstanceRow> args) =>
        OnInstanceSelected.InvokeAsync(args.Item.Id);

    private string GetRowClass(ExplorerInstanceRow row) =>
        SelectedInstanceId == row.Id ? "table-active" : string.Empty;
}
