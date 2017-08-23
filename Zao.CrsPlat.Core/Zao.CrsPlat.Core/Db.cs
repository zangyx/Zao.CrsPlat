using System;
using System.Collections.Generic;
using System.Linq;
using Dos.ORM;
using System.Data;
using System.Reflection;
using System.Collections;


namespace Zao.CrsPlat.Core
{
    public static class Db
    {
        private const string logPreString = "Zao_";

        static Db()
        {
            ZaoDb.RegisterSqlLogger(delegate (string sql) {
                if (sql.Contains("INSERT") || sql.Contains("DELETE") || sql.Contains("UPDATE"))
                {
                    Dos.Common.LogHelper.Debug(sql, logPreString);
                }
            });
        }

        public static DbSession GetDbSession(this Entity entity)
        {
            return typeof(Db).GetField(
                ZaoDb.From<Sys_table_dic>()
                .Select(Sys_table_dic._.tableOwner)
                .Where(Sys_table_dic._.tableName == entity.GetType().Name)
                .Top(1).ToScalar().ToString()
                ).GetValue(null) as DbSession;
        }

        public static void ErrorLog(string errorMsg)
        {
            Dos.Common.LogHelper.Error(errorMsg, logPreString);
        }

        /// <summary>
        /// 底层日志记录时写入登陆人的胸牌号
        /// </summary>
        public static string Login { get; set; }
        public static readonly DbSession ZaoDb = new DbSession(DatabaseType.SqlServer9, "Data Source=PC-201707232100\\ZAODB;Initial Catalog=ZaoDb;User Id=Zao;Password=zangyx");
    }
    public static class Extension
    {
        /// <summary>
        /// 扩展插入方法(单条)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static decimal Insert<TEntity>(this TEntity entity)
            where TEntity : Entity
        {
            if (entity == null) return 0;
            int result = (int)entity.GetDbSession()
                .Insert(entity)
                .AddDalLog(entity, "Insert", entity.GetObjectAllProToString(), null);
            return result;
        }

        /// <summary>
        /// 扩展插入方法（单条通用序列做主键）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static decimal Insert<TEntity>(this TEntity entity, bool IsCommonAuto)
            where TEntity : Entity
        {
            if (entity == null) return 0;
            if (IsCommonAuto) ((dynamic)entity).OBJECT_ID =
                Db.ZaoDb.FromSql("select ZaoDb_auto_object_id.nextval from dual")
                .ToScalar<decimal>();
            int result = (int)entity.GetDbSession()
                .Insert(entity)
                .AddDalLog(entity, "Insert", entity.GetObjectAllProToString(), null);
            return ((dynamic)entity).OBJECT_ID;
        }

        /// <summary>
        /// 扩展实体集合的插入方法（List）
        /// </summary>
        /// <param name="lentity"></param>
        /// <returns></returns>
        public static int Insert<TEntity>(this List<TEntity> lentity)
            where TEntity : Entity
        {
            if (lentity.Count == 0) return 0;
            int result = lentity[0].GetDbSession().Insert(lentity);
            foreach (var entity in lentity)
                entity.AddDalLog(entity, "Insert", entity.GetObjectAllProToString(), null);
            return result;
        }

        /// <summary>
        /// 扩展更新方法(单条)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int Update<TEntity>(this TEntity entity)
            where TEntity : Entity
        {
            if (entity == null)
            {
                return 0;
            }
            int result = (int)entity.GetDbSession()
                .Update(entity)
                .AddDalLog(entity, "Update", entity.GetObjectAllProToString(), entity.GetObjectOldProToString());
            return result;
        }

        /// <summary>
        /// 扩展实体集合的更新方法
        /// </summary>
        /// <param name="lentity"></param>
        /// <returns></returns>
        public static int Update<TEntity>(this List<TEntity> lentity)
            where TEntity : Entity
        {
            if (lentity.Count == 0) return 0;
            int result = lentity[0].GetDbSession().Update(lentity);
            foreach (var entity in lentity)
                entity.AddDalLog(entity, "Update", entity.GetObjectAllProToString(), entity.GetObjectOldProToString());
            return result;
        }

        /// <summary>
        /// 扩展删除方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int Delete<TEntity>(this TEntity entity)
            where TEntity : Entity
        {
            if (entity == null) return 0;
            int result = (int)entity.GetDbSession()
                .Delete(entity)
                .AddDalLog(entity, "Delete", entity.GetObjectAllProToString(), null);
            return result;
        }

