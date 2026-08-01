using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Wyrm.Services;
using Wyrm.ViewModels;

namespace Wyrm.Components.Explorer;

public partial class InstanceFormModal : ComponentBase
{
    [Parameter] public EventCallback OnSave { get; set; }

    public IReadOnlyList<PropertyFieldInput> CurrentFields => _fields;

    private Modal? _modal;
    private string _title = string.Empty;
    private List<PropertyFieldInput> _fields = new();
    private List<string> _errors = new();

    public Task ShowAsync(string title, List<PropertyFieldInput> fields)
    {
        _title = title;
        _fields = fields;
        _errors = new List<string>();
        return _modal?.ShowAsync() ?? Task.CompletedTask;
    }

    private async Task Save()
    {
        _errors = new List<string>();

        foreach (var field in _fields)
        {
            if (!PropertyValueParser.TryValidate(field.DataType, field.RawValue, out var error))
            {
                _errors.Add($"{field.Name}: {error}");
            }
        }

        if (_errors.Count > 0)
        {
            return;
        }

        await OnSave.InvokeAsync();

        if (_modal != null)
        {
            await _modal.HideAsync();
        }
    }
}
