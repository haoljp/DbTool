﻿using System;
using DbTool.Core.Entity;
using WeihanLi.Common;
using WeihanLi.Extensions;

namespace DbTool.Core
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// FCL类型转换为DbType
        /// </summary>
        /// <param name="type">Fcl 类型</param>
        /// <returns></returns>
        public static DbType FclType2DbType(Type type)
        {
            var typeFullName = type.Unwrap().FullName;

            switch (typeFullName)
            {
                case "System.Boolean":
                    return DbType.Bit;

                case "System.Byte":
                    return DbType.TinyInt;

                case "System.Int16":
                    return DbType.SmallInt;

                case "System.Int32":
                    return DbType.Int;

                case "System.Int64":
                    return DbType.BigInt;

                case "System.Single":
                    return DbType.Numeric;

                case "System.Double":
                    return DbType.Float;

                case "System.Decimal":
                    return DbType.Money;

                case "System.DateTime":
                    return DbType.DateTime;

                case "System.DateTimeOffset":
                    return DbType.DateTimeOffset;

                case "System.Guid":
                    return DbType.UniqueIdentifier;

                case "System.Object":
                    return DbType.Variant;

                default:
                    return DbType.NVarChar;
            }
        }

        /// <summary>
        /// 数据库数据类型转换为FCL类型
        /// </summary>
        /// <param name="dbType"> 数据库数据类型 </param>
        /// <param name="isNullable"> 该数据列是否可以为空 </param>
        /// <returns></returns>
        public static string SqlDbType2FclType(string dbType, bool isNullable = true)
        {
            var sqlDbType = (DbType)Enum.Parse(typeof(DbType), dbType, true);
            string type;
            switch (sqlDbType)
            {
                case DbType.Bit:
                    type = isNullable ? "bool?" : "bool";
                    break;

                case DbType.Float:
                case DbType.Real:
                    type = isNullable ? "double?" : "double";
                    break;

                case DbType.Binary:
                case DbType.VarBinary:
                case DbType.Image:
                case DbType.Timestamp:
                case DbType.RowVersion:
                    type = "byte[]";
                    break;

                case DbType.TinyInt:
                    type = isNullable ? "byte?" : "byte";
                    break;

                case DbType.SmallInt:
                case DbType.Int:
                    type = isNullable ? "int?" : "int";
                    break;

                case DbType.BigInt:
                    type = isNullable ? "long?" : "long";
                    break;

                case DbType.Char:
                case DbType.NChar:
                case DbType.NText:
                case DbType.NVarChar:
                case DbType.VarChar:
                case DbType.Text:
                case DbType.LongText:
                    type = "string";
                    break;

                case DbType.Numeric:
                case DbType.Money:
                case DbType.Decimal:
                case DbType.SmallMoney:
                    type = isNullable ? "decimal?" : "decimal";
                    break;

                case DbType.UniqueIdentifier:
                    type = isNullable ? "Guid?" : "Guid";
                    break;

                case DbType.Date:
                case DbType.SmallDateTime:
                case DbType.DateTime:
                case DbType.DateTime2:
                    type = isNullable ? "DateTime?" : "DateTime";
                    break;

                case DbType.Time:
                    type = isNullable ? "TimeSpan?" : "TimeSpan";
                    break;

                case DbType.DateTimeOffset:
                    type = isNullable ? "DateTimeOffset?" : "DateTimeOffset";
                    break;

                default:
                    type = "object";
                    break;
            }
            return type;
        }

        /// <summary>
        /// 获取数据库类型对应的默认长度
        /// </summary>
        /// <param name="dbType">数据类型</param>
        /// <param name="defaultLength">自定义默认长度</param>
        /// <returns></returns>
        public static uint GetDefaultSizeForDbType(string dbType, uint defaultLength = 64)
        {
            var sqlDbType = (DbType)Enum.Parse(typeof(DbType), dbType, true);
            var len = defaultLength;
            switch (sqlDbType)
            {
                case DbType.BigInt:
                    len = 8;
                    break;

                case DbType.Binary:
                    len = 8;
                    break;

                case DbType.Bit:
                    len = 1;
                    break;

                case DbType.Char:
                    len = 200;
                    break;

                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                    len = 8;
                    break;

                case DbType.Decimal:
                    len = 20;
                    break;

                case DbType.Float:
                    len = 20;
                    break;

                case DbType.Image:
                    break;

                case DbType.Int:
                    len = 4;
                    break;

                case DbType.Money:
                    len = 20;
                    break;

                case DbType.NChar:
                    len = 500;
                    break;

                case DbType.NText:
                    len = 200;
                    break;

                case DbType.Numeric:
                    len = 20;
                    break;

                case DbType.NVarChar:
                    len = 2000;
                    break;

                case DbType.Real:
                    len = 10;
                    break;

                case DbType.RowVersion:
                    break;

                case DbType.SmallDateTime:
                    len = 4;
                    break;

                case DbType.SmallInt:
                    len = 2;
                    break;

                case DbType.SmallMoney:
                    break;

                case DbType.Text:
                    len = 5000;
                    break;

                case DbType.Time:
                    len = 8;
                    break;

                case DbType.Timestamp:
                    len = 8;
                    break;

                case DbType.TinyInt:
                    len = 1;
                    break;

                case DbType.UniqueIdentifier:
                    len = 16;
                    break;

                    //case DbType.VarBinary:
                    //case DbType.VarChar:
                    //case DbType.Variant:
                    //case DbType.Xml:
                    //case DbType.Structured:
                    //default:
                    //  break;
            }
            return len;
        }

        /// <summary>
        /// 根据表信息生成sql语句
        /// </summary>
        /// <param name="tableEntity"> 表信息 </param>
        /// <param name="genDescription">生成描述信息</param>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static string GenerateSqlStatement(this TableEntity tableEntity, bool genDescription = true, string dbType = "SqlServer")
        {
            if (string.IsNullOrEmpty(tableEntity?.TableName))
            {
                return "";
            }
            return DependencyResolver.Current.ResolveService<DbProviderFactory>().GetDbProvider(dbType)?
                .GenerateSqlStatement(tableEntity, genDescription);
        }

        /// <summary>
        /// TrimTableName
        /// </summary>
        /// <returns></returns>
        public static string TrimTableName(this string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                return "";
            }
            tableName = tableName.Trim();
            if (tableName.Substring(0, 4).EqualsIgnoreCase("tab_")
                || tableName.Substring(0, 4).EqualsIgnoreCase("tbl_"))
            {
                return tableName.Substring(4);
            }
            if (tableName.Substring(0, 3).EqualsIgnoreCase("tab")
                || tableName.Substring(0, 3).EqualsIgnoreCase("tbl"))
            {
                return tableName.Substring(3);
            }
            return tableName;
        }

        /// <summary>
        /// TrimModelName
        /// </summary>
        /// <param name="modelName">modelName</param>
        /// <returns></returns>
        public static string TrimModelName(this string modelName)
        {
            if (string.IsNullOrEmpty(modelName))
            {
                return "";
            }
            modelName = modelName.Trim();
            if (modelName.EndsWith("Model"))
            {
                return modelName.Substring(0, modelName.Length - 5);
            }
            if (modelName.EndsWith("Entity"))
            {
                return modelName.Substring(0, modelName.Length - 6);
            }
            return modelName;
        }

        public static string GetDefaultFromDbDefaultValue(string dataType, object defaultValue)
        {
            if (null == defaultValue)
            {
                return null;
            }

            dataType = dataType.Trim().ToUpper();
            var str = defaultValue.ToString().ToUpper();
            if ((str.Contains("GETDATE") || str.Contains("NOW") || str.Contains("CURRENT_TIMESTAMP")) && (dataType.Contains("DATE") || dataType.Contains("TIME")))
            {
                return "DateTime.Now";
            }

            if (dataType.Equals("BIT"))
            {
                if (str.StartsWith("b'"))
                {
                    return str.Substring(2, 1).Equals("1") ? "true" : "false";
                }
                if (defaultValue is int)
                {
                    return defaultValue.To<int>() == 1 ? "true" : "false";
                }

                return defaultValue.ToString().ToLower();
            }

            return defaultValue.ToString();
        }

        /// <summary>
        /// 将属性名称转换为私有字段名称
        /// </summary>
        /// <param name="propertyName"> 属性名称 </param>
        /// <returns> 私有字段名称 </returns>
        public static string ToPrivateFieldName(this string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return "";
            }
            if (propertyName.Equals(propertyName.ToUpperInvariant()))
            {
                return propertyName.ToLowerInvariant();
            }
            if (char.IsUpper(propertyName[0]))//首字母大写，首字母转换为小写
            {
                return char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1);
            }
            else
            {
                return "_" + propertyName;
            }
        }
    }
}