        /// <summary>
        /// 扩展实体集合的删除方法
        /// </summary>
        /// <param name="lentity"></param>
        /// <returns></returns>
        public static int Delete<TEntity>(this List<TEntity> lentity)
            where TEntity : Entity
        {
            if (lentity.Count == 0)
            {
                return 0;
            }
            var countdel = 0;
            foreach (var entity in lentity)
            {
                countdel += entity.GetDbSession().Delete(entity);
                entity.AddDalLog(entity, "Delete", entity.GetObjectAllProToString(), null);
            }
            return countdel;
        }

        /// <summary>
        /// 给泛型实体类扩展转datatable的方法
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="lentity"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<TEntity>(this List<TEntity> lentity, params string[] propertyName)
            where TEntity : Entity
        {
            var propertyNameList = new List<string>();
            if (propertyName != null)
            {
                propertyNameList.AddRange(propertyName);
            }
            var result = new DataTable();
            if (lentity.Count > 0)
            {
                var propertys = lentity[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        if (pi.PropertyType.IsGenericType)
                        {
                            result.Columns.Add(pi.Name, Nullable.GetUnderlyingType(pi.PropertyType));
                        }
                        else
                        {
                            result.Columns.Add(pi.Name, pi.PropertyType);
                        }
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name))
                        {
                            if (pi.PropertyType.IsGenericType)
                            {
                                result.Columns.Add(pi.Name, Nullable.GetUnderlyingType(pi.PropertyType));
                            }
                            else
                            {
                                result.Columns.Add(pi.Name, pi.PropertyType);
                            }
                        }
                    }
                }

                for (var i = 0; i < lentity.Count; i++)
                {
                    var tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        if (propertyNameList.Count == 0)
                        {
                            var obj = pi.GetValue(lentity[i], null);
                            tempList.Add(obj);
                        }
                        else
                        {
                            if (propertyNameList.Contains(pi.Name))
                            {
                                var obj = pi.GetValue(lentity[i], null);
                                tempList.Add(obj);
                            }
                        }
                    }
                    var array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

        /// <summary>
        /// 重载扩展方法（给泛型实体类扩展转datatable的方法）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="lentity"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<TEntity>(this List<TEntity> lentity)
            where TEntity : Entity
        {
            return ToDataTable<TEntity>(lentity, null);
        }

