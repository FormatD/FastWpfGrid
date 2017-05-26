using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastWpfGridTest
{
    public static class DataTableExtension
    {
        public static DataTable OrderBy<T>(this DataTable datatable, Func<DataRow, T> func)
        {
            var dt = datatable.Clone();
            foreach (var row in datatable.AsEnumerable().OrderBy(func))
            {
                dt.ImportRow(row);
            }

            return dt;
        }

        public static IEnumerable<DataRow> OrderBy<T>(this IEnumerable<DataRow> rows, Func<DataRow, T> func)
        {
            return Enumerable.OrderBy(rows, func);
        }

        public static DataTable OrderByDescending<T>(this DataTable datatable, Func<DataRow, T> func)
        {
            var dt = datatable.Clone();
            foreach (var row in datatable.AsEnumerable().OrderByDescending(func))
            {
                dt.ImportRow(row);
            }

            return dt;
        }

        public static IEnumerable<DataRow> OrderByDescending<T>(this IEnumerable<DataRow> rows, Func<DataRow, T> func)
        {
            return Enumerable.OrderByDescending(rows, func);
        }
    }
}
