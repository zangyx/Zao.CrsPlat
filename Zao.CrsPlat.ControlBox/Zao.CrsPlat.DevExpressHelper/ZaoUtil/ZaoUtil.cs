using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;

namespace Zao.CrsPlat.DevExpressHelper.ZaoUtil
{

    public class ZaoUtil {

        #region 成员

        private GridControl _aimGridControl;
        private GridView _aimGridView;
        private TreeList _aimTreeList;
        private RibbonControl _aimRibbonControl;
        private string[] _aimShowingEditor;


        #endregion 成员

        #region 属性

        public GridView AimGridView
        {
            get
            {
                return _aimGridView;
            }

            set
            {
                _aimGridView = value;
            }
        }

        public TreeList AimTreeList
        {
            get
            {
                return _aimTreeList;
            }

            set
            {
                _aimTreeList = value;
            }
        }

        public RibbonControl AimRibbonControl
        {
            get
            {
                return _aimRibbonControl;
            }

            set
            {
                _aimRibbonControl = value;
            }
        }

        public GridControl AimGridControl
        {
            get
            {
                if (_aimGridControl is null) { _aimGridControl = AimGridView.GridControl; }
                return _aimGridControl;
            }

            set
            {
                _aimGridControl = value;
            }
        }

        /// <summary>
        /// 设置哪些列可编辑
        /// </summary>
        public string[] AimShowingEditor
        {
            get
            {
                return _aimShowingEditor;
            }

            set
            {
                _aimShowingEditor = value;
            }
        }

        #endregion 属性

        #region 构造

        public ZaoUtil(GridControl gridControl, GridView gridView) {
            AimGridControl = gridControl;
            AimGridView = gridView;
        }

        public ZaoUtil(GridView gridView) {
            AimGridView = gridView;
        }

        public ZaoUtil(TreeList treeList) {
            AimTreeList = treeList;
        }

        public ZaoUtil(RibbonControl ribbonControl) {
            AimRibbonControl = ribbonControl;
        }
        public ZaoUtil(RibbonControl ribbonControl, GridView gridView) {
            AimRibbonControl = ribbonControl;
            AimGridView = gridView;
        }

        /// <summary>
        /// 切换目标GridView
        /// </summary>
        /// <param name="gridView"></param>
        /// <returns></returns>
        public ZaoUtil CutOverGridView(GridView gridView) {
            AimGridView = gridView;
            return this;
        }

        #endregion 构造

        #region 对公工具方法

        #region GridView

        /// <summary>
        /// 表头改为复选框
        /// </summary>
        /// <returns></returns>
        public ZaoUtil ColumnMultiCheck() {
            for (var i = 0; i < AimGridView.Columns.Count; i++) {
                _aimGridView.Columns[i].OptionsFilter.FilterPopupMode
                     = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
            }
            return this;
        }

        /// <summary>
        /// 自适应所有列列宽(返回ZaoUtil提供连续.设置)
        /// </summary>
        public ZaoUtil ColumnsBestFit() {
            _aimGridView.BestFitColumns();
            return this;
        }

        /// <summary>
        /// 设置列是否隐藏
        /// </summary>
        /// <param name="colName">列名</param>
        /// <param name="ifHide">隐藏/不隐藏（true/false）</param>
        public ZaoUtil ColumnSetHide(string colName, bool ifHide) {
            if (_aimGridView.Columns.ColumnByFieldName(colName) == null) {
                return this;
            }
            _aimGridView.Columns[colName].Visible = ifHide;
            return this;
        }

        /// <summary>
        /// 设置列宽度
        /// </summary>
        /// <param name="colName">列名</param>
        /// <param name="width">宽度</param>
        public ZaoUtil ColumnSetWidth(string colName, int width) {
            if (_aimGridView.Columns.ColumnByFieldName(colName) == null) {
                return this;
            }
            _aimGridView.Columns[colName].Width = width;
            return this;
        }

        /// <summary>
        /// 设置序号列的列宽
        /// </summary>
        /// <param name="width"></param>
        public ZaoUtil IndicatorSetWidth(int width) {
            _aimGridView.IndicatorWidth = width;
            return this;
        }

