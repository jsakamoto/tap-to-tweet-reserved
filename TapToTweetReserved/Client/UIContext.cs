using System;
using System.Collections.Generic;

namespace TapToTweetReserved.Client
{
    public class UIContext
    {
        private readonly List<MenuItem> _MenuItems = new List<MenuItem>();

        public IEnumerable<MenuItem> MenuItems => _MenuItems;

        public event EventHandler StateHasChanged;

        public void SetMenuItem(params MenuItem[] menuItems)
        {
            this._MenuItems.Clear();
            this._MenuItems.AddRange(menuItems);
            this.StateHasChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
