using System;
using System.Collections.Generic;

namespace TapToTweetReserved.Client
{
    public class UIContext
    {
        private readonly List<MenuItem> _MenuItems = new List<MenuItem>();

        public IEnumerable<MenuItem> MenuItems => _MenuItems;

        public event EventHandler StateHasChanged;

        private string _PageTitle = "";

        public string PageTitle
        {
            get => _PageTitle;
            set { _PageTitle = value; this.StateHasChanged?.Invoke(this, EventArgs.Empty); }
        }

        public void SetMenuItem(params MenuItem[] menuItems)
        {
            this._MenuItems.Clear();
            this._MenuItems.AddRange(menuItems);
            this.StateHasChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
