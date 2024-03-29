﻿namespace TapToTweetReserved.Client;

public class UIContext
{
    private readonly List<MenuItem> _MenuItems = new List<MenuItem>();

    public IEnumerable<MenuItem> MenuItems => this._MenuItems;

    public event EventHandler? StateHasChanged;

    private Func<string> _PageTitle = () => "";

    public Func<string> PageTitle
    {
        get => this._PageTitle;
        set { this._PageTitle = value; this.StateHasChanged?.Invoke(this, EventArgs.Empty); }
    }

    public void SetMenuItem(params MenuItem[] menuItems)
    {
        this._MenuItems.Clear();
        this._MenuItems.AddRange(menuItems);
        this.StateHasChanged?.Invoke(this, EventArgs.Empty);
    }
}
