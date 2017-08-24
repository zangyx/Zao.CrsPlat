using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Zao.CrsPlat.DevExpressHelper.ZaoUtil
{
    /// <summary>
    /// Excel操作辅助类（无需VBA引用）
    /// </summary>
    public class ZaoExcel {
        /// <summary>
        /// Excel 版本
        /// </summary>
        public enum ExcelType {
            Excel2003,
            Excel2007
        }

        /// <summary>
        /// IMEX 三种模式。
        /// IMEX是用来告诉驱动程序使用Excel文件的模式，其值有0、1、2三种，分别代表导出、导入、混合模式。
        /// </summary>
        public enum IMEXType {
            ExportMode = 0,
            ImportMode = 1,
            LinkedMode = 2
        }



        /// <summary>
        /// 返回Excel 连接字符串   [IMEX=1]
        /// </summary>
        /// <param name="excelPath">Excel文件 绝对路径</param>
        /// <param name="header">是否把第一行作为列名</param>
        /// <param name="eType">Excel 版本 </param>
        /// <returns></returns>
        public static string GetExcelConnectstring(string excelPath, bool header, ExcelType eType) {
            return GetExcelConnectstring(excelPath, header, eType, IMEXType.ImportMode);
        }

        /// <summary>
        /// 返回Excel 连接字符串
        /// </summary>
        /// <param name="excelPath">Excel文件 绝对路径</param>
        /// <param name="header">是否把第一行作为列名</param>
        /// <param name="eType">Excel 版本 </param>
        /// <param name="imex">IMEX模式</param>
        /// <returns></returns>
        public static string GetExcelConnectstring(string excelPath, bool header, ExcelType eType, IMEXType imex) {
            if (!ZaoFile.IsExistFile(excelPath)) {
                throw new FileNotFoundException("Excel路径不存在!");
            }
            var connectstring = string.Empty;

            var hdr = "NO";
            if (header) {
                hdr = "YES";
            }
            if (eType == ExcelType.Excel2003) {
                connectstring = "Provider=Microsoft.Jet.OleDb.4.0; data source=" + excelPath + ";Extended Properties='Excel 8.0; HDR=" + hdr + "; IMEX=" + imex.GetHashCode() + "'";
            }            else {
                connectstring = "Provider=Microsoft.ACE.OLEDB.12.0; data source=" + excelPath + ";Extended Properties='Excel 12.0 Xml; HDR=" + hdr + "; IMEX=" + imex.GetHashCode() + "'";
            }
            return connectstring;
        }





        /// <summary>
        /// 返回Excel工作表名
        /// </summary>
        /// <param name="excelPath">Excel文件 绝对路径</param>
        /// <param name="eType">Excel 版本 </param>
        /// <returns></returns>
        public static List<string> GetExcelTablesName(string excelPath, ExcelType eType) {
            var connectstring = GetExcelConnectstring(excelPath, true, eType);
            return GetExcelTablesName(connectstring);
        }

        /// <summary>
        /// 返回Excel工作表名
        /// </summary>
        /// <param name="connectstring">excel连接字符串</param>
        /// <returns></returns>
        public static List<string> GetExcelTablesName(string connectstring) {
            using (var conn = new OleDbConnection(connectstring)) {
                return GetExcelTablesName(conn);
            }
        }

        /// <summary>
        /// 返回Excel工作表名
        /// </summary>
        /// <param name="connection">excel连接</param>
        /// <returns></returns>
        public static List<string> GetExcelTablesName(OleDbConnection connection) {
            var list = new List<string>();

            if (connection.State == ConnectionState.Closed) {
                connection.Open();
            }
            var dt = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if (dt != null && dt.Rows.Count > 0) {
                for (var i = 0; i < dt.Rows.Count; i++) {
                    list.Add(ZaoDataType.ConvertTo<string>(dt.Rows[i][2]));
                }
            }

            return list;
        }

        /// <summary>
        /// 返回Excel第一个工作表表名
        /// </summary>
        /// <param name="excelPath">Excel文件 绝对路径</param>
        /// <param name="eType">Excel 版本 </param>
        /// <returns></returns>
        public static string GetExcelFirstTableName(string excelPath, ExcelType eType) {
            var connectstring = GetExcelConnectstring(excelPath, true, eType);
            return GetExcelFirstTableName(connectstring);
        }

        /// <summary>
        /// 返回Excel第一个工作表表名
        /// </summary>
        /// <param name="connectstring">excel连接字符串</param>
        /// <returns></returns>
        public static string GetExcelFirstTableName(string connectstring) {
            using (var conn = new OleDbConnection(connectstring)) {
                return GetExcelFirstTableName(conn);
            }
        }

        /// <summary>
        /// 返回Excel第一个工作表表名
        /// </summary>
        /// <param name="connection">excel连接</param>
        /// <returns></returns>
        public static string GetExcelFirstTableName(OleDbConnection connection) {
            var tableName = string.Empty;

            if (connection.State == ConnectionState.Closed) {
                connection.Open();
            }
            var dt = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if (dt != null && dt.Rows.Count > 0) {
                tableName = ZaoDataType.ConvertTo<string>(dt.Rows[0][2]);
            }

            return tableName;
        }

        /// <summary>
        /// 获取Excel文件中指定工作表的列
        /// </summary>
        /// <param name="excelPath">Excel文件 绝对路径</param>
        /// <param name="table">名称 excel table  例如：Sheet1$</param>
        /// <returns></returns>
        public static List<string> GetColumnsList(string excelPath, ExcelType eType, string table) {
            var list = new List<string>();
            DataTable tableColumns = null;
            var connectstring = GetExcelConnectstring(excelPath, true, eType);
            using (var conn = new OleDbConnection(connectstring)) {
                conn.Open();
                tableColumns = GetReaderSchema(table, conn);
            }
            foreach (DataRow dr in tableColumns.Rows) {
                var columnName = dr["ColumnName"].ToString();
                var datatype = ((OleDbType)dr["ProviderType"]).ToString();
                var netType = dr["DataType"].ToString();
                list.Add(columnName);
            }

            return list;
        }

        private static DataTable GetReaderSchema(string tableName, OleDbConnection connection) {
            DataTable schemaTable = null;
            IDbCommand cmd = new OleDbCommand();
            cmd.CommandText = string.Format("select * from [{0}]", tableName);
            cmd.Connection = connection;

            using (var reader = cmd.ExecuteReader(CommandBehavior.KeyInfo | CommandBehavior.SchemaOnly)) {
                schemaTable = reader.GetSchemaTable();
            }
            return schemaTable;
        }

        /// <summary>
        /// EXCEL导入DataSet
        /// </summary>
        /// <param name="excelPath">Excel文件 绝对路径</param>
        /// <param name="table">名称 excel table  例如：Sheet1$ </param>
        /// <param name="header">是否把第一行作为列名</param>
        /// <param name="eType">Excel 版本 </param>
        /// <returns>返回Excel相应工作表中的数据 DataSet   [table不存在时返回空的DataSet]</returns>
        public static DataSet ExcelToDataSet(string excelPath, string table, bool header, ExcelType eType) {
            var connectstring = GetExcelConnectstring(excelPath, header, eType);
            return ExcelToDataSet(connectstring, table);
        }

        /*2017-4-7*zangyx*增加可设置imextype参数的方法*/
        /// <summary>
        /// EXCEL导入DataSet
        /// </summary>
        /// <param name="excelPath">Excel文件 绝对路径</param>
        /// <param name="table">名称 excel table  例如：Sheet1$ </param>
        /// <param name="header">是否把第一行作为列名</param>
        /// <param name="eType">Excel 版本 </param>
        /// <returns>返回Excel相应工作表中的数据 DataSet   [table不存在时返回空的DataSet]</returns>
        public static DataSet ExcelToDataSet(string excelPath, string table, bool header, ExcelType eType, IMEXType imex) {
            var connectstring = GetExcelConnectstring(excelPath, header, eType, imex);
            return ExcelToDataSet(connectstring, table);
        }

        /// <summary>
        /// 判断工作表名是否存在
        /// </summary>
        /// <param name="connection">excel连接</param>
        /// <param name="table">名称 excel table  例如：Sheet1$</param>
        /// <returns></returns>
        private static bool isExistExcelTableName(OleDbConnection connection, string table) {
            var list = GetExcelTablesName(connection);
            foreach (string tName in list) {
                if (tName.ToLower() == table.ToLower()) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// EXCEL导入DataSet
        /// </summary>
        /// <param name="connectstring">excel连接字符串</param>
        /// <param name="table">名称 excel table  例如：Sheet1$ </param>
        /// <returns>返回Excel相应工作表中的数据 DataSet   [table不存在时返回空的DataSet]</returns>
        public static DataSet ExcelToDataSet(string connectstring, string table) {
            using (var conn = new OleDbConnection(connectstring)) {
                var ds = new DataSet();


                if (isExistExcelTableName(conn, table)) {
                    var adapter = new OleDbDataAdapter("SELECT * FROM [" + table + "]", conn);
                    adapter.Fill(ds, table);
                }

                return ds;
            }
        }

        /// <summary>
        /// EXCEL所有工作表导入DataSet
        /// </summary>
        /// <param name="excelPath">Excel文件 绝对路径</param>
        /// <param name="header">是否把第一行作为列名</param>
        /// <param name="eType">Excel 版本 </param>
        /// <returns>返回Excel第一个工作表中的数据 DataSet </returns>
        public static DataSet ExcelToDataSet(string excelPath, bool header, ExcelType eType) {
            var connectstring = GetExcelConnectstring(excelPath, header, eType);
            return ExcelToDataSet(connectstring);
        }

        /*2017-4-7*zangyx*增加可设置imextype参数的方法*/
        /// <summary>
        /// EXCEL所有工作表导入DataSet
        /// </summary>
        /// <param name="excelPath">Excel文件 绝对路径</param>
        /// <param name="header">是否把第一行作为列名</param>
        /// <param name="eType">Excel 版本 </param>
        /// <returns>返回Excel第一个工作表中的数据 DataSet </returns>
        public static DataSet ExcelToDataSet(string excelPath, bool header, ExcelType eType, IMEXType imex) {
            var connectstring = GetExcelConnectstring(excelPath, header, eType,imex);
            return ExcelToDataSet(connectstring);
        }

        /// <summary>
        /// EXCEL所有工作表导入DataSet
        /// </summary>
        /// <param name="connectstring">excel连接字符串</param>
        /// <returns>返回Excel第一个工作表中的数据 DataSet </returns>
        public static DataSet ExcelToDataSet(string connectstring) {
            using (var conn = new OleDbConnection(connectstring)) {
                var ds = new DataSet();
                var tableNames = GetExcelTablesName(conn);

                foreach (string tableName in tableNames) {
                    var adapter = new OleDbDataAdapter("SELECT * FROM [" + tableName + "]", conn);
                    adapter.Fill(ds, tableName);
                }
                return ds;
            }
        }



        /// <summary>
        /// 把一个数据集中的数据导出到Excel文件中(XML格式操作)
        /// </summary>
        /// <param name="source">DataSet数据</param>
        /// <param name="fileName">保存的Excel文件名</param>
        public static void DataSetToExcel(DataSet source, string fileName) {
            var excelDoc = new StreamWriter(fileName);
            const string startExcelXML = "<xml version>\r\n<Workbook " +
                  "xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n" +
                  " xmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\n " +
                  "xmlns:x=\"urn:schemas-    microsoft-com:office:" +
                  "excel\"\r\n xmlns:ss=\"urn:schemas-microsoft-com:" +
                  "office:spreadsheet\">\r\n <Styles>\r\n " +
                  "<Style ss:ID=\"Default\" ss:Name=\"Normal\">\r\n " +
                  "<Alignment ss:Vertical=\"Bottom\"/>\r\n <Borders/>" +
                  "\r\n <Font/>\r\n <Interior/>\r\n <NumberFormat/>" +
                  "\r\n <Protection/>\r\n </Style>\r\n " +
                  "<Style ss:ID=\"BoldColumn\">\r\n <Font " +
                  "x:Family=\"Swiss\" ss:Bold=\"1\"/>\r\n </Style>\r\n " +
                  "<Style     ss:ID=\"StringLiteral\">\r\n <NumberFormat" +
                  " ss:Format=\"@\"/>\r\n </Style>\r\n <Style " +
                  "ss:ID=\"Decimal\">\r\n <NumberFormat " +
                  "ss:Format=\"#,##0.###\"/>\r\n </Style>\r\n " +
                  "<Style ss:ID=\"Integer\">\r\n <NumberFormat " +
                  "ss:Format=\"0\"/>\r\n </Style>\r\n <Style " +
                  "ss:ID=\"DateLiteral\">\r\n <NumberFormat " +
                  "ss:Format=\"yyyy-mm-dd;@\"/>\r\n </Style>\r\n " +
                  "</Styles>\r\n ";
            const string endExcelXML = "</Workbook>";


            var sheetCount = 1;
            excelDoc.Write(startExcelXML);
            for (var i = 0; i < source.Tables.Count; i++) {
                var rowCount = 0;
                var dt = source.Tables[i];

                excelDoc.Write("<Worksheet ss:Name=\"Sheet" + sheetCount + "\">");
                excelDoc.Write("<Table>");
                excelDoc.Write("<Row>");
                for (var x = 0; x < dt.Columns.Count; x++) {
                    excelDoc.Write("<Cell ss:StyleID=\"BoldColumn\"><Data ss:Type=\"String\">");
                    excelDoc.Write(source.Tables[0].Columns[x].ColumnName);
                    excelDoc.Write("</Data></Cell>");
                }
                excelDoc.Write("</Row>");
                foreach (DataRow x in dt.Rows) {
                    rowCount++;


                    if (rowCount == 64000) {
                        rowCount = 0;
                        sheetCount++;
                        excelDoc.Write("</Table>");
                        excelDoc.Write(" </Worksheet>");
                        excelDoc.Write("<Worksheet ss:Name=\"Sheet" + sheetCount + "\">");
                        excelDoc.Write("<Table>");
                    }
                    excelDoc.Write("<Row>");

                    for (var y = 0; y < source.Tables[0].Columns.Count; y++) {
                        var rowType = x[y].GetType();

                        switch (rowType.ToString()) {
                            case "System.String":
                                var XMLstring = x[y].ToString();
                                XMLstring = XMLstring.Trim();
                                XMLstring = XMLstring.Replace("&", "&");
                                XMLstring = XMLstring.Replace(">", ">");
                                XMLstring = XMLstring.Replace("<", "<");
                                excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                               "<Data ss:Type=\"String\">");
                                excelDoc.Write(XMLstring);
                                excelDoc.Write("</Data></Cell>");
                                break;
                            case "System.DateTime":





                                var XMLDate = (DateTime)x[y];

                                var XMLDatetoString = XMLDate.Year +
                                                         "-" +
                                (XMLDate.Month < 10
                                ? "0" +
                                                                XMLDate.Month
                                : XMLDate.Month.ToString()) +
                                                         "-" +
                                (XMLDate.Day < 10
                                ? "0" +
                                                                XMLDate.Day
                                : XMLDate.Day.ToString()) +
                                                         "T" +
                                (XMLDate.Hour < 10
                                ? "0" +
                                                                XMLDate.Hour
                                : XMLDate.Hour.ToString()) +
                                                         ":" +
                                (XMLDate.Minute < 10
                                ? "0" +
                                                                XMLDate.Minute
                                : XMLDate.Minute.ToString()) +
                                                         ":" +
                                (XMLDate.Second < 10
                                ? "0" +
                                                                XMLDate.Second
                                : XMLDate.Second.ToString()) +
                                                         ".000";
                                excelDoc.Write("<Cell ss:StyleID=\"DateLiteral\">" +
                                             "<Data ss:Type=\"DateTime\">");
                                excelDoc.Write(XMLDatetoString);
                                excelDoc.Write("</Data></Cell>");
                                break;
                            case "System.Boolean":
                                excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                            "<Data ss:Type=\"String\">");
                                excelDoc.Write(x[y].ToString());
                                excelDoc.Write("</Data></Cell>");
                                break;
                            case "System.Int16":
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Byte":
                                excelDoc.Write("<Cell ss:StyleID=\"Integer\">" +
                                        "<Data ss:Type=\"Number\">");
                                excelDoc.Write(x[y].ToString());
                                excelDoc.Write("</Data></Cell>");
                                break;
                            case "System.Decimal":
                            case "System.Double":
                                excelDoc.Write("<Cell ss:StyleID=\"Decimal\">" +
                                      "<Data ss:Type=\"Number\">");
                                excelDoc.Write(x[y].ToString());
                                excelDoc.Write("</Data></Cell>");
                                break;
                            case "System.DBNull":
                                excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                      "<Data ss:Type=\"String\">");
                                excelDoc.Write("");
                                excelDoc.Write("</Data></Cell>");
                                break;
                            default:
                                throw (new Exception(rowType.ToString() + " not handled."));
                        }
                    }
                    excelDoc.Write("</Row>");
                }
                excelDoc.Write("</Table>");
                excelDoc.Write(" </Worksheet>");

                sheetCount++;
            }

            excelDoc.Write(endExcelXML);
            excelDoc.Close();
        }

        /// <summary>
        /// 将DataTable导出为Excel(OleDb 方式操作）
        /// </summary>
        /// <param name="dataTable">表</param>
        /// <param name="fileName">导出默认文件名</param>
        public static void DataSetToExcel(DataTable dataTable, string fileName) {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "xls files (*.xls)|*.xls";
            saveFileDialog.FileName = fileName;
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                fileName = saveFileDialog.FileName;
                if (File.Exists(fileName)) {
                    try {
                        File.Delete(fileName);
                    } catch {
                        MessageBox.Show("该文件正在使用中,关闭文件或重新命名导出文件再试!");
                        return;
                    }
                }
                var oleDbConn = new OleDbConnection();
                var oleDbCmd = new OleDbCommand();
                var sSql = string.Empty;
                try {
                    oleDbConn.ConnectionString = "Provider=Microsoft.Jet.OleDb.4.0;Data Source=" + fileName + @";Extended ProPerties=""Excel 8.0;HDR=Yes;""";
                    oleDbConn.Open();
                    oleDbCmd.CommandType = CommandType.Text;
                    oleDbCmd.Connection = oleDbConn;
                    sSql = "CREATE TABLE sheet1 (";
                    for (var i = 0; i < dataTable.Columns.Count; i++) {
                        if (i < dataTable.Columns.Count - 1) {
                            sSql += "[" + dataTable.Columns[i].Caption + "] TEXT(100) ,";
                        }                        else {
                            sSql += "[" + dataTable.Columns[i].Caption + "] TEXT(200) )";
                        }
                    }
                    oleDbCmd.CommandText = sSql;
                    oleDbCmd.ExecuteNonQuery();
                    for (var j = 0; j < dataTable.Rows.Count; j++) {
                        sSql = "INSERT INTO sheet1 VALUES('";
                        for (var i = 0; i < dataTable.Columns.Count; i++) {
                            if (i < dataTable.Columns.Count - 1) {
                                sSql += dataTable.Rows[j][i].ToString() + " ','";
                            }                            else {
                                sSql += dataTable.Rows[j][i].ToString() + " ')";
                            }
                        }
                        oleDbCmd.CommandText = sSql;
                        oleDbCmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("导出EXCEL成功");
                } catch (Exception ex) {
                    MessageBox.Show("导出EXCEL失败:" + ex.Message);
                }
                finally {
                    oleDbCmd.Dispose();
                    oleDbConn.Close();
                    oleDbConn.Dispose();
                }
            }
        }

        /// <summary>
        /// 将数据导出至Excel文件
        /// </summary>
        /// <param name="Table">DataTable对象</param>
        /// <param name="ExcelFilePath">Excel文件路径</param>
        public static bool OutputToExcel(DataTable Table, string ExcelFilePath) {
            if (File.Exists(ExcelFilePath)) {
                throw new Exception("该文件已经存在！");
            }

            if ((Table.TableName.Trim().Length == 0) || (Table.TableName.ToLower() == "table")) {
                Table.TableName = "Sheet1";
            }


            var ColCount = Table.Columns.Count;


            var i = 0;


            var para = new OleDbParameter[ColCount];


            var TableStructStr = @"Create Table " + Table.TableName + "(";


            var connString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + ExcelFilePath + ";Extended Properties=Excel 8.0;";
            var objConn = new OleDbConnection(connString);


            var objCmd = new OleDbCommand();


            var DataTypeList = new ArrayList();
            DataTypeList.Add("System.Decimal");
            DataTypeList.Add("System.Double");
            DataTypeList.Add("System.Int16");
            DataTypeList.Add("System.Int32");
            DataTypeList.Add("System.Int64");
            DataTypeList.Add("System.Single");


            foreach (DataColumn col in Table.Columns) {
                if (DataTypeList.IndexOf(col.DataType.ToString()) >= 0) {
                    para[i] = new OleDbParameter("@" + col.ColumnName, OleDbType.Double);
                    objCmd.Parameters.Add(para[i]);


                    if (i + 1 == ColCount) {
                        TableStructStr += col.ColumnName + " double)";
                    } else {
                        TableStructStr += col.ColumnName + " double,";
                    }
                } else {
                    para[i] = new OleDbParameter("@" + col.ColumnName, OleDbType.VarChar);
                    objCmd.Parameters.Add(para[i]);


                    if (i + 1 == ColCount) {
                        TableStructStr += col.ColumnName + " varchar)";
                    } else {
                        TableStructStr += col.ColumnName + " varchar,";
                    }
                }
                i++;
            }


            try {
                objCmd.Connection = objConn;
                objCmd.CommandText = TableStructStr;

                if (objConn.State == ConnectionState.Closed) {
                    objConn.Open();
                }
                objCmd.ExecuteNonQuery();
            } catch (Exception exp) {
                throw exp;
            }


            var InsertSql_1 = "Insert into " + Table.TableName + " (";
            var InsertSql_2 = " Values (";
            var InsertSql = string.Empty;


            for (var colID = 0; colID < ColCount; colID++) {
                if (colID + 1 == ColCount) {
                    InsertSql_1 += Table.Columns[colID].ColumnName + ")";
                    InsertSql_2 += "@" + Table.Columns[colID].ColumnName + ")";
                } else {
                    InsertSql_1 += Table.Columns[colID].ColumnName + ",";
                    InsertSql_2 += "@" + Table.Columns[colID].ColumnName + ",";
                }
            }

            InsertSql = InsertSql_1 + InsertSql_2;


            for (var rowID = 0; rowID < Table.Rows.Count; rowID++) {
                for (var colID = 0; colID < ColCount; colID++) {
                    if (para[colID].DbType == DbType.Double && Table.Rows[rowID][colID].ToString().Trim() == string.Empty) {
                        para[colID].Value = 0;
                    } else {
                        para[colID].Value = Table.Rows[rowID][colID].ToString().Trim();
                    }
                }
                try {
                    objCmd.CommandText = InsertSql;
                    objCmd.ExecuteNonQuery();
                } catch (Exception) {
                }
            }
            try {
                if (objConn.State == ConnectionState.Open) {
                    objConn.Close();
                }
            } catch (Exception exp) {
                throw exp;
            }
            return true;
        }

        /// <summary>
        /// 将数据导出至Excel文件
        /// </summary>
        /// <param name="Table">DataTable对象</param>
        /// <param name="Columns">要导出的数据列集合</param>
        /// <param name="ExcelFilePath">Excel文件路径</param>
        public static bool OutputToExcel(DataTable Table, ArrayList Columns, string ExcelFilePath) {
            if (File.Exists(ExcelFilePath)) {
                throw new Exception("该文件已经存在！");
            }


            if (Columns.Count > Table.Columns.Count) {
                for (var s = Table.Columns.Count + 1; s <= Columns.Count; s++) {
                    Columns.RemoveAt(s);
                }
            }


            var column = new DataColumn();
            for (var j = 0; j < Columns.Count; j++) {
                try {
                    column = (DataColumn)Columns[j];
                } catch (Exception) {
                    Columns.RemoveAt(j);
                }
            }
            if ((Table.TableName.Trim().Length == 0) || (Table.TableName.ToLower() == "table")) {
                Table.TableName = "Sheet1";
            }


            var ColCount = Columns.Count;


            var para = new OleDbParameter[ColCount];


            var TableStructStr = @"Create Table " + Table.TableName + "(";


            var connString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + ExcelFilePath + ";Extended Properties=Excel 8.0;";
            var objConn = new OleDbConnection(connString);


            var objCmd = new OleDbCommand();


            var DataTypeList = new ArrayList();
            DataTypeList.Add("System.Decimal");
            DataTypeList.Add("System.Double");
            DataTypeList.Add("System.Int16");
            DataTypeList.Add("System.Int32");
            DataTypeList.Add("System.Int64");
            DataTypeList.Add("System.Single");

            var col = new DataColumn();


            for (var k = 0; k < ColCount; k++) {
                col = (DataColumn)Columns[k];


                if (DataTypeList.IndexOf(col.DataType.ToString().Trim()) >= 0) {
                    para[k] = new OleDbParameter("@" + col.Caption.Trim(), OleDbType.Double);
                    objCmd.Parameters.Add(para[k]);


                    if (k + 1 == ColCount) {
                        TableStructStr += col.Caption.Trim() + " Double)";
                    } else {
                        TableStructStr += col.Caption.Trim() + " Double,";
                    }
                } else {
                    para[k] = new OleDbParameter("@" + col.Caption.Trim(), OleDbType.VarChar);
                    objCmd.Parameters.Add(para[k]);


                    if (k + 1 == ColCount) {
                        TableStructStr += col.Caption.Trim() + " VarChar)";
                    } else {
                        TableStructStr += col.Caption.Trim() + " VarChar,";
                    }
                }
            }


            try {
                objCmd.Connection = objConn;
                objCmd.CommandText = TableStructStr;

                if (objConn.State == ConnectionState.Closed) {
                    objConn.Open();
                }
                objCmd.ExecuteNonQuery();
            } catch (Exception exp) {
                throw exp;
            }


            var InsertSql_1 = "Insert into " + Table.TableName + " (";
            var InsertSql_2 = " Values (";
            var InsertSql = string.Empty;


            for (var colID = 0; colID < ColCount; colID++) {
                if (colID + 1 == ColCount) {
                    InsertSql_1 += Columns[colID].ToString().Trim() + ")";
                    InsertSql_2 += "@" + Columns[colID].ToString().Trim() + ")";
                } else {
                    InsertSql_1 += Columns[colID].ToString().Trim() + ",";
                    InsertSql_2 += "@" + Columns[colID].ToString().Trim() + ",";
                }
            }

            InsertSql = InsertSql_1 + InsertSql_2;


            var DataCol = new DataColumn();
            for (var rowID = 0; rowID < Table.Rows.Count; rowID++) {
                for (var colID = 0; colID < ColCount; colID++) {
                    DataCol = (DataColumn)Columns[colID];
                    if (para[colID].DbType == DbType.Double && Table.Rows[rowID][DataCol.Caption].ToString().Trim() == string.Empty) {
                        para[colID].Value = 0;
                    } else {
                        para[colID].Value = Table.Rows[rowID][DataCol.Caption].ToString().Trim();
                    }
                }
                try {
                    objCmd.CommandText = InsertSql;
                    objCmd.ExecuteNonQuery();
                } catch (Exception) {
                }
            }
            try {
                if (objConn.State == ConnectionState.Open) {
                    objConn.Close();
                }
            } catch (Exception exp) {
                throw exp;
            }
            return true;
        }
    }
}
