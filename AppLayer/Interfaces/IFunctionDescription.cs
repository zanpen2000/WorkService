using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppLayer.Interfaces
{
    public interface IFunctionDescription
    {
        /// <summary>
        /// 图标
        /// </summary>
        string Icon { get; }
        /// <summary>
        /// 编辑图标
        /// </summary>
        string EditIcon { get; }
        /// <summary>
        /// 显示名称
        /// </summary>
        string Displayname { get; }
        /// <summary>
        /// 类名
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 父功能类名
        /// </summary>
        string ParentName { get; }
        /// <summary>
        /// 提示
        /// </summary>
        string ToolTip { get; }
        /// <summary>
        /// 是否是菜单项
        /// </summary>
        bool IsMenu { get; }
    }
}
