using System.Security.Claims;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Wyrm.Components.Designer;
using Wyrm.Components.Shared;
using Wyrm.Models;
using Wyrm.ViewModels;

namespace Wyrm.Components.Pages;

public partial class Designer : ComponentBase
{
    private List<Repository> _repositories = new();
    private int? _selectedRepositoryId;
    private string? _repositoryError;
    private int? _newObjectTypeRepositoryId;

    private ObjectType? _selectedObjectType;
    private List<PropertyType>? _propertyTypes;
    private bool _gridLoading;
    private int? _newPropertyTypeObjectTypeId;

    private PropertyType? _selectedPropertyType;
    private bool _detailLoading;

    private RepositoryFormModal? _repositoryFormModal;
    private ObjectTypeFormModal? _objectTypeFormModal;
    private PropertyTypeFormModal? _propertyTypeFormModal;
    private ConfirmDialog? _confirmDialog;

    [CascadingParameter]
    private Task<AuthenticationState>? AuthStateTask { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await ReloadRepositoriesAsync();
        await SelectFirstObjectTypeAsync();
    }

    private async Task SelectFirstObjectTypeAsync()
    {
        var firstObjectType = _repositories
            .OrderBy(r => r.Name)
            .SelectMany(r => r.ObjectTypes.OrderBy(ot => ot.Name))
            .FirstOrDefault();

        if (firstObjectType == null)
        {
            return;
        }

        _selectedRepositoryId = firstObjectType.RepositoryId;
        await SelectObjectTypeAsync(firstObjectType);
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

    private async Task ReloadRepositoriesAsync()
    {
        _repositories = await RepositoryService.GetAllWithObjectTypesAsync();
    }

    // --- Repository ---

    private void SelectRepository(Repository repository)
    {
        _selectedRepositoryId = repository.Id;
    }

    private Task OpenCreateRepositoryForm() =>
        _repositoryFormModal?.ShowAsync() ?? Task.CompletedTask;

    private Task OpenEditRepositoryForm(Repository repository) =>
        _repositoryFormModal?.ShowAsync(repository) ?? Task.CompletedTask;

    private async Task SaveRepositoryAsync(RepositoryFormInput input)
    {
        var userId = await GetUserIdAsync();
        await RepositoryService.SaveAsync(input, userId);
        await ReloadRepositoriesAsync();
    }

    private async Task DeleteRepositoryAsync(Repository repository)
    {
        if (_confirmDialog == null)
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
            _propertyTypes = null;
            _selectedPropertyType = null;
        }

        await ReloadRepositoriesAsync();
    }

    // --- ObjectType ---

    private async Task SelectObjectTypeAsync(ObjectType objectType)
    {
        _selectedPropertyType = null;
        _gridLoading = true;
        StateHasChanged();

        var loaded = await ObjectTypeService.GetForDesignerAsync(objectType.Id);

        _selectedObjectType = loaded;
        _propertyTypes = loaded.PropertyTypes.ToList();
        _gridLoading = false;

        var firstPropertyType = _propertyTypes.FirstOrDefault();
        if (firstPropertyType != null)
        {
            await SelectPropertyTypeAsync(firstPropertyType.Id);
        }
    }

    private Task OpenCreateObjectTypeForm(Repository repository)
    {
        _newObjectTypeRepositoryId = repository.Id;
        return _objectTypeFormModal?.ShowAsync() ?? Task.CompletedTask;
    }

    private Task OpenEditObjectTypeForm(ObjectType objectType)
    {
        _newObjectTypeRepositoryId = null;
        return _objectTypeFormModal?.ShowAsync(objectType) ?? Task.CompletedTask;
    }

    private async Task SaveObjectTypeAsync(ObjectTypeFormInput input)
    {
        var userId = await GetUserIdAsync();
        await ObjectTypeService.SaveAsync(input, _newObjectTypeRepositoryId, userId);

        _newObjectTypeRepositoryId = null;
        await ReloadRepositoriesAsync();

        if (_selectedObjectType != null && _selectedObjectType.Id == input.Id)
        {
            await SelectObjectTypeAsync(_selectedObjectType);
        }
    }

