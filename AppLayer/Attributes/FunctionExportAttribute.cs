using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace AppLayer.Attributes
{
    using Interfaces;

    [MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple=false,Inherited=true)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class FunctionExportAttribute : ExportAttribute, IFunctionDescription
    {
        public FunctionExportAttribute(Type contractType, string icon, string eicon, string dname, string name, string pname, string tooltip, bool ismenu)
            : base(contractType)
        {
            this.Icon = icon;
            this.EditIcon = eicon;
            this.Displayname = dname;
            this.Name = name;
            this.ParentName = pname;
            this.ToolTip = tooltip;
            this.IsMenu = ismenu;
        }

        public string Icon
        {
            get;
            private set;
        }

        public string EditIcon
        {
            get;
            private set;
        }

        public string Displayname
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public string ParentName
        {
            get;
            private set;
        }

        public string ToolTip
        {
            get;
            private set;
        }

        public bool IsMenu
        {
            get;
            private set;
        }
    }
}
