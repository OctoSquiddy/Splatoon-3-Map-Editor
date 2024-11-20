using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core.ViewModels
{
    public struct NodeIcon
    {
        private readonly string Text;

        public uint Color { get; set; } 

        public NodeIcon(string text, uint color = 0xFFFFFFFF) {
            Text = text;
            Color = color;
        }

        public static implicit operator String(NodeIcon value) {
            return value.Text;
        }

        public static explicit operator NodeIcon(string value) {
            return new NodeIcon(value);
        }

        public override string ToString() => Text;
    }
}
