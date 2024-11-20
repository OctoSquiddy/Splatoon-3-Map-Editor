using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using MapStudio.UI;
using Toolbox.Core;

namespace UIFramework
{
    /// <summary>
    /// Represents a single tree node for rendering into a tree view UI.
    /// </summary>
    public class TreeNode : ISelectableElement
    {
        public EventHandler OnHeaderChanged;
        public EventHandler OnSelected;
        public EventHandler OnChecked;
        public EventHandler OnHeaderRenamed;

        //Render events
        public EventHandler IconDrawer;
        public EventHandler RenderOverride;

        /// <summary>
        /// Gets or sets the header of the tree node.
        /// </summary>
        public virtual string Header
        {
            get { return _header == null ? "<NULL>" : _header; }
            set
            {
                if (_header != value) {
                    _header = value;
                    OnHeaderChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private string _header;

        /// <summary>
        /// Gets or sets the parent of the tree node.
        /// </summary>
        public TreeNode Parent { get; set; }

        /// <summary>
        /// Gets the children of the tree node.
        /// </summary>
        public virtual ObservableCollection<TreeNode> Children
        {
            get { return _children; }
        }

        private readonly ObservableCollection<TreeNode> _children = new ObservableCollection<TreeNode>();

        /// <summary>
        /// Gets or sets a list of menu items for the tree node.
        /// </summary>
        public virtual List<MenuItem> ContextMenus { get; set; } = new List<MenuItem>();

        /// <summary>
        /// Determines if the node has a checkbox next to it or not.
        /// </summary>
        public virtual bool HasCheckBox { get; set; } = false;

        /// <summary>
        /// Determines if the node can rename or not.
        /// </summary>
        public virtual bool CanRename { get; set; } = false;

        /// <summary>
        /// Makes the current tree node in rename mode;
        /// </summary>
        public virtual bool ActivateRename { get; set; } = false;

        /// <summary>
        /// Determines if the node can drag/drop or not.
        /// </summary>
        public bool CanDragDrop { get; set; }

        /// <summary>
        /// Gets or sets the node tag, used for node properties.
        /// </summary>
        public virtual object Tag { get; set; }

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
                OnAfterCollapse();
            }
        }

        private bool _isExpanded;

        /// <summary>
        /// Determines if the tree node is selected or not.
        /// </summary>
        public virtual bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnSelected?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private bool _isSelected;

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

                OnChecked?.Invoke(value, EventArgs.Empty);
            }
        }

        private bool _isChecked = true;

        /// <summary>
        /// Represents an icon drawn next to the tree node header.
        /// </summary>
        public virtual string Icon { get; set; } = IconManager.FOLDER_ICON.ToString();

        public System.Numerics.Vector4 IconColor = new System.Numerics.Vector4(1);

        /// <summary>
        /// Gets the index of the node.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Gets a unique ID of the tree node generated at runtime.
        /// </summary>
        public string ID { get; private set; }

        //pool of IDs to create unique IDs for UI nodes
        static Random randomIDPool = new Random();

        public TreeNode() {
            Init();
        }

        public TreeNode(string name) {
            Header = name;
            Init();
        }

        /// <summary>
        /// Adds the child to the node while also setting it's parent.
        /// </summary>
        public void AddChild(TreeNode child)
        {
            child.Parent = this;
            Children.Add(child);
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

        /// <summary>
        /// Sorts the tree children in ascending order.
        /// </summary>
        public void Sort() => Sort(this.Children.OrderBy(o => o.Header).ToList());

        /// <summary>
        /// Sorts the tree children in descending order.
        /// </summary>
        public void SortByDescending() => Sort(this.Children.OrderByDescending(o => o.Header).ToList());

        /// <summary>
        /// Called during a mouse double clicked operation.
        /// </summary>
        public virtual void OnDoubleClicked()
        {
            if (!IsExpanded && Children.Count > 0)
                this.IsExpanded = true;
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

        public override string ToString() {
            return Header;
        }

        private void Init() {
            //assign a unique ID
            ReloadID();
            //add events for lists
            this.Children.CollectionChanged += children_CollectionChanged;
        }

        private void ReloadID() {
            ID = $"##node_{randomIDPool.Next()}";
        }

        private void Sort(List<TreeNode> sortableList) {
            for (int i = 0; i < sortableList.Count; i++) {
                this.Children.Move(this.Children.IndexOf(sortableList[i]), i);
            }
        }

        private void children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            //Update indices when the collection has been altered.
            for (int i = 0; i < Children.Count; i++)
                Children[i].Index = i;
        }
    }
}
