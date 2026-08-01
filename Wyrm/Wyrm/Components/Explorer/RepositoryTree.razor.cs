using Microsoft.AspNetCore.Components;
using Wyrm.Models;

namespace Wyrm.Components.Explorer;

public partial class RepositoryTree : ComponentBase
{
    [Parameter, EditorRequired] public required List<Repository> Repositories { get; set; }
    [Parameter] public int? SelectedObjectTypeId { get; set; }
    [Parameter] public EventCallback<ObjectType> OnObjectTypeSelected { get; set; }
    [Parameter] public int? SelectedRepositoryId { get; set; }
    [Parameter] public EventCallback<Repository> OnRepositorySelected { get; set; }

    private readonly HashSet<int> _collapsedRepositoryIds = new();

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
}
