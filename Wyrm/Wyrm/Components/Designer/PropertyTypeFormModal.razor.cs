using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Wyrm.Models;
using Wyrm.ViewModels;

namespace Wyrm.Components.Designer;

public partial class PropertyTypeFormModal : ComponentBase
{
    [Parameter] public EventCallback<PropertyTypeFormInput> OnSave { get; set; }

    private Modal? _modal;
    private string _title = "New Property Type";
    private int? _id;
    private string _name = string.Empty;
    private string _description = string.Empty;
    private PropertyDataType _dataType;
    private string? _error;

    public Task ShowAsync()
    {
        _title = "New Property Type";
        _id = null;
        _name = string.Empty;
        _description = string.Empty;
        _dataType = PropertyDataType.String;
        _error = null;
        return _modal?.ShowAsync() ?? Task.CompletedTask;
    }

    public Task ShowAsync(PropertyType propertyType)
    {
        _title = $"Edit {propertyType.Name}";
        _id = propertyType.Id;
        _name = propertyType.Name;
        _description = propertyType.Description;
        _dataType = propertyType.DataType;
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

        _error = null;
        await OnSave.InvokeAsync(new PropertyTypeFormInput { Id = _id, Name = _name.Trim(), Description = _description.Trim(), DataType = _dataType });

        if (_modal != null)
        {
            await _modal.HideAsync();
        }
    }
}