        /// <summary>
        /// 设置固定到左边的列数
        /// </summary>
        /// <param name="length">固定到左边的列数</param>
        /// <returns></returns>
        public ZaoUtil ColumnFixCount(int length) {
            for (var i = 0; i < length; i++) {
                _aimGridView.VisibleColumns[i].Fixed
                    = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            }
            return this;
        }

        /// <summary>
        /// 设置带checkbox选择列
        /// </summary>
        /// <returns></returns>
        public ZaoUtil SetCheckBoxSelect() {
            _aimGridView.OptionsSelection.MultiSelectMode
                = GridMultiSelectMode.CheckBoxRowSelect;
            return this;
        }

        /// <summary>
        /// 设置多行列名（具体内部属性的功能待测试确定）
        /// </summary>
        /// <param name="wordWrap">列头以及需要面板文字的包裹效果</param>
        /// <param name="rowHorzAlignment">行文本的水平方向的位置属性</param>
        /// <param name="headerHorzAlignment">列头的文本水平方向的位置属性</param>
        /// <returns></returns>
        public ZaoUtil ColNameSetMultiRow(DevExpress.Utils.WordWrap wordWrap = DevExpress.Utils.WordWrap.Wrap,
            DevExpress.Utils.HorzAlignment rowHorzAlignment = DevExpress.Utils.HorzAlignment.Near,
            DevExpress.Utils.HorzAlignment headerHorzAlignment = DevExpress.Utils.HorzAlignment.Center
            ) {
            _aimGridView.OptionsView.AllowHtmlDrawHeaders = true;
            _aimGridView.Appearance.HeaderPanel.TextOptions.WordWrap = wordWrap;
            _aimGridView.Appearance.Row.TextOptions.HAlignment = rowHorzAlignment;
            _aimGridView.Appearance.HeaderPanel.TextOptions.HAlignment = headerHorzAlignment;
            ColumnMultiCheck();
            return this;
        }

        /// <summary>
        /// 设置样式保存
        /// </summary>
        public ZaoUtil GridViewSaveStyle() {
            string path = string.Format("{0}\\ViewStyle\\Style_{1}_{2}.xml",
                Application.StartupPath,
                _aimGridView.GridControl.FindForm().Name,
                _aimGridView.Name);
            string dic = string.Format("{0}\\ViewStyle\\",
                Application.StartupPath);
            if (!Directory.Exists(dic)) Directory.CreateDirectory(dic);
            _aimGridView.SaveLayoutToXml(path);
            return this;
        }

        /// <summary>
        /// 设置清除样式
        /// </summary>
        public ZaoUtil GridViewDeleteStyle() {
            string path = string.Format("{0}\\ViewStyle\\Style_{1}_{2}.xml",
                Application.StartupPath,
                _aimGridView.GridControl.FindForm().Name,
                _aimGridView.Name);
            if (File.Exists(path)) File.Delete(path);
            return this;
        }

        /// <summary>
        /// 设置载入样式
        /// </summary>
        public ZaoUtil GridViewLoadStyle() {
            string path = string.Format("{0}\\ViewStyle\\Style_{1}_{2}.xml",
                Application.StartupPath,
                _aimGridView.GridControl.FindForm().Name,
                _aimGridView.Name);
            if (File.Exists(path)) _aimGridView.RestoreLayoutFromXml(path);
            return this;
        }

        /// <summary>
        /// 设置gridControl显示分页控件
        /// </summary>
        /// <returns></returns>
        public ZaoUtil GridControlInitStyleEmbeddedNavigator()
        {
            AimGridControl.UseEmbeddedNavigator = true;
            AimGridControl.EmbeddedNavigator.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            AimGridControl.EmbeddedNavigator.Buttons.Append.Visible = false;
            AimGridControl.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            AimGridControl.EmbeddedNavigator.Buttons.Edit.Visible = false;
            AimGridControl.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            AimGridControl.EmbeddedNavigator.Buttons.Remove.Enabled = false;
            AimGridControl.EmbeddedNavigator.Buttons.Remove.Visible = false;
            AimGridControl.EmbeddedNavigator.TextStringFormat = "当前：{0}/{1}页";
            return this;
        }

