using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using FastWpfGrid;
using System.Data;
using System;

namespace FastWpfGridTest
{
    public class GridModel4 : DataTableGridModel
    {
        private const int rowCount = 1000;
        private const int columnCount = 1000;

        public GridModel4() : base(CreateDataTable())
        {
            for (int row = 5; row < 20; row++)
            {
                for (int col = 10; col < 100; col++)
                {
                    BackgroudColorsMap[$"{row}_{col}"] = Colors.Honeydew;
                }
            }

            RowHeaderColumnName = "Column_1";
            Reset();
        }

        private static DataTable CreateDataTable()
        {
            var dataTable = new DataTable();

            for (int i = 0; i < columnCount; i++)
            {
                dataTable.Columns.Add($"Column_{i}");
            }

            for (int i = 0; i < rowCount; i++)
            {
                dataTable.Rows.Add(Enumerable.Range(0, columnCount).Select(x => $"Row_{i}_{x}").ToArray());
            }

            return dataTable;
        }
    }

    public class DataTableGridModel : FastGridModelBase
    {
        private DataTable _dataTable;
        private bool _useDataAsRowHeader;
        private Dictionary<int, bool?> _isSortAscMap = new Dictionary<int, bool?>(); // null:not sort; true:asc; false:desc

        public Dictionary<string, Color> BackgroudColorsMap { get; set; } = new Dictionary<string, Color>();
        List<DataRow> _rows = new List<DataRow>();

        public string RowHeaderColumnName { get; set; }

        public DataTableGridModel(DataTable dataTable)
        {
            _dataTable = dataTable;
            _rows = _dataTable.AsEnumerable().ToList();
            Reset();
        }

        public void Reset()
        {
            _useDataAsRowHeader = !string.IsNullOrWhiteSpace(RowHeaderColumnName) && _dataTable.Columns.Contains(RowHeaderColumnName);
        }

        public override int ColumnCount
        {
            get { return _dataTable.Columns.Count; }
        }

        public override int RowCount
        {
            get { return _dataTable.Rows.Count; }
        }

        public override IFastGridCell GetColumnHeader(IFastGridView view, int column)
        {
            var sortMethod = _isSortAscMap.GetValueOrDefault(column);
            string image = !sortMethod.HasValue ? "/Images/flip_vertical_small.png" : sortMethod.Value ? "/Images/flip_horizontal_small.png" : "/Images/flip_transform_small.png";
            string sortCommand = !sortMethod.HasValue ? null : sortMethod.Value ? "Asc" : "Desc";

            var cell = new FastGridCellImpl();
            cell.AddTextBlock(_dataTable.Columns[column].ColumnName).IsBold = true;

            var btn = cell.AddImageBlock(image);
            btn.CommandParameter = "";
            btn.MouseHoverBehaviour = MouseHoverBehaviours.ShowAllWhenMouseOut;

            return cell;
        }

        public override void HandleCommand(IFastGridView view, FastGridCellAddress address, object commandParameter, ref bool handled)
        {
            if (address.IsColumnHeader && address.Column.HasValue)
            {
                var column = address.Column.Value;

                var currSortMethod = _isSortAscMap.GetValueOrDefault(column);
                _isSortAscMap.Clear();

                if (currSortMethod == true)
                {
                    _rows = _rows.OrderByDescending(x => x[column]).ToList();
                    _isSortAscMap[column] = false;
                }
                else
                {
                    _rows = _rows.OrderBy(x => x[column]).ToList();
                    _isSortAscMap[column] = true;
                }

                InvalidateAll();
                handled = true;
            }
        }

        public override string GetRowHeaderText(int row)
        {
            if (_useDataAsRowHeader)
                return _rows[row][RowHeaderColumnName].ToString();
            return base.GetRowHeaderText(row);
        }

        public override IFastGridCell GetCell(IFastGridView view, int row, int column)
        {
            var cell = new FastGridCellImpl();
            var text = cell.AddTextBlock(_rows[row][column]);
            cell.BackgroundColor = GetBackgroudColor(row, column);

            return cell;
        }

        private Color? GetBackgroudColor(int row, int column)
        {
            return BackgroudColorsMap.GetValueOrDefault(string.Format("{0}_{1}", row, column));
        }
    }
}
