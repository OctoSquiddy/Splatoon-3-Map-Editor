using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Toolbox.Core.ViewModels
{
    public class MenuItemModel : BaseViewModel
    {
        private readonly ICommand _command;

        private string _header;
        public string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                RaisePropertyChanged("Header");
            }
        }


        public string Icon { get; set; }

        public string ToolTip { get; set; } = "";

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }

        public bool CanCheck = false;

        public List<MenuItemModel> MenuItems { get; set; }

        public ICommand Command
        {
            get
            {
                return _command;
            }
        }

        private string _template;
        public string Template
        {
            get
            {
                return _template;
            }
            set
            {
                _template = value;
            }
        }

        public bool IsEnabled { get; set; } = true;

        public object Tag { get; set; }

        public MenuItemModel() { }

        public MenuItemModel(string name)
        {
            Header = name;
            MenuItems = new List<MenuItemModel>();
            _command = new CommandViewModel(Execute);
        }

        public MenuItemModel(string name, EventHandler clicked, string toolTip = "", bool isChecked = false)
        {
            Header = name;
            _command = new CommandViewModel(() => { clicked?.Invoke(this, EventArgs.Empty); });
            MenuItems = new List<MenuItemModel>();
            ToolTip = toolTip;
            IsChecked = isChecked;
        }

        public MenuItemModel(string name, Action clicked, string toolTip = "", bool isChecked = false)
        {
            Header = name;
            _command = new CommandViewModel(clicked);
            MenuItems = new List<MenuItemModel>();
            ToolTip = toolTip;
            IsChecked = isChecked;
        }

        private void Execute() { }
    }
}
