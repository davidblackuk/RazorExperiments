using Microsoft.AspNetCore.Components;
using Wyrm.Models;

namespace Wyrm.Components.Designer;

public partial class SchemaTree : ComponentBase
{
    [Parameter, EditorRequired] public required List<Repository> Repositories { get; set; }

    [Parameter] public int? SelectedRepositoryId { get; set; }
    [Parameter] public EventCallback<Repository> OnRepositorySelected { get; set; }
    [Parameter] public EventCallback<Repository> OnAddObjectTypeRequested { get; set; }
    [Parameter] public EventCallback<Repository> OnEditRepositoryRequested { get; set; }
    [Parameter] public EventCallback<Repository> OnDeleteRepositoryRequested { get; set; }

    [Parameter] public int? SelectedObjectTypeId { get; set; }
    [Parameter] public EventCallback<ObjectType> OnObjectTypeSelected { get; set; }
    [Parameter] public EventCallback<ObjectType> OnAddPropertyTypeRequested { get; set; }
    [Parameter] public EventCallback<ObjectType> OnEditObjectTypeRequested { get; set; }
    [Parameter] public EventCallback<ObjectType> OnDeleteObjectTypeRequested { get; set; }

    private readonly HashSet<int> _collapsedRepositoryIds = new();
    private int? _openRepositoryMenuId;
    private int? _openObjectTypeMenuId;

    private void ToggleRepository(int repositoryId)
    {
        if (!_collapsedRepositoryIds.Add(repositoryId))
        {
            _collapsedRepositoryIds.Remove(repositoryId);
        }
    }

    private async Task SelectRepository(Repository repository)
    {
        ToggleRepository(repository.Id);
        await OnRepositorySelected.InvokeAsync(repository);
    }

    private bool IsCollapsed(int repositoryId) => _collapsedRepositoryIds.Contains(repositoryId);

    private void ToggleRepositoryMenu(int repositoryId) =>
        _openRepositoryMenuId = _openRepositoryMenuId == repositoryId ? null : repositoryId;

    private void ToggleObjectTypeMenu(int objectTypeId) =>
        _openObjectTypeMenuId = _openObjectTypeMenuId == objectTypeId ? null : objectTypeId;

    private void CloseMenus()
    {
        _openRepositoryMenuId = null;
        _openObjectTypeMenuId = null;
    }
}
