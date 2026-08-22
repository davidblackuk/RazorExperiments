using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Wyrm.Services;
using Wyrm.ViewModels;

namespace Wyrm.Components.Explorer;

public partial class AssociationInstanceFormModal : ComponentBase
{
    [Parameter] public EventCallback<AssociationInstanceFormInput> OnSave { get; set; }

    private Modal? _modal;
    private int _currentInstanceId;
    private List<EligibleAssociationOption> _options = new();
    private int _selectedOptionIndex;
    private EligibleAssociationOption? _selectedOption;
    private int? _selectedCandidateId;
    private List<PropertyFieldInput> _propertyFields = new();
    private List<string> _errors = new();

    public Task ShowAsync(int currentInstanceId, List<EligibleAssociationOption> options)
    {
        _currentInstanceId = currentInstanceId;
        _options = options;
        _selectedOptionIndex = 0;
        _errors = new List<string>();
        OnOptionChanged();
        return _modal?.ShowAsync() ?? Task.CompletedTask;
    }

    private void OnOptionChanged()
    {
        _selectedOption = _selectedOptionIndex >= 0 && _selectedOptionIndex < _options.Count ? _options[_selectedOptionIndex] : null;
        _selectedCandidateId = _selectedOption?.Candidates.FirstOrDefault()?.InstanceId;
        _propertyFields = _selectedOption?.PropertyFields
            .Select(f => new PropertyFieldInput { PropertyTypeId = f.PropertyTypeId, Name = f.Name, Description = f.Description, DataType = f.DataType, RawValue = f.RawValue })
            .ToList() ?? new List<PropertyFieldInput>();
    }

    private async Task Save()
    {
        _errors = new List<string>();

        if (_selectedOption == null || !_selectedCandidateId.HasValue)
        {
            _errors.Add("Select an object to link to.");
            return;
        }

        foreach (var field in _propertyFields)
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

        await OnSave.InvokeAsync(new AssociationInstanceFormInput
        {
            AssociationTypeId = _selectedOption.AssociationTypeId,
            CurrentInstanceIsSource = _selectedOption.CurrentInstanceIsSource,
            CurrentInstanceId = _currentInstanceId,
            OtherInstanceId = _selectedCandidateId.Value,
            PropertyFields = _propertyFields
        });

        if (_modal != null)
        {
            await _modal.HideAsync();
        }
    }
}
