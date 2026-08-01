using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Wyrm.Models;
using Wyrm.ViewModels;

namespace Wyrm.Components.Shared;

public partial class RepositoryFormModal : ComponentBase
{
    [Parameter] public EventCallback<RepositoryFormInput> OnSave { get; set; }

    private Modal? _modal;
    private string _title = "New Repository";
    private int? _id;
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string? _error;

    public Task ShowAsync()
    {
        _title = "New Repository";
        _id = null;
        _name = string.Empty;
        _description = string.Empty;
        _error = null;
        return _modal?.ShowAsync() ?? Task.CompletedTask;
    }

    public Task ShowAsync(Repository repository)
    {
        _title = $"Edit {repository.Name}";
        _id = repository.Id;
        _name = repository.Name;
        _description = repository.Description;
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
        await OnSave.InvokeAsync(new RepositoryFormInput { Id = _id, Name = _name.Trim(), Description = _description.Trim() });

        if (_modal != null)
        {
            await _modal.HideAsync();
        }
    }
}
