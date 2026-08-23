using Microsoft.AspNetCore.Components;

namespace Wyrm.Components.Explorer;

public partial class RepositoryToolbar : ComponentBase
{
    private bool _menuOpen;

    [Parameter] public bool CanDelete { get; set; }
    [Parameter] public EventCallback OnAddRequested { get; set; }
    [Parameter] public EventCallback OnDeleteRequested { get; set; }
    [Parameter] public EventCallback OnBackupRequested { get; set; }

    private void ToggleMenu() => _menuOpen = !_menuOpen;

    private void CloseMenu() => _menuOpen = false;
}
