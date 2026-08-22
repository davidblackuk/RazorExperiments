using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Wyrm.Models;
using Wyrm.ViewModels;

namespace Wyrm.Components.Designer;

public partial class AssociationTypeFormModal : ComponentBase
{
    [Parameter] public EventCallback<AssociationTypeFormInput> OnSave { get; set; }

    private Modal? _modal;
    private string _title = "New Association Type";
    private int? _id;
    private string _forwardName = string.Empty;
    private string _reverseName = string.Empty;
    private string _description = string.Empty;
    private int? _sourceObjectTypeId;
    private int? _targetObjectTypeId;
    private List<ObjectType> _objectTypes = new();
    private string? _error;

    public Task ShowAsync(List<ObjectType> objectTypes)
    {
        _title = "New Association Type";
        _id = null;
        _forwardName = string.Empty;
        _reverseName = string.Empty;
        _description = string.Empty;
        _sourceObjectTypeId = null;
        _targetObjectTypeId = null;
        _objectTypes = objectTypes;
        _error = null;
        return _modal?.ShowAsync() ?? Task.CompletedTask;
    }

    public Task ShowAsync(AssociationType associationType, List<ObjectType> objectTypes)
    {
        _title = $"Edit {associationType.ForwardName} / {associationType.ReverseName}";
        _id = associationType.Id;
        _forwardName = associationType.ForwardName;
        _reverseName = associationType.ReverseName;
        _description = associationType.Description;
        _sourceObjectTypeId = associationType.SourceObjectTypeId;
        _targetObjectTypeId = associationType.TargetObjectTypeId;
        _objectTypes = objectTypes;
        _error = null;
        return _modal?.ShowAsync() ?? Task.CompletedTask;
    }

    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(_forwardName))
        {
            _error = "Forward name is required.";
            return;
        }

        if (string.IsNullOrWhiteSpace(_reverseName))
        {
            _error = "Reverse name is required.";
            return;
        }

        _error = null;
        await OnSave.InvokeAsync(new AssociationTypeFormInput
        {
            Id = _id,
            ForwardName = _forwardName.Trim(),
            ReverseName = _reverseName.Trim(),
            Description = _description.Trim(),
            SourceObjectTypeId = _sourceObjectTypeId,
            TargetObjectTypeId = _targetObjectTypeId
        });

        if (_modal != null)
        {
            await _modal.HideAsync();
        }
    }
}
