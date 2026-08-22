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

    [Parameter] public EventCallback<Repository> OnAddAssociationTypeRequested { get; set; }
    [Parameter] public int? SelectedAssociationTypeId { get; set; }
    [Parameter] public EventCallback<AssociationType> OnAssociationTypeSelected { get; set; }
    [Parameter] public EventCallback<AssociationType> OnAddAssociationPropertyTypeRequested { get; set; }
    [Parameter] public EventCallback<AssociationType> OnEditAssociationTypeRequested { get; set; }
    [Parameter] public EventCallback<AssociationType> OnDeleteAssociationTypeRequested { get; set; }

    private readonly HashSet<int> _collapsedRepositoryIds = new();
    private readonly HashSet<int> _collapsedObjectTypesGroupIds = new();
    private readonly HashSet<int> _collapsedAssociationTypesGroupIds = new();

    private int? _openRepositoryMenuId;
    private int? _openObjectTypesGroupMenuId;
    private int? _openAssociationTypesGroupMenuId;
    private int? _openObjectTypeMenuId;
    private int? _openAssociationTypeMenuId;

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

    private void ToggleObjectTypesGroup(int repositoryId)
    {
        if (!_collapsedObjectTypesGroupIds.Add(repositoryId))
        {
            _collapsedObjectTypesGroupIds.Remove(repositoryId);
        }
    }

    private bool IsObjectTypesGroupCollapsed(int repositoryId) => _collapsedObjectTypesGroupIds.Contains(repositoryId);

    private void ToggleAssociationTypesGroup(int repositoryId)
    {
        if (!_collapsedAssociationTypesGroupIds.Add(repositoryId))
        {
            _collapsedAssociationTypesGroupIds.Remove(repositoryId);
        }
    }

    private bool IsAssociationTypesGroupCollapsed(int repositoryId) => _collapsedAssociationTypesGroupIds.Contains(repositoryId);

    private void ToggleRepositoryMenu(int repositoryId) =>
        _openRepositoryMenuId = _openRepositoryMenuId == repositoryId ? null : repositoryId;

    private void ToggleObjectTypesGroupMenu(int repositoryId) =>
        _openObjectTypesGroupMenuId = _openObjectTypesGroupMenuId == repositoryId ? null : repositoryId;

    private void ToggleAssociationTypesGroupMenu(int repositoryId) =>
        _openAssociationTypesGroupMenuId = _openAssociationTypesGroupMenuId == repositoryId ? null : repositoryId;

    private void ToggleObjectTypeMenu(int objectTypeId) =>
        _openObjectTypeMenuId = _openObjectTypeMenuId == objectTypeId ? null : objectTypeId;

    private void ToggleAssociationTypeMenu(int associationTypeId) =>
        _openAssociationTypeMenuId = _openAssociationTypeMenuId == associationTypeId ? null : associationTypeId;

    private void CloseMenus()
    {
        _openRepositoryMenuId = null;
        _openObjectTypesGroupMenuId = null;
        _openAssociationTypesGroupMenuId = null;
        _openObjectTypeMenuId = null;
        _openAssociationTypeMenuId = null;
    }
}
