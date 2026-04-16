using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace RogerTech.Common.Common
{
    public static class ListToDataTableHelper
    {
        /// <summary>
        /// 将泛型List<T>转换为DataTable
        /// </summary>
        /// <typeparam name="T">泛型实体类型</typeparam>
        /// <param name="list">待转换的泛型集合</param>
        /// <returns>转换后的DataTable</returns>
        public static DataTable ConvertListToDataTable<T>(List<T> list,out ArrayList arrayList)
        {
            arrayList = new ArrayList();
            // 1. 验证输入参数
            if (list == null)
                throw new ArgumentNullException(nameof(list), "待转换的List不能为空");
            if (list.Count == 0)
            {
                // 若List为空，仍返回结构完整的空DataTable（基于T的属性）
                return CreateEmptyDataTable<T>();
            }

            // 2. 创建DataTable并定义列结构（基于T的属性）
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in propertyInfos)
            {
                // 获取属性的数据类型（处理可空类型，如int? → int）
                Type columnType = prop.PropertyType;
                if (columnType.IsGenericType && columnType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    columnType = Nullable.GetUnderlyingType(columnType);
                }

                // 添加DataTable列（列名=属性名，列类型=属性数据类型）
                dataTable.Columns.Add(prop.Name, columnType ?? prop.PropertyType);
                arrayList.Add(prop.Name);
            }

            // 3. 遍历List，填充DataTable数据行
            foreach (T item in list)
            {
                DataRow dataRow = dataTable.NewRow();
                foreach (PropertyInfo prop in propertyInfos)
                {
                    // 获取实体对象的属性值，若为null则赋值DBNull（DataTable不支持null，需用DBNull替代）
                    object value = prop.GetValue(item, null) ?? DBNull.Value;
                    dataRow[prop.Name] = value;
                }
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        /// <summary>
        /// 创建基于T属性结构的空DataTable
        /// </summary>
        /// <typeparam name="T">泛型实体类型</typeparam>
        /// <returns>空DataTable（仅含列结构）</returns>
        private static DataTable CreateEmptyDataTable<T>()
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in propertyInfos)
            {
                Type columnType = prop.PropertyType;
                if (columnType.IsGenericType && columnType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    columnType = Nullable.GetUnderlyingType(columnType);
                }

                dataTable.Columns.Add(prop.Name, columnType ?? prop.PropertyType);
            }

            return dataTable;
        }
    }
}