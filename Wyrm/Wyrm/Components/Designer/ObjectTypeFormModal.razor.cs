using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Wyrm.Models;
using Wyrm.ViewModels;

namespace Wyrm.Components.Designer;

public partial class ObjectTypeFormModal : ComponentBase
{
    [Parameter] public EventCallback<ObjectTypeFormInput> OnSave { get; set; }

    private Modal? _modal;
    private string _title = "New Object Type";
    private int? _id;
    private string _name = string.Empty;
    private string _pluralName = string.Empty;
    private string _description = string.Empty;
    private string? _error;

    public Task ShowAsync()
    {
        _title = "New Object Type";
        _id = null;
        _name = string.Empty;
        _pluralName = string.Empty;
        _description = string.Empty;
        _error = null;
        return _modal?.ShowAsync() ?? Task.CompletedTask;
    }

    public Task ShowAsync(ObjectType objectType)
    {
        _title = $"Edit {objectType.Name}";
        _id = objectType.Id;
        _name = objectType.Name;
        _pluralName = objectType.PluralName;
        _description = objectType.Description;
        _error = null;
        return _modal?.ShowAsync() ?? Task.CompletedTask;
    }

    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(_name))
        {
            _error = "Name is required.";
            return;
        }

        if (string.IsNullOrWhiteSpace(_pluralName))
        {
            _error = "Plural name is required.";
            return;
        }

        _error = null;
        await OnSave.InvokeAsync(new ObjectTypeFormInput { Id = _id, Name = _name.Trim(), PluralName = _pluralName.Trim(), Description = _description.Trim() });

        if (_modal != null)
        {
            await _modal.HideAsync();
        }
    }
}
