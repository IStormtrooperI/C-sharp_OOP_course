using System;
using System.Collections.Generic;

namespace Generics.Tables
{
    public class Table<TIntRows, TStringColumns, TDoubleContent>
    {
        public Dictionary<int, TIntRows> rows;

        public Dictionary<int, TStringColumns> columns;

        public TDoubleContent[,] content = new TDoubleContent[5000, 5000];

        public class CountToRowsAndColumns
        {
            public int count;
            public int Count()
            {
                return count;
            }
        }

        private readonly CountToRowsAndColumns countRows = new CountToRowsAndColumns();
        public CountToRowsAndColumns Rows
        {
            get
            {
                return countRows;
            }
        }

        private readonly CountToRowsAndColumns countColumns = new CountToRowsAndColumns();
        public CountToRowsAndColumns Columns
        {
            get
            {
                return countColumns;
            }
        }

        public bool AddRow(TIntRows row)
        {
            if (rows == null) 
            {
                rows = new Dictionary<int, TIntRows> { { 0, row } };
                ++countRows.count;
                return true;
            }
            else if (!rows.ContainsValue(row))
            {
                rows.Add(rows.Count, row);
                ++countRows.count;
                return true;
            }
            return false;
        }

        public bool AddColumn(TStringColumns column)
        {
            if (columns == null)
            {
                columns = new Dictionary<int, TStringColumns> { { 0, column } };
                ++countColumns.count;
                return true;
            }
            else if (!columns.ContainsValue(column))
            {
                columns.Add(columns.Count, column);
                ++countColumns.count;
                return true;
            }
            return false;
        }

        public int GetRowIndex(TIntRows row) 
        {
            if (rows != null) 
            {
                for (int i = 0; i < rows.Count; i++)
                {
                    rows.TryGetValue(i, out TIntRows value);
                    if (row.Equals(value))
                    {
                        return i;
                    }
                }
            }
            return -1; 
        }

        public int GetColumnIndex(TStringColumns column)
        {
            if (columns != null) 
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    columns.TryGetValue(i, out TStringColumns value);
                    if (column.Equals(value))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public void AddDefaultContent(TIntRows row, TStringColumns column)
        {
            if(AddRow(row))
                --countRows.count;

            if(AddColumn(column))
                --countColumns.count;


            var indexRow = GetRowIndex(row);
            var indexColumn = GetColumnIndex(column);

            if (content[indexRow, indexColumn] == null)
            {
                content[indexRow, indexColumn] = default;
            }
        }

        public Indexator<TIntRows, TStringColumns, TDoubleContent> Open
        {
            get
            {
                return new Indexator<TIntRows, TStringColumns, TDoubleContent>(this, true);
            }
        }

        public Indexator<TIntRows, TStringColumns, TDoubleContent> Existed
        {
            get
            {
                return new Indexator<TIntRows, TStringColumns, TDoubleContent>(this, false);
            }
        }
    }

    public class Indexator<TIntRows, TStringColumns, TDoubleContent>
    {
        public bool IsAccess { get; set; }
        public Table<TIntRows, TStringColumns, TDoubleContent> Table { get; set; }

        public Indexator(Table<TIntRows, TStringColumns, TDoubleContent> table, bool isAccess)
        {
            Table = table;
            IsAccess = isAccess;
        }

        public TDoubleContent this [TIntRows row, TStringColumns column]
        {
            get
            {
                if(IsAccess)
                    Table.AddDefaultContent(row, column);
                try
                {
                    var indexRow = Table.GetRowIndex(row);
                    var indexColumn = Table.GetColumnIndex(column);

                    return Table.content[indexRow, indexColumn];
                }
                catch (Exception)
                {
                    throw new ArgumentException();
                }
            }
            set
            {
                if (IsAccess)
                {
                    Table.AddRow(row);
                    Table.AddColumn(column);
                }
                try
                {
                    var indexRow = Table.GetRowIndex(row);
                    var indexColumn = Table.GetColumnIndex(column);

                    Table.content[indexRow, indexColumn] = value;
                }
                catch (Exception)
                {
                    throw new ArgumentException();
                }
            }
        }
    }
}
