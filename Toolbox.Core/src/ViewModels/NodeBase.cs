using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Numerics;
using System.ComponentModel;

namespace Toolbox.Core.ViewModels
{
    public class NodeBase : ISelectableElement, INotifyPropertyChanged
    {
        public bool HasBeenEditedWithSelector = false;

        public EventHandler OnSelected;
        public EventHandler OnChecked;
        public EventHandler OnHeaderRenamed;

        public EventHandler IconDrawer;

        private readonly ObservableCollection<NodeBase> _children = new ObservableCollection<NodeBase>();

        /// <summary>
        /// Gets the children of the tree node.
        /// </summary>
        public virtual ObservableCollection<NodeBase> Children
        {
            get { return _children; }
        }

        /// <summary>
        /// Gets or sets a list of menu items for the tree node.
        /// </summary>
        public virtual List<MenuItemModel> ContextMenus { get; set; } = new List<MenuItemModel>();

        /// <summary>
        /// Gets or sets the parent of the tree node.
        /// </summary>
        public NodeBase Parent { get; set; }

        /// <summary> 
        /// Gets or sets a UI drawer for the tag property.
        /// </summary>
        public NodePropertyUI TagUI { get; set; } = new NodePropertyUI();

        public int DisplayIndex = -1;

        public Func<string> GetHeader;

        private string _header;

        /// <summary>
        /// Gets or sets the header of the tree node.
        /// </summary>
        public virtual string Header 
        {
            get { return GetHeader(); }
            set
            {
                GetHeader = () => { return value; };
                RaisePropertyChanged("Header");
            }
        }

        private bool _hasCheckBox = false;

        /// <summary>
        /// Determines if the node has a checkbox next to it or not.
        /// </summary>
        public virtual bool HasCheckBox
        {
            get { return _hasCheckBox; }
            set
            {
                _hasCheckBox = value;
                RaisePropertyChanged("HasCheckBox");
            }
        }

        private object tag;

        /// <summary>
        /// Gets or sets the node tag, used for node properties.
        /// </summary>
        public virtual object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        private bool _isExpanded;

        /// <summary>
        /// Determines if the tree node is expanded or not.
        /// </summary>
        public virtual bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                OnBeforeExpand();
                _isExpanded = value;
                RaisePropertyChanged("IsExpanded");
            }
        }

        private bool _isSelected;

        /// <summary>
        /// Determines if the tree node is selected or not.
        /// </summary>
        public virtual bool IsSelected
        {
            get {
                return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    RaisePropertyChanged("IsSelected");
                    OnSelected?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Determines if the tree node is visible or not.
        /// </summary>
        public bool Visible { get; set; }

        public bool ActivateRename { get; set; }

        private int _index;

        /// <summary>
        /// Gets the index of the node.
        /// </summary>
        public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                RaisePropertyChanged("Index");
            }
        }

        private string _icon = "\\Images\\Folder.png";

        /// <summary>
        /// Gets the icon key or character of the node.
        /// </summary>
        public virtual string Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                RaisePropertyChanged("Icon");
                ImageSource = new Uri(_icon, UriKind.Relative);
            }
        }

        public virtual Vector4 IconColor { get; set; } = new Vector4(1);

        private object imageSoure = new Uri("\\Images\\Folder.png", UriKind.Relative);

        /// <summary>
        /// Gets or sets the raw image source of the Icon value.
        /// </summary>
        public virtual object ImageSource
        {
            get { return imageSoure; }
            set
            {
                SetImageSource(value);
                RaisePropertyChanged("ImageSource");
            }
        }

        private void SetImageSource(object value)
        {
            if (value is Uri)
                imageSoure = (Uri)value;
            else if (value is string)
                imageSoure = (string)value;
            else
                imageSoure = value;
        }

        /// <summary>
        /// Expands all parenting nodes attached to this tree node.
        /// </summary>
        public void ExpandParent()
        {
            if (Parent != null && !Parent.IsExpanded)
                Parent.IsExpanded = true;

            if (Parent != null)
                Parent.ExpandParent();
        }

        private bool _isChecked = true;

        /// <summary>
        /// Determines if the node has been checked or not by a checkbox.
        /// </summary>
        public virtual bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                foreach (var child in Children)
                    child.IsChecked = value;

                RaisePropertyChanged("IsChecked");
                OnChecked?.Invoke(value, EventArgs.Empty);
            }
        }

        private bool _canRename = false;

        /// <summary>
        /// Determines if the node can be renamed or not.
        /// </summary>
        public virtual bool CanRename
        {
            get {
                if (Tag is IRenamableNode)
                    return true;

                return _canRename; }
            set
            {
                _canRename = value;
                RaisePropertyChanged("CanRename");
            }
        }

        /// <summary>
        /// Gets a unique ID of the tree node generated at runtime.
        /// </summary>
        public string ID { get; private set; }

        static Random randomIDPool = new Random();

        public NodeBase()  {
            ReloadID();
            Init();
        }

        public NodeBase(string name) {
            Header = name;
            ReloadID();
            Init();
        }

        private void ReloadID() {
            ID = $"##node_{randomIDPool.Next()}";
        }

        private void Init()
        {
            this.Children.CollectionChanged += children_CollectionChanged;
        }

        /// <summary>
        /// Adds the child to the node while also setting it's parent.
        /// </summary>
        public void AddChild(NodeBase child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public void Sort<T>(ObservableCollection<T> collection, Comparison<T> comparison)
        {
            var sortableList = new List<T>(collection);
            sortableList.Sort(comparison);

            for (int i = 0; i < sortableList.Count; i++) {
                collection.Move(collection.IndexOf(sortableList[i]), i);
            }
        }

        public bool SuppressUpdate = false;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string memberName = "")
        {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(memberName));
            }
        }

        void children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //Update indices when the collection has been altered.
            for (int i = 0; i < Children.Count; i++)
                Children[i].Index = i;
        }

        /// <summary>
        /// Called during a mouse double clicked operation.
        /// </summary>
        public virtual void OnDoubleClicked()
        {

        }

        /// <summary>
        /// Called before node has been expanded.
        /// </summary>
        public virtual void OnBeforeExpand()
        {

        }

        /// <summary>
        /// Called after node has been collapsed.
        /// </summary>
        public virtual void OnAfterCollapse()
        {

        }

        public override string ToString()
        {
            return Header;
        }
    }
}