        /// <summary>
        /// 初始化GridView可以编辑通用样式
        /// </summary>
        /// <returns></returns>
        public ZaoUtil GridViewInitStyleEditable(params string[] cols) {
            _aimGridView.OptionsView.ShowFooter = true;
            _aimGridView.OptionsBehavior.Editable = true;
            _aimGridView.OptionsView.ShowGroupPanel = false;
            _aimGridView.OptionsView.ColumnAutoWidth = false;
            _aimGridView.OptionsSelection.MultiSelect = true;
            _aimGridView.OptionsView.ShowAutoFilterRow = true;
            _aimGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
            _aimGridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            IndicatorSetWidth(50).ColNameSetMultiRow().GridViewLoadStyle();
            _aimGridView.CustomDrawRowIndicator += GridViewDrawRowIndicator;
            /************设定不可编辑列**************/
            AimShowingEditor = cols;
            _aimGridView.ShowingEditor += _aimGridView_NotShowingEditor;
            /************设定不可编辑列**************/
            return this;
        }

        /// <summary>
        /// 初始化GridView不可编辑通用样式
        /// </summary>
        /// <returns></returns>
        public ZaoUtil GridViewInitStyleNotEditable(params string[] cols) {
            _aimGridView.OptionsView.ShowFooter = true;
            _aimGridView.OptionsBehavior.Editable = true;
            _aimGridView.OptionsView.ShowGroupPanel = false;
            _aimGridView.OptionsView.ColumnAutoWidth = false;
            _aimGridView.OptionsSelection.MultiSelect = true;
            _aimGridView.OptionsView.ShowAutoFilterRow = true;
            _aimGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
            _aimGridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            IndicatorSetWidth(50).ColNameSetMultiRow().GridViewLoadStyle();
            _aimGridView.CustomDrawRowIndicator += GridViewDrawRowIndicator;
            /************设定可编辑列**************/
            AimShowingEditor = cols;
            _aimGridView.ShowingEditor += _aimGridView_ShowingEditor;
            /************设定可编辑列**************/
            return this;
        }

        private void _aimGridView_NotShowingEditor(object sender, CancelEventArgs e) {
            if (AimShowingEditor.Length == 0) { e.Cancel = false; return; }
            for (int i = 0; i < AimShowingEditor.Length; i++) {
                if ((sender as GridView).FocusedColumn.FieldName == AimShowingEditor[i]) {
                    e.Cancel = true;
                    break;
                } else {
                    e.Cancel = false;
                }
            }
        }

        /// <summary>
        /// 初始化GridView行勾选通用样式-1
        /// </summary>
        /// <returns></returns>
        public ZaoUtil GridViewInitStyleCheckRowEdit(params string[] cols) {
            _aimGridView.OptionsView.ShowFooter = true;
            _aimGridView.OptionsBehavior.Editable = true;
            _aimGridView.OptionsView.ShowGroupPanel = false;
            _aimGridView.OptionsView.ColumnAutoWidth = false;
            _aimGridView.OptionsSelection.MultiSelect = true;
            _aimGridView.OptionsView.ShowAutoFilterRow = true;
            _aimGridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            _aimGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
            IndicatorSetWidth(50).ColNameSetMultiRow().GridViewLoadStyle();
            _aimGridView.CustomDrawRowIndicator += GridViewDrawRowIndicator;
            /************设定可编辑列**************/
            AimShowingEditor = cols;
            _aimGridView.ShowingEditor += _aimGridView_ShowingEditor;
            /************设定可编辑列**************/

            return this;
        }

        /// <summary>
        /// 初始化GridView行勾选通用样式-2(分多步完成)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _aimGridView_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e) {
            if (AimShowingEditor.Length == 0) { e.Cancel = true; return; }
            for (int i = 0; i < AimShowingEditor.Length; i++) {
                if ((sender as GridView).FocusedColumn.FieldName == AimShowingEditor[i]) {
                    e.Cancel = false;
                    break;
                } else {
                    e.Cancel = true;
                }
            }
        }

