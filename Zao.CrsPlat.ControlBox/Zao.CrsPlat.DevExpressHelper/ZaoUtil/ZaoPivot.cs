using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;


namespace Zao.CrsPlat.DevExpressHelper.ZaoUtil
{
    public class ZaoPivot {

        #region 成员

        private PivotGridControl _pivotGridControl;
        private ChartControl _chartControl;
        private PivotToolCollection _pivotToolCollection;
        private ViewType[] _viewType;

        #endregion 成员

        #region 属性

        /// <summary>
        /// 目标统计表
        /// </summary>
        public PivotGridControl AimPivotGridControl
        {
            get
            {
                return _pivotGridControl;
            }

            set
            {
                _pivotGridControl = value;
            }
        }

        /// <summary>
        /// 目标图表
        /// </summary>
        public ChartControl AimChartControl
        {
            get
            {
                return _chartControl;
            }

            set
            {
                _chartControl = value;
            }
        }

        /// <summary>
        /// 界面按钮勾选等集合
        /// </summary>
        public PivotToolCollection AimPivotToolCollection
        {
            get
            {
                return _pivotToolCollection;
            }

            set
            {
                _pivotToolCollection = value;
            }
        }

        /// <summary>
        /// 图表类型包含的集合
        /// </summary>
        public ViewType[] ViewTypeArray
        {
            get
            {
                _viewType = new ViewType[] {
                    ViewType.PolarArea,
                    ViewType.PolarLine,
                    ViewType.ScatterPolarLine,
                    ViewType.SideBySideGantt,
                    ViewType.Bubble,
                    ViewType.SideBySideRangeBar,
                    ViewType.RangeBar,
                    ViewType.Gantt,
                    ViewType.PolarPoint,
                    ViewType.Stock,
                    ViewType.CandleStick,
                    ViewType.SideBySideFullStackedBar,
                    ViewType.SideBySideFullStackedBar3D,
                    ViewType.SideBySideStackedBar,
                    ViewType.SideBySideStackedBar3D };
                return _viewType;
            }

            set
            {
                _viewType = value;
            }
        }

        #endregion 属性

        #region 构造

        public ZaoPivot(PivotGridControl pivotGridControl, ChartControl chartControl, PivotToolCollection toolCollection) {
            AimPivotGridControl = pivotGridControl;
            AimChartControl = chartControl;
            AimPivotToolCollection = toolCollection;

            chartControl.DataSource = pivotGridControl;
            chartControl.SeriesTemplate.ChangeView(ViewType.Bar);
            pivotGridControl.OptionsChartDataSource.ProvideDataByColumns = AimPivotToolCollection.CeChartDataVertical.Checked;
            pivotGridControl.OptionsChartDataSource.SelectionOnly = AimPivotToolCollection.CeSelectionOnly.Checked;
            pivotGridControl.OptionsChartDataSource.ProvideColumnGrandTotals = AimPivotToolCollection.CeShowColumnGrandTotals.Checked;
            pivotGridControl.OptionsChartDataSource.ProvideRowGrandTotals = AimPivotToolCollection.CeShowRowGrandTotals.Checked;

            chartControl.CrosshairOptions.ShowArgumentLine = true;
            chartControl.SeriesTemplate.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;

            AimPivotToolCollection.BarSelectSeries.EditValueChanged += BarSelectSeries_EditValueChanged;
            AimPivotToolCollection.CeChartDataVertical.CheckedChanged += CeChartDataVertical_CheckedChanged;
            AimPivotToolCollection.CeSelectionOnly.CheckedChanged += CeSelectionOnly_CheckedChanged;
            AimPivotToolCollection.CeShowColumnGrandTotals.CheckedChanged += CeShowColumnGrandTotals_CheckedChanged;
            AimPivotToolCollection.CeShowRowGrandTotals.CheckedChanged += CeShowRowGrandTotals_CheckedChanged;
            AimPivotToolCollection.CheckShowPointLabels.CheckedChanged += CheckShowPointLabels_CheckedChanged;
            AimPivotToolCollection.BarSelectSeries.Edit = CreateComBox();
            (AimPivotToolCollection.BarSelectSeries.Edit as RepositoryItemComboBox).TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
        }


        #endregion

        #region 对公工具方法
        /// <summary>
        /// 显示行合计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CeShowRowGrandTotals_CheckedChanged(object sender, ItemClickEventArgs e) {
            AimPivotGridControl.OptionsChartDataSource.ProvideRowGrandTotals
                = (sender as BarCheckItem).Checked;
        }

        /// <summary>
        /// 显示列合计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CeShowColumnGrandTotals_CheckedChanged(object sender, ItemClickEventArgs e) {
            AimPivotGridControl.OptionsChartDataSource.ProvideColumnGrandTotals
                = (sender as BarCheckItem).Checked;
        }

        /// <summary>
        /// 仅显示选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CeSelectionOnly_CheckedChanged(object sender, ItemClickEventArgs e) {
            AimPivotGridControl.OptionsChartDataSource.SelectionOnly
                = (sender as BarCheckItem).Checked;
        }

        /// <summary>
        /// 行列置换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CeChartDataVertical_CheckedChanged(object sender, ItemClickEventArgs e) {
            AimPivotGridControl.OptionsChartDataSource.ProvideDataByColumns
                = (sender as BarCheckItem).Checked;
        }

        /// <summary>
        /// 是否显示Label数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckShowPointLabels_CheckedChanged(object sender, ItemClickEventArgs e) {
            AimChartControl.SeriesTemplate.LabelsVisibility = (sender as BarCheckItem).Checked ?
                DevExpress.Utils.DefaultBoolean.True : DevExpress.Utils.DefaultBoolean.False;
            AimChartControl.CrosshairEnabled = (sender as BarCheckItem).Checked ?
                DevExpress.Utils.DefaultBoolean.False : DevExpress.Utils.DefaultBoolean.True;
        }

        /// <summary>
        /// 选择Series的表现形式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BarSelectSeries_EditValueChanged(object sender, EventArgs e) {
            AimChartControl.SeriesTemplate.ChangeView((ViewType)AimPivotToolCollection.BarSelectSeries.EditValue);
            if (AimChartControl.SeriesTemplate.Label != null) {
                AimChartControl.SeriesTemplate.LabelsVisibility = AimPivotToolCollection.CheckShowPointLabels.Checked ?
                    DevExpress.Utils.DefaultBoolean.True : DevExpress.Utils.DefaultBoolean.False;
                AimChartControl.CrosshairEnabled = AimPivotToolCollection.CheckShowPointLabels.Checked ?
                    DevExpress.Utils.DefaultBoolean.False : DevExpress.Utils.DefaultBoolean.True;
                AimPivotToolCollection.CheckShowPointLabels.Enabled = true;
            } else {
                AimPivotToolCollection.CheckShowPointLabels.Enabled = false;
            }
            if ((AimChartControl.SeriesTemplate.View as SimpleDiagramSeriesViewBase) == null)
                AimChartControl.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True;
            if (AimChartControl.Diagram is Diagram3D) {
                Diagram3D diagram = (Diagram3D)AimChartControl.Diagram;
                diagram.RuntimeRotation = true;
                diagram.RuntimeZooming = true;
                diagram.RuntimeScrolling = true;
            }
            foreach (Series series in AimChartControl.Series)
                UpdateSeriesTransparency(series.View);
            UpdateSeriesTransparency(AimChartControl.SeriesTemplate.View);
        }

        /// <summary>
        /// 设定数据刷新延迟时间(暂未使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void seUpdateDelay_EditValueChanged(object sender, EventArgs e) {
            AimPivotGridControl.OptionsChartDataSource.UpdateDelay
                = (sender as BarEditItem).EditValue.ToInt();
        }

        /// <summary>
        /// 更新表现形式
        /// </summary>
        /// <param name="seriesView"></param>
        public void UpdateSeriesTransparency(SeriesViewBase seriesView) {
            ISupportTransparency supportTransparency = seriesView as ISupportTransparency;
            if (supportTransparency != null) {
                if ((seriesView is AreaSeriesView) || (seriesView is Area3DSeriesView)
                    || (seriesView is RadarAreaSeriesView) || (seriesView is Bar3DSeriesView))
                    supportTransparency.Transparency = 135;
                else
                    supportTransparency.Transparency = 0;
            }
        }

        //private void exportChart_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
        //    saveFileDialog1.Title = "请选择存储路径...";
        //    saveFileDialog1.DefaultExt = ".Xlsx";
        //    saveFileDialog1.FileName = "Excel导出图表文件";
        //    saveFileDialog1.Filter = "Excel文件|*.xlsx;*.xls|所有文件|*.*";
        //    if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
        //        chartControl.ExportToXlsx(saveFileDialog1.FileName);
        //    }
        //}

        //private void exportPivot_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
        //    saveFileDialog1.Title = "请选择存储路径...";
        //    saveFileDialog1.DefaultExt = ".Xlsx";
        //    saveFileDialog1.FileName = "Excel导出数据文件";
        //    saveFileDialog1.Filter = "Excel文件|*.xlsx;*.xls|所有文件|*.*";
        //    if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
        //        pivotGridControl.ExportToXlsx(saveFileDialog1.FileName);
        //    }
        //}
        #endregion 对公工具方法

        #region 对私工具方法

        /// <summary>
        /// 创建一个Combox对象，内置到BarEditItem中
        /// </summary>
        /// <returns></returns>
        private RepositoryItemComboBox CreateComBox() {
            RepositoryItemComboBox repositoryItemComboBox1 = new RepositoryItemComboBox();
            // 
            // repositoryItemComboBox1
            // 
            repositoryItemComboBox1.AutoHeight = false;
            repositoryItemComboBox1.Name = "repositoryItemComboBox1";
            foreach (ViewType type in Enum.GetValues(typeof(ViewType))) {
                if (Array.IndexOf<ViewType>(ViewTypeArray, type) >= 0) continue;
                repositoryItemComboBox1.Items.Add(type);
            }
            return repositoryItemComboBox1;
        }
        #endregion
    }

    /// <summary>
    /// 前台定义的具有通用性的工具集
    /// </summary>
    public class PivotToolCollection {
        private BarCheckItem ceSelectionOnly;
        private BarCheckItem checkShowPointLabels;
        private BarCheckItem ceChartDataVertical;
        private BarCheckItem ceShowRowGrandTotals;
        private BarCheckItem ceShowColumnGrandTotals;
        private BarEditItem barSelectSeries;

        /// <summary>
        /// 仅展示选择
        /// </summary>
        public BarCheckItem CeSelectionOnly
        {
            get
            {
                return ceSelectionOnly;
            }

            set
            {
                ceSelectionOnly = value;
            }
        }

        /// <summary>
        /// 是否显示Labels
        /// </summary>
        public BarCheckItem CheckShowPointLabels
        {
            get
            {
                return checkShowPointLabels;
            }

            set
            {
                checkShowPointLabels = value;
            }
        }

        /// <summary>
        /// 行列置换
        /// </summary>
        public BarCheckItem CeChartDataVertical
        {
            get
            {
                return ceChartDataVertical;
            }

            set
            {
                ceChartDataVertical = value;
            }
        }

        /// <summary>
        /// 展示行合计
        /// </summary>
        public BarCheckItem CeShowRowGrandTotals
        {
            get
            {
                return ceShowRowGrandTotals;
            }

            set
            {
                ceShowRowGrandTotals = value;
            }
        }

        /// <summary>
        /// 展示列合计
        /// </summary>
        public BarCheckItem CeShowColumnGrandTotals
        {
            get
            {
                return ceShowColumnGrandTotals;
            }

            set
            {
                ceShowColumnGrandTotals = value;
            }
        }

        /// <summary>
        /// 图表类型选择
        /// </summary>
        public BarEditItem BarSelectSeries
        {
            get
            {
                return barSelectSeries;
            }

            set
            {
                barSelectSeries = value;
            }
        }
    }
}
