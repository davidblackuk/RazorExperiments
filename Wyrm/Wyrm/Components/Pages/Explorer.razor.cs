using System.Security.Claims;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Wyrm.Components.Explorer;
using Wyrm.Components.Shared;
using Wyrm.Models;
using Wyrm.Services;
using Wyrm.ViewModels;

namespace Wyrm.Components.Pages;

public partial class Explorer : ComponentBase
{
    private enum FormMode { Create, Edit }

    private List<Repository> _repositories = new();

    private int? _selectedRepositoryId;
    private string? _repositoryError;
    private RepositoryFormModal? _repositoryFormModal;
    private ConfirmDialog? _confirmDialog;

    private ObjectType? _selectedObjectType;
    private List<ExplorerInstanceRow>? _rows;
    private bool _gridLoading;

    private ObjectInstance? _selectedInstance;
    private ExplorerInstanceDetailViewModel? _selectedInstanceDetail;
    private bool _detailLoading;

    private InstanceFormModal? _instanceFormModal;
    private FormMode _formMode;
    private int? _editingInstanceId;

    [CascadingParameter]
    private Task<AuthenticationState>? AuthStateTask { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await ReloadRepositoriesAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("explorerSplitters.init", ".explorer-shell");
        }
    }

    private async Task<string> GetUserIdAsync()
    {
        var state = await AuthStateTask!;
        return state.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("User id claim missing.");
    }

    private async Task SelectObjectTypeAsync(ObjectType objectType)
    {
        _selectedInstance = null;
        _selectedInstanceDetail = null;
        _gridLoading = true;
        StateHasChanged();

        var view = await ObjectInstanceService.GetRowsForObjectTypeAsync(objectType.Id);

        _selectedObjectType = view.ObjectType;
        _rows = view.Rows;
        _gridLoading = false;
    }

    private async Task SelectInstanceAsync(int instanceId)
    {
        _detailLoading = true;
        StateHasChanged();

        _selectedInstanceDetail = await ObjectInstanceService.GetDetailAsync(instanceId);
        _selectedInstance = _selectedInstanceDetail.ObjectInstance;

        _detailLoading = false;
    }

    private Task OpenCreateForm()
    {
        if (_selectedObjectType == null || _instanceFormModal == null)
        {
            return Task.CompletedTask;
        }

        _formMode = FormMode.Create;
        _editingInstanceId = null;
        var fields = _selectedObjectType.PropertyTypes
            .Where(pt => !SystemPropertyNames.IsAuditMirror(pt.Name))
            .Select(pt => new PropertyFieldInput { PropertyTypeId = pt.Id, Name = pt.Name, Description = pt.Description, DataType = pt.DataType, RawValue = null })
            .ToList();

        return _instanceFormModal.ShowAsync($"New {_selectedObjectType.Name}", fields);
    }

    private async Task OpenEditFormAsync(int instanceId)
    {
        if (_instanceFormModal == null)
        {
            return;
        }

        _formMode = FormMode.Edit;
        _editingInstanceId = instanceId;

        var view = await ObjectInstanceService.GetEditFormFieldsAsync(instanceId);
        await _instanceFormModal.ShowAsync($"Edit {view.DisplayName}", view.Fields);
    }

    private Task OpenEditFormForRowAsync(int instanceId) => OpenEditFormAsync(instanceId);

    private Task OpenEditFormForSelectedAsync() =>
        _selectedInstance != null ? OpenEditFormAsync(_selectedInstance.Id) : Task.CompletedTask;

    private async Task SaveFormAsync()
    {
        if (_selectedObjectType == null || _instanceFormModal == null)
        {
            return;
        }

        var userId = await GetUserIdAsync();
        int savedInstanceId;

        if (_formMode == FormMode.Create)
        {
            savedInstanceId = await ObjectInstanceService.SaveAsync(null, _selectedObjectType.Id, _instanceFormModal.CurrentFields, userId);
        }
        else if (_formMode == FormMode.Edit && _editingInstanceId.HasValue)
        {
            savedInstanceId = await ObjectInstanceService.SaveAsync(_editingInstanceId.Value, _selectedObjectType.Id, _instanceFormModal.CurrentFields, userId);
        }
        else
        {
            return;
        }

        await SelectObjectTypeAsync(_selectedObjectType);
        await SelectInstanceAsync(savedInstanceId);
    }

    private Task OpenDeleteConfirmForRow(int instanceId)
    {
        var row = _rows?.FirstOrDefault(r => r.Id == instanceId);
        return DeleteInstanceAsync(instanceId, row?.DisplayName ?? $"Instance #{instanceId}");
    }

    private Task OpenDeleteConfirmForSelected() =>
        _selectedInstanceDetail != null
            ? DeleteInstanceAsync(_selectedInstanceDetail.ObjectInstance.Id, _selectedInstanceDetail.DisplayName)
            : Task.CompletedTask;

    private async Task DeleteInstanceAsync(int instanceId, string displayName)
    {
        if (_confirmDialog == null || _selectedObjectType == null)
        {
            return;
        }

        var confirmed = await _confirmDialog.ShowAsync(
            title: $"Delete: {displayName}",
            message1: $"Are you sure you want to delete {displayName}?",
            message2: "Deleting this instance will delete all of its property values. There is no undo operation for this action.",
            confirmDialogOptions: new ConfirmDialogOptions { YesButtonText = "Delete", YesButtonColor = ButtonColor.Danger });

        if (!confirmed)
        {
            return;
        }

        await ObjectInstanceService.DeleteAsync(instanceId);

        if (_selectedInstance?.Id == instanceId)
        {
            _selectedInstance = null;
            _selectedInstanceDetail = null;
        }

        await SelectObjectTypeAsync(_selectedObjectType);
    }

    private void SelectRepository(Repository repository)
    {
        _selectedRepositoryId = repository.Id;
    }

    private Task OpenCreateRepositoryForm() =>
        _repositoryFormModal?.ShowAsync() ?? Task.CompletedTask;

    private async Task SaveRepositoryAsync(RepositoryFormInput input)
    {
        var userId = await GetUserIdAsync();
        await RepositoryService.SaveAsync(input, userId);
        await ReloadRepositoriesAsync();
    }

    private async Task OpenDeleteRepositoryConfirm()
    {
        var repository = _selectedRepositoryId.HasValue
            ? _repositories.FirstOrDefault(r => r.Id == _selectedRepositoryId.Value)
            : null;

        if (repository == null || _confirmDialog == null)
        {
            return;
        }

        var confirmed = await _confirmDialog.ShowAsync(
            title: $"Delete: {repository.Name}",
            message1: $"Are you sure you want to delete the repository '{repository.Name}'?",
            message2: "This is only allowed while the repository has no object types.",
            confirmDialogOptions: new ConfirmDialogOptions { YesButtonText = "Delete", YesButtonColor = ButtonColor.Danger });

        if (!confirmed)
        {
            return;
        }

        var result = await RepositoryService.DeleteAsync(repository.Id);
        if (!result.Success)
        {
            _repositoryError = result.ErrorMessage;
            return;
        }

        if (_selectedRepositoryId == repository.Id)
        {
            _selectedRepositoryId = null;
        }

        if (_selectedObjectType?.RepositoryId == repository.Id)
        {
            _selectedObjectType = null;
            _rows = null;
            _selectedInstance = null;
            _selectedInstanceDetail = null;
        }

        await ReloadRepositoriesAsync();
    }

    private async Task ReloadRepositoriesAsync()
    {
        _repositories = await RepositoryService.GetAllWithObjectTypesAsync();
    }
}