        /*事件方法体*/

        /// <summary>
        /// 设置行序号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GridViewDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e) {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0) {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        /// <summary>
        /// 从表展开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GridViewMasterRowExpanded(object sender, CustomMasterRowEventArgs e) {
            var detailView = (sender as GridView).GetDetailView(e.RowHandle, e.RelationIndex) as GridView;
            //new ZaoUtil(detailView).IniStyleUnEditable2();
            detailView.MasterRowExpanded += GridViewMasterRowExpanded;
            //detailView.CustomDrawRowIndicator += new GridViewHelper(detailView).gridV_CustomDrawRowIndicator;
        }

        /// <summary>
        /// 保存样式按钮方法体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GridViewSaveStyle(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            _aimGridView.CloseEditor();/*以备用户编辑时未触发单元格变更事件就点了保存样式按钮*/
            GridViewSaveStyle();
        }

        /// <summary>
        /// 清除样式按钮方法体
        /// </summary>
        public void GridViewDeleteStyle(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            GridViewDeleteStyle();
        }

        #endregion GridView

        #region RibbonControl

        /// <summary>
        /// 初始化RibbonControl通用样式
        /// </summary>
        /// <returns></returns>
        public ZaoUtil RibbonInitStyle() {
            foreach (RibbonPage itemPage in _aimRibbonControl.Pages) {
                foreach (RibbonPageGroup itemPageGroup in itemPage.Groups) {
                    itemPageGroup.ShowCaptionButton = false;
                }
            }
            _aimRibbonControl.RibbonStyle = RibbonControlStyle.Office2013;
            _aimRibbonControl.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            _aimRibbonControl.ShowFullScreenButton = DevExpress.Utils.DefaultBoolean.False;
            _aimRibbonControl.ShowToolbarCustomizeItem = false;
            _aimRibbonControl.Toolbar.ShowCustomizeItem = false;
            return this;
        }

        #endregion RibbonControl

        #region TreeList

        /// <summary>
        /// 初始化样式
        /// </summary>
        /// <returns></returns>
        public ZaoUtil TreeListInitStyle() {
            _aimTreeList.OptionsBehavior.Editable = false;
            _aimTreeList.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
            return this;
        }

        /// <summary>
        /// 指定父子列
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="son"></param>
        /// <returns></returns>
        public ZaoUtil TreeListSetParentSon(string parent, string son) {
            _aimTreeList.ParentFieldName = parent;
            _aimTreeList.KeyFieldName = son;
            return this;
        }

        /// <summary>
        /// 设置列的显示名称
        /// </summary>
        /// <param name="field"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public ZaoUtil ColumnSetNewName(string field, string newName) {
            _aimTreeList.Columns[field].Caption = newName;
            return this;
        }

        /// <summary>
        /// 设置保存样式
        /// </summary>
        public ZaoUtil TreeListSaveStyle() {
            string path = string.Format("{0}\\ViewStyle\\Style_{1}_{2}.xml",
                Application.StartupPath,
                _aimTreeList.FindForm().Name,
                _aimTreeList.Name);
            string dic = string.Format("{0}\\ViewStyle\\",
                Application.StartupPath);
            if (!Directory.Exists(dic)) Directory.CreateDirectory(dic);
            _aimTreeList.SaveLayoutToXml(path);
            return this;
        }

        /// <summary>
        /// 保存样式按钮方法体
        /// </summary>
        public void TreeListSaveStyle(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            _aimTreeList.CloseEditor();/*以备用户编辑时未触发单元格变更事件就点了保存样式按钮*/
            TreeListSaveStyle();
            XtraMessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 设置载入样式
        /// </summary>
        public ZaoUtil TreeListLoadStyle() {
            string path = string.Format("{0}\\ViewStyle\\Style_{1}_{2}.xml",
                Application.StartupPath,
                _aimTreeList.FindForm().Name,
                _aimTreeList.Name);
            if (File.Exists(path)) _aimTreeList.RestoreLayoutFromXml(path);
            return this;
        }

        /// <summary>
        /// 设置清除样式
        /// </summary>
        public ZaoUtil TreeListDeleteStyle() {
            string path = string.Format("{0}\\ViewStyle\\Style_{1}_{2}.xml",
                Application.StartupPath,
                _aimTreeList.FindForm().Name,
                _aimTreeList.Name);
            if (File.Exists(path)) File.Delete(path);
            return this;
        }

        /// <summary>
        /// 清除样式按钮方法体
        /// </summary>
        public void TreeListDeleteStyle(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            TreeListDeleteStyle();
        }

        /// <summary>
        /// 自定义各层节点的图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TreeListCustomDrawNodeImages(object sender, CustomDrawNodeImagesEventArgs e) {
            if (e.Node.Nodes.Count > 0) {
                if (e.Node.Expanded) {
                    e.SelectImageIndex = 2;
                    return;
                }
                e.SelectImageIndex = 1;
            } else {
                e.SelectImageIndex = 0;
            }
        }

        /// <summary>
        /// 展开所有节点
        /// </summary>
        public void TreeListExpandAllNodes(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            _aimTreeList.ExpandAll();
        }

        /// <summary>
        /// 折叠所有节点
        /// </summary>
        public void TreeListCollapseAllNodes(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            _aimTreeList.CollapseAll();
        }

        /// <summary>
        /// 展开主要节点
        /// </summary>
        public void TreeListExpandMainNodes(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            _aimTreeList.CollapseAll();
            _aimTreeList.ExpandToLevel(0);
        }

        /// <summary>
        /// 勾选全部树结点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TreeListSelectAllNodes(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            var lNodes = _aimTreeList.GetNodeList();
            foreach (var node in lNodes) {
                node.CheckState = CheckState.Checked;
            }
        }

        /// <summary>
        /// 全部树节点不勾选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TreeListNotSelectAllNodes(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            var lNodes = _aimTreeList.GetNodeList();
            foreach (var node in lNodes) {
                node.CheckState = CheckState.Unchecked;
            }
        }

        /*权限编辑功能相关*/

        /// <summary>
        /// 树节点勾选之前判断状态,然后设为相反状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TreeListBeforeCheckNode(object sender, CheckNodeEventArgs e) {
            e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
            (sender as TreeList).AfterCheckNode += TreeListAfterCheckNode;
        }

        /// <summary>
        /// 勾选后响应事件加载父子状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TreeListAfterCheckNode(object sender, NodeEventArgs e) {
            //父
            ParentNodesSetChecked(e.Node, e.Node.CheckState);
            //子
            ChildNodesSetChecked(e.Node, e.Node.CheckState);
        }

        /// <summary>
        /// 递归设置子节点的状态
        /// </summary>
        /// <param name="node"></param>
        /// <param name="check"></param>
        private void ChildNodesSetChecked(TreeListNode node, CheckState check) {
            for (var i = 0; i < node.Nodes.Count; i++) {
                node.Nodes[i].CheckState = check;
                ChildNodesSetChecked(node.Nodes[i], check);
            }
        }

        /// <summary>
        /// 递归设置父节点的状态
        /// </summary>
        /// <param name="node"></param>
        /// <param name="check"></param>
        private void ParentNodesSetChecked(TreeListNode node, CheckState check) {
            if (node.ParentNode != null) {
                var b = false;
                CheckState state;
                for (var i = 0; i < node.ParentNode.Nodes.Count; i++) {
                    state = (CheckState)node.ParentNode.Nodes[i].CheckState;
                    //只要有一个兄弟与他状态不一致,就把父亲设为待定状态
                    if (!check.Equals(state)) {
                        b = !b;
                        break;
                    }
                }
                node.ParentNode.CheckState = b ? CheckState.Indeterminate : check;
                ParentNodesSetChecked(node.ParentNode, check);
            }
        }

        /*权限编辑功能相关*/

        #endregion TreeList

        #endregion 对公工具方法
    }
}