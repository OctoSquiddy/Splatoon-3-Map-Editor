using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Core
{
    public class BindGUI : Attribute
    {
        /// <summary>
        /// The label displayed next to the control.
        /// </summary>
        public string Label
        {
            get
            {
                if (LabelConverter != null)
                    return LabelConverter.Convert(_label);
                return _label;
            }
            set
            {
                _label = value;
            }
        }

        public int Order { get; set; } = 99;

        private string _label;

        /// <summary>
        /// The category displayed (adds control to a drop panel).
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The tooltip displayed when hovered.
        /// </summary>
        public string ToolTip { get; set; }

        /// <summary>
        /// The index for the column the control is displayed on
        /// If none is set then controls will stack by default order.
        /// </summary>
        public int ColumnIndex { get; set; } = -1;

        /// <summary>
        /// The index for the row the control is displayed on.
        /// If none is set then controls will stack by default order.
        /// </summary>
        public int RowIndex { get; set; } = -1;

        public BindControl Control { get; set; }

        public IStringConverter LabelConverter { get; set; }

        public bool HasDropdown { get; set; } = true;

        public IDictionary<object, string> ComboBoxData { get; set; } = new Dictionary<object, string>();

        public BindGUI(string text = "")
        {
            Label = text;
        }

        public BindGUI(string text, object[] ComboBoxDataKey, string[] ComboBoxDataValue)
        {
            Label = text;

            for (int i = 0; i < ComboBoxDataKey.Length; i++)
            {
                ComboBoxData.Add(ComboBoxDataKey[i], ComboBoxDataValue[i]);
            }
        }

        public BindGUI(string text, int column, int row)
        {
            Label = text;
            ColumnIndex = column;
            RowIndex = row;
        }
    }

    public class BindNumberBox : Attribute
    {
        public float Max { get; set; } = 9999999999;
        public float Min { get; set; } = -99999999999;
        public float Increment { get; set; } = 1;
    }

    public class BindSlider : Attribute
    {
        public float Max { get; set; } = 9999999999;
        public float Min { get; set; } = -99999999999;
        public float Increment { get; set; } = 1;
    }

    public enum BindControl
    {
        Default,
        ComboBox,
        ColorBox,
        ToggleButton,
    }
}
