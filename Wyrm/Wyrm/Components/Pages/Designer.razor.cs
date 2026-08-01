using System.Security.Claims;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Wyrm.Components.Designer;
using Wyrm.Components.Shared;
using Wyrm.Data;
using Wyrm.Models;
using Wyrm.Services;
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
        await using var context = await DbContextFactory.CreateDbContextAsync();
        _repositories = await context.Repositories
            .Include(r => r.ObjectTypes)
            .OrderBy(r => r.Name)
            .ToListAsync();
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
        var now = DateTime.UtcNow;
        await using var context = await DbContextFactory.CreateDbContextAsync();

        if (input.Id.HasValue)
        {
            var repository = await context.Repositories.FindAsync(input.Id.Value);
            if (repository != null)
            {
                repository.Name = input.Name;
                repository.Description = input.Description;
                repository.UpdatedById = userId;
                repository.UpdatedAt = now;
                await context.SaveChangesAsync();
            }
        }
        else
        {
            context.Repositories.Add(new Repository
            {
                Name = input.Name,
                Description = input.Description,
                CreatedById = userId,
                CreatedAt = now,
                UpdatedById = userId,
                UpdatedAt = now
            });
            await context.SaveChangesAsync();
        }

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

        await using var context = await DbContextFactory.CreateDbContextAsync();
        var toDelete = await context.Repositories
            .Include(r => r.ObjectTypes)
            .FirstOrDefaultAsync(r => r.Id == repository.Id);

        if (toDelete != null)
        {
            if (toDelete.ObjectTypes.Any())
            {
                _repositoryError = $"Cannot delete '{toDelete.Name}' because it still contains object types.";
                return;
            }

            context.Repositories.Remove(toDelete);
            await context.SaveChangesAsync();
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

        await using var context = await DbContextFactory.CreateDbContextAsync();
        var loaded = await context.ObjectTypes
            .Include(o => o.Repository)
            .Include(o => o.PropertyTypes.OrderBy(pt => pt.Id))
            .FirstAsync(o => o.Id == objectType.Id);

        _selectedObjectType = loaded;
        _propertyTypes = loaded.PropertyTypes.ToList();
        _gridLoading = false;
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
        var now = DateTime.UtcNow;
        await using var context = await DbContextFactory.CreateDbContextAsync();

        if (input.Id.HasValue)
        {
            var objectType = await context.ObjectTypes.FindAsync(input.Id.Value);
            if (objectType != null)
            {
                objectType.Name = input.Name;
                objectType.Description = input.Description;
                objectType.UpdatedById = userId;
                objectType.UpdatedAt = now;
                await context.SaveChangesAsync();
            }
        }
        else if (_newObjectTypeRepositoryId.HasValue)
        {
            var objectType = new ObjectType
            {
                Name = input.Name,
                Description = input.Description,
                RepositoryId = _newObjectTypeRepositoryId.Value,
                CreatedById = userId,
                CreatedAt = now,
                UpdatedById = userId,
                UpdatedAt = now,
                PropertyTypes = ObjectTypeSystemProperties.CreateDefaults(userId, now)
            };
            context.ObjectTypes.Add(objectType);
            await context.SaveChangesAsync();
        }

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

        await using var context = await DbContextFactory.CreateDbContextAsync();
        var toDelete = await context.ObjectTypes.FindAsync(objectType.Id);
        if (toDelete != null)
        {
            context.ObjectTypes.Remove(toDelete);
            await context.SaveChangesAsync();
        }

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

        await using var context = await DbContextFactory.CreateDbContextAsync();
        _selectedPropertyType = await context.PropertyTypes
            .Include(p => p.CreatedBy)
            .Include(p => p.UpdatedBy)
            .FirstAsync(p => p.Id == propertyTypeId);

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
        var now = DateTime.UtcNow;
        await using var context = await DbContextFactory.CreateDbContextAsync();

        if (input.Id.HasValue)
        {
            var propertyType = await context.PropertyTypes.FindAsync(input.Id.Value);
            if (propertyType != null)
            {
                propertyType.Name = input.Name;
                propertyType.Description = input.Description;
                propertyType.DataType = input.DataType;
                propertyType.UpdatedById = userId;
                propertyType.UpdatedAt = now;
                await context.SaveChangesAsync();
            }
        }
        else if (_newPropertyTypeObjectTypeId.HasValue)
        {
            context.PropertyTypes.Add(new PropertyType
            {
                Name = input.Name,
                Description = input.Description,
                DataType = input.DataType,
                ObjectTypeId = _newPropertyTypeObjectTypeId.Value,
                CreatedById = userId,
                CreatedAt = now,
                UpdatedById = userId,
                UpdatedAt = now
            });
            await context.SaveChangesAsync();
        }

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

        await using var context = await DbContextFactory.CreateDbContextAsync();
        var toDelete = await context.PropertyTypes.FindAsync(propertyType.Id);
        if (toDelete != null)
        {
            context.PropertyTypes.Remove(toDelete);
            await context.SaveChangesAsync();
        }

        if (_selectedPropertyType?.Id == propertyType.Id)
        {
            _selectedPropertyType = null;
        }

        await SelectObjectTypeAsync(_selectedObjectType);
    }
}
