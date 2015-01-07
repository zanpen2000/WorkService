using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfClient.Models
{
    internal class PropertyNodeItem
    {
        public string Icon { get; set; }
        public string EditIcon { get; set; }
        public string DisplayName { get; set; }
        public string ToolTip { get; set; }
        public int id { get; set; }

        public List<PropertyNodeItem> Childen { get; set; }
        public PropertyNodeItem()
        {
            Childen = new List<PropertyNodeItem>();
        }
    }
}
