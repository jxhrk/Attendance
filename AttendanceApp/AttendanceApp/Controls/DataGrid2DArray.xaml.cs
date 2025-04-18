using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace AttendanceApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для DataGrid2DArray.xaml
    /// </summary>
    public partial class DataGrid2DArray : UserControl
    {
        private DataView data;
        private int Columns;
        private int Rows;

        private int MaxColumns;
        private int MaxRows;

        public DataGrid2DArray()
        {
            InitializeComponent();
        }

        public void Init<T>(int columns, int rows, string rowHeaderName, string[] resultColumnsNames)
        {
            MaxColumns = columns;
            MaxRows = rows;
            BuildDataView<T>(MaxColumns, MaxRows, rowHeaderName, resultColumnsNames);
        }

        private void BuildDataView<T>(int columns, int rows, string rowHeaderName, string[] resultColumnsNames)
        {
            var table = new DataTable();
            table.Columns.Add(rowHeaderName, typeof(string))
                             .ExtendedProperties.Add("idx", -1);
            for (var i = 0; i < columns; i++)
            {
                table.Columns.Add((i + 1).ToString("00"), typeof(T))
                             .ExtendedProperties.Add("idx", i); // Save original column index
            }

            foreach (string column in resultColumnsNames)
            {
                table.Columns.Add(column, typeof(string))
                             .ExtendedProperties.Add("idx", -1);
            }

            for (var i = 0; i < rows; i++)
            {
                table.Rows.Add(table.NewRow());
            }

            data = new DataView(table);
            StListGrid.ItemsSource = data;
        }

        public void Set2DArray<T>(T[,] array, string[] rowNames)
        {
            for (var ri = 0; ri < array.GetLength(0); ri++)
            {
                data[ri][0] = rowNames[ri];
                for (var ci = 0; ci < array.GetLength(1); ci++)
                {
                    data[ri][ci + 1] = array[ri, ci];
                }
            }
        }

        public void UpdateCellsVisibility(int columns)
        {
            int colUpdateFrom = Math.Min(Columns, columns);
            Columns = columns;

            for (int i = colUpdateFrom; i <= MaxColumns; i++)
            {
                StListGrid.Columns[i].Visibility = i > Columns ? Visibility.Collapsed : Visibility.Visible;
            }

        }

        public void UpdateRowsVisibility(int rows)
        {


            int rowUpdateFrom = Math.Min(Rows, rows);
            Rows = rows;

            DataView view = (StListGrid.ItemsSource as DataView);
            DataRowCollection coll = view.Table.Rows;
            int collCount = coll.Count;
            for (int i = rowUpdateFrom; i <= MaxRows; i++)
            {
                if (i < collCount)
                {
                    if (i + 1 > Rows)
                    {
                        coll.RemoveAt(coll.Count-1);
                    }
                }
                else
                {
                    if (i + 1 <= Rows)
                    {
                        coll.Add(view.Table.NewRow());
                    }
                }


                //StListGrid.ItemContainerGenerator.Items[i].Visibility = i > Columns ? Visibility.Collapsed : Visibility.Visible;
            }

        }

    }
}