        /// <summary>
        /// 检查DataTable 是否有数据行
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static bool IsHaveRows(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// datatable扩展tolist方法
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static IList<TEntity> ToList<TEntity>(this DataTable dt)
            where TEntity : Entity
        {
            if (!IsHaveRows(dt))
            {
                return new List<TEntity>();
            }
            IList<TEntity> list = new List<TEntity>();
            var entity = default(TEntity);
            foreach (DataRow dr in dt.Rows)
            {
                entity = dr.ToEntity<TEntity>();
                list.Add(entity);
            }
            return list;
        }

        /// <summary>
        /// 系统日志记录函数
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="opType"></param>
        /// <param name="opContent"></param>
        /// <param name="opOldContent"></param>
        public static object AddDalLog<TEntity>(this object obj, TEntity entity, string opType, string opContent, string opOldContent)
        where TEntity : Entity
        {
            //Db.ZaoDb.Insert(new QAC_IDU_SYSLOG()
            //{
            //    TABLE_NAME = entity.GetTableName(),
            //    OP_TYPE = opType,
            //    OP_CONTENT = opContent,
            //    OP_OLD_CONTENT = opOldContent,
            //    OP_PERSON = Db.Login,
            //    OP_DATE = DateTime.Now,
            //    NAMESPACE =
            //        Db.ZaoDb.From<Sys_table_dic>()
            //        .Where(Sys_table_dic._.tableName == entity.GetTableName())
            //        .First() == null ?
            //        null :
            //        Db.ZaoDb.From<Sys_table_dic>()
            //            .Where(Sys_table_dic._.tableName == entity.GetTableName())
            //            .First().tableOwner,
            //    //NAMESPACE = entity.GetUserName(),
            //    OP_IP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault<IPAddress>(x => x.AddressFamily.ToString().Equals("InterNetwork")).ToString(),
            //    OP_DEVICE = Dns.GetHostName(),
            //    REMARK = WindowsIdentity.GetCurrent().Name
            //});
            //return obj;
            return new object();
        }

        /// <summary>
        /// 系统日志记录函数
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="opType"></param>
        /// <param name="opContent"></param>
        /// <param name="opOldContent"></param>
        public static void DbTransAddDalLog<TEntity>(DbTrans transLog, TEntity entity, string opType, string opContent, string opOldContent)
        where TEntity : Entity
        {
            //transLog.Insert(new QAC_IDU_SYSLOG()
            //{
            //    TABLE_NAME = entity.GetTableName(),
            //    OP_TYPE = opType,
            //    OP_CONTENT = opContent,
            //    OP_OLD_CONTENT = opOldContent,
            //    OP_PERSON = Db.Login,
            //    OP_DATE = DateTime.Now,
            //    NAMESPACE =
            //        Db.ZaoDb.From<Sys_table_dic>()
            //        .Where(Sys_table_dic._.tableName == entity.GetTableName())
            //        .First() == null ?
            //        null :
            //        Db.ZaoDb.From<Sys_table_dic>()
            //            .Where(Sys_table_dic._.tableName == entity.GetTableName())
            //            .First().tableOwner,
            //    //NAMESPACE = entity.GetUserName(),
            //    OP_IP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault<IPAddress>(x => x.AddressFamily.ToString().Equals("InterNetwork")).ToString(),
            //    OP_DEVICE = Dns.GetHostName(),
            //    REMARK = WindowsIdentity.GetCurrent().Name
            //});
        }

        /// <summary>
        /// 增删操作时记录整个对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetObjectAllProToString<TEntity>(this TEntity t)
            where TEntity : Entity
        {
            string result = "对象属性集合{";
            PropertyInfo[] pis = t.GetType().GetProperties();
            foreach (var pi in pis)
            {
                if (pi.GetValue(t, null) != null)
                {
                    result += pi.GetValue(t, null).ToString() + ";";
                }
                else
                {
                    result += "null" + ";";
                }
            }
            return result + "}";
        }

        /// <summary>
        /// 增删操作时记录整个对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetObjectColAllProToString<TEntity>(this TEntity t)
            where TEntity : Entity
        {
            string result = "对象属性集合{";
            PropertyInfo[] pis = t.GetType().GetProperties();
            foreach (var pi in pis)
            {
                if (pi.GetValue(t, null) != null)
                {
                    result += pi.Name + ":" + pi.GetValue(t, null).ToString() + ";";
                }
                else
                {
                    result += pi.Name + ":" + "null" + ";";
                }
            }
            return result + "}";
        }

        /// <summary>
        /// Update操作时记录原状态
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetObjectOldProToString<TEntity>(this TEntity t) where TEntity : Entity
        {
            string result = "对象属性集合{";
            foreach (var mf in t.GetModifyFields())
            {
                var oldV = mf.OldValue == null ?
                    "null" :
                    mf.OldValue.ToString();
                var newV = mf.NewValue == null ?
                    "null" :
                    mf.NewValue.ToString();
                result += mf.Field.PropertyName
                    + ":"
                    + oldV
                    + "(新值："
                    + newV
                    + ");";
            }
            return result + "}";
        }

        /// <summary>
        /// Update操作时记录新状态
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetObjectNewProToString<TEntity>(this TEntity t) where TEntity : Entity
        {
            string result = "对象属性集合{";
            foreach (var mf in t.GetModifyFields())
            {
                if (mf.NewValue != null)
                {
                    result += mf.Field.PropertyName + ":" + mf.NewValue.ToString() + ";";
                }
                else
                {
                    result += mf.Field.PropertyName + ":" + "null" + ";";
                }
            }
            return result + "}";
        }

        /// <summary>
        /// 数据行转化成实体对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static TEntity ToEntity<TEntity>(this DataRow dr)
        where TEntity : Entity
        {
            var entity = default(TEntity);
            if (dr == null) return entity;
            entity = Activator.CreateInstance<TEntity>();
            foreach (DataColumn dc in dr.Table.Columns)
            {
                var drValue = dr[dc.ColumnName];
                var pi = entity.GetType().GetProperty(dc.ColumnName);

                if (pi != null && pi.CanWrite && (drValue != null && !Convert.IsDBNull(drValue)))
                {
                    var type = pi.PropertyType.ToString();
                    if (pi.PropertyType.IsGenericType)
                    {
                        type = Nullable.GetUnderlyingType(pi.PropertyType).ToString();
                    }
                    switch (type)
                    {
                        case "System.Boolean": pi.SetValue(entity, Boolean.Parse(drValue.ToString()), null); break;
                        case "System.Byte": pi.SetValue(entity, Byte.Parse(drValue.ToString()), null); break;
                        case "System.Decimal": pi.SetValue(entity, Decimal.Parse(drValue.ToString()), null); break;
                        case "System.DateTime": pi.SetValue(entity, DateTime.Parse(drValue.ToString()), null); break;
                        case "System.Int16": pi.SetValue(entity, Int16.Parse(drValue.ToString()), null); break;
                        case "System.Int32": pi.SetValue(entity, Int32.Parse(drValue.ToString()), null); break;
                        case "System.Int64": pi.SetValue(entity, Int64.Parse(drValue.ToString()), null); break;
                        case "System.Single": pi.SetValue(entity, Single.Parse(drValue.ToString()), null); break;
                        default: pi.SetValue(entity, drValue, null); break;
                    }
                }
            }
            return entity;
        }

        /// <summary>
        /// 扩展插入方法(单条)（事务）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static decimal Insert<TEntity>(this TEntity entity, DbTrans trans, DbTrans transLog)
            where TEntity : Entity
        {
            if (entity == null) return 0;
            int result = (int)trans.Insert(entity);
            DbTransAddDalLog(transLog, entity, "Insert", entity.GetObjectAllProToString(), null);
            return result;
        }

        /// <summary>
        /// 扩展实体集合的插入方法（List）(事务)
        /// </summary>
        /// <param name="lentity"></param>
        /// <returns></returns>
        public static int Insert<TEntity>(this List<TEntity> lentity, DbTrans trans, DbTrans transLog)
            where TEntity : Entity
        {
            if (lentity.Count == 0) return 0;
            int result = 0;
            foreach (var entity in lentity)
            {
                result += trans.Insert(entity);
                DbTransAddDalLog(transLog, entity, "Insert", entity.GetObjectAllProToString(), null);
            }
            return result;
        }

        /// <summary>
        /// 扩展更新方法(单条)（事务）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int Update<TEntity>(this TEntity entity, DbTrans trans, DbTrans transLog)
            where TEntity : Entity
        {
            if (entity == null) return 0;
            int result = (int)trans.Update(entity);
            DbTransAddDalLog(transLog, entity, "Update", entity.GetObjectAllProToString(), entity.GetObjectOldProToString());
            return result;
        }

        /// <summary>
        /// 扩展实体集合的更新方法（事务）
        /// </summary>
        /// <param name="lentity"></param>
        /// <returns></returns>
        public static int Update<TEntity>(this List<TEntity> lentity, DbTrans trans, DbTrans transLog)
            where TEntity : Entity
        {
            if (lentity.Count == 0) return 0;
            int result = 0;
            foreach (var entity in lentity)
            {
                result += trans.Update(entity);
                DbTransAddDalLog(transLog, entity, "Update", entity.GetObjectAllProToString(), entity.GetObjectOldProToString());
            }
            return result;
        }

        /// <summary>
        /// 扩展删除方法(事务)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int Delete<TEntity>(this TEntity entity, DbTrans trans, DbTrans transLog)
            where TEntity : Entity
        {
            if (entity == null) return 0;
            int result = (int)trans.Delete(entity);
            DbTransAddDalLog(transLog, entity, "Delete", entity.GetObjectAllProToString(), null);
            return result;
        }

        /// <summary>
        /// 扩展实体集合的删除方法
        /// </summary>
        /// <param name="lentity"></param>
        /// <returns></returns>
        public static int Delete<TEntity>(this List<TEntity> lentity, DbTrans trans, DbTrans transLog)
            where TEntity : Entity
        {
            if (lentity.Count == 0) return 0;
            var countdel = 0;
            foreach (var entity in lentity)
            {
                countdel += trans.Delete(entity);
                DbTransAddDalLog(transLog, entity, "Delete", entity.GetObjectAllProToString(), null);
            }
            return countdel;
        }
    }
}
