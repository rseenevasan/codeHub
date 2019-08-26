using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Time.Core.Extensions
{
    public static class TableQueryExtensions
    {
        const string RowKey = "RowKey";
        public static string RowKeyStartsWith(string searchStr)
        {
            if (string.IsNullOrEmpty(searchStr)) return null;

            char lastChar = searchStr[searchStr.Length - 1];
            char nextLastChar = (char)((int)lastChar + 1);
            string nextSearchStr = searchStr.Substring(0, searchStr.Length - 1) + nextLastChar;

            return TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition(RowKey, QueryComparisons.GreaterThanOrEqual, searchStr),
                TableOperators.And,
                TableQuery.GenerateFilterCondition(RowKey, QueryComparisons.LessThan, nextSearchStr)
                );

        }

    }
}
