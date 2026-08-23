using Microsoft.AspNetCore.Components;

namespace Wyrm.Components.Designer;

public partial class DesignerToolbar : ComponentBase
{
    private bool _menuOpen;

    [Parameter] public EventCallback OnAddRequested { get; set; }

    private void ToggleMenu() => _menuOpen = !_menuOpen;

    private void CloseMenu() => _menuOpen = false;
}