    private async Task DeleteObjectTypeAsync(ObjectType objectType)
    {
        if (_confirmDialog == null)
        {
            return;
        }

        var confirmed = await _confirmDialog.ShowAsync(
            title: $"Delete: {objectType.Name}",
            message1: $"Are you sure you want to delete the object type '{objectType.Name}'?",
            message2: "This will delete all of its property types and instances. There is no undo operation for this action.",
            confirmDialogOptions: new ConfirmDialogOptions { YesButtonText = "Delete", YesButtonColor = ButtonColor.Danger });

        if (!confirmed)
        {
            return;
        }

        await ObjectTypeService.DeleteAsync(objectType.Id);

        if (_selectedObjectType?.Id == objectType.Id)
        {
            _selectedObjectType = null;
            _propertyTypes = null;
            _selectedPropertyType = null;
        }

        await ReloadRepositoriesAsync();
    }

    // --- PropertyType ---

    private async Task SelectPropertyTypeAsync(int propertyTypeId)
    {
        _detailLoading = true;
        StateHasChanged();

        _selectedPropertyType = await PropertyTypeService.GetWithAuditUsersAsync(propertyTypeId);

        _detailLoading = false;
    }

    private Task OpenCreatePropertyTypeForm(ObjectType objectType)
    {
        _newPropertyTypeObjectTypeId = objectType.Id;
        return _propertyTypeFormModal?.ShowAsync() ?? Task.CompletedTask;
    }

    private Task OpenEditPropertyTypeForm(PropertyType propertyType)
    {
        _newPropertyTypeObjectTypeId = null;
        return _propertyTypeFormModal?.ShowAsync(propertyType) ?? Task.CompletedTask;
    }

    private Task OpenEditPropertyTypeForRowAsync(int propertyTypeId)
    {
        var propertyType = _propertyTypes?.FirstOrDefault(p => p.Id == propertyTypeId);
        return propertyType != null ? OpenEditPropertyTypeForm(propertyType) : Task.CompletedTask;
    }

    private Task OpenEditFormForSelectedPropertyTypeAsync() =>
        _selectedPropertyType != null ? OpenEditPropertyTypeForm(_selectedPropertyType) : Task.CompletedTask;

    private async Task SavePropertyTypeAsync(PropertyTypeFormInput input)
    {
        var userId = await GetUserIdAsync();
        await PropertyTypeService.SaveAsync(input, _newPropertyTypeObjectTypeId, userId);

        _newPropertyTypeObjectTypeId = null;

        if (_selectedObjectType != null)
        {
            await SelectObjectTypeAsync(_selectedObjectType);
        }

        if (input.Id.HasValue)
        {
            await SelectPropertyTypeAsync(input.Id.Value);
        }
    }

    private Task DeletePropertyTypeForRowAsync(int propertyTypeId)
    {
        var propertyType = _propertyTypes?.FirstOrDefault(p => p.Id == propertyTypeId);
        return propertyType != null ? DeletePropertyTypeAsync(propertyType) : Task.CompletedTask;
    }

    private Task DeletePropertyTypeForSelectedAsync() =>
        _selectedPropertyType != null ? DeletePropertyTypeAsync(_selectedPropertyType) : Task.CompletedTask;

    private async Task DeletePropertyTypeAsync(PropertyType propertyType)
    {
        if (_confirmDialog == null || _selectedObjectType == null)
        {
            return;
        }

        var confirmed = await _confirmDialog.ShowAsync(
            title: $"Delete: {propertyType.Name}",
            message1: $"Are you sure you want to delete the property type '{propertyType.Name}'?",
            message2: "This will delete all values stored for it on existing instances. There is no undo operation for this action.",
            confirmDialogOptions: new ConfirmDialogOptions { YesButtonText = "Delete", YesButtonColor = ButtonColor.Danger });

        if (!confirmed)
        {
            return;
        }

        await PropertyTypeService.DeleteAsync(propertyType.Id);

        if (_selectedPropertyType?.Id == propertyType.Id)
        {
            _selectedPropertyType = null;
        }

        await SelectObjectTypeAsync(_selectedObjectType);
    }
}
