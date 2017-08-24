using System.Data;


namespace Zao.CrsPlat.DevExpressHelper.ZaoUtil
{

    public class ZaoMasterDetail {
        private DataTable _dataSource;

        public DataTable DataSource
        {
            get
            {
                return _dataSource;
            }

            set
            {
                _dataSource = value;
            }
        }

        /// <summary>
        /// 添加一个从表，并建立与父表的主从关系
        /// </summary>
        /// <param name="dSource">主表</param>
        /// <param name="dSonSource">从表</param>
        /// <param name="fTableName">父表表名</param>
        /// <param name="sTableName">子表表名</param>
        /// <param name="relationName">关系名称</param>
        /// <param name="colFather">父层外键</param>
        /// <param name="colSon">子层主键</param>
        public ZaoMasterDetail AddOneSon(DataTable dSource,
            DataTable dSonSource,
            string fTableName,
            string sTableName,
            string relationName,
            string colFather,
            string colSon) {
            AddLevelTable(fTableName,
                SetTableName(dSource, fTableName),
                sTableName,
                dSonSource)
                .DataSet.Relations.Add(relationName,
                dSource.DataSet.Tables[fTableName].Columns[colFather],
                dSource.DataSet.Tables[sTableName].Columns[colSon],
                false);
            DataSource = dSource;
            return this;
        }

        /// <summary>
        /// 添加一个从表，并建立与父表的主从关系
        /// </summary>
        /// <param name="dSonSource">从表</param>
        /// <param name="fTableName">父表表名</param>
        /// <param name="sTableName">子表表名</param>
        /// <param name="relationName">关系名称</param>
        /// <param name="colFather">父层外键</param>
        /// <param name="colSon">子层主键</param>
        public ZaoMasterDetail AddOneSon(DataTable dSonSource,
            string fTableName,
            string sTableName,
            string relationName,
            string colFather,
            string colSon) {
            AddLevelTable(fTableName,
                DataSource,
                sTableName,
                dSonSource)
                .DataSet.Relations.Add(relationName,
                DataSource.DataSet.Tables[fTableName].Columns[colFather],
                DataSource.DataSet.Tables[sTableName].Columns[colSon],
                false);
            return this;
        }

        /// <summary>
        /// 设定DataTable的表名属性（TableName）（重要，建立关系的依据）
        /// </summary>
        /// <param name="dt">表</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        private DataTable SetTableName(DataTable dt, string tableName) {
            dt.TableName = tableName;
            return dt;
        }

        /// <summary>
        /// 将子表，对应层的名称，以及DataTable挂到主表的DataSet上
        /// </summary>
        /// <param name="fTableName">主表名称</param>
        /// <param name="dataSource">主表</param>
        /// <param name="levelName">子层名称</param>
        /// <param name="levelDT">子表数据</param>
        private DataTable AddLevelTable(string fTableName,
            DataTable dataSource,
            string levelName,
            DataTable levelDT) {
            dataSource.DataSet.Tables.Add(SetTableName(levelDT, levelName).Copy());
            return dataSource;
        }
    }
}