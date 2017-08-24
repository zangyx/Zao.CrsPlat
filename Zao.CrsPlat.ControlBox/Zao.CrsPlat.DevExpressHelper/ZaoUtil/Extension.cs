using System;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;


namespace Zao.CrsPlat.DevExpressHelper.ZaoUtil
{

    public static class Extension {
        /// <summary>
        /// 扩展方法，将对象转换为string格式并取消前后空格,如果对象未null，返回“”
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToStrTrim(this object obj) {
            if (obj != null) {
                return obj.ToString().Trim();
            } else {
                return string.Empty;
            }
        }

        /// <summary>
        /// 扩展方法，将对象转换为int型，若遇错误，则返回设定值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="error">错误时返回值</param>
        /// <returns></returns>
        public static int ToInt(this object obj, int error = 0) {
            try {
                return int.Parse(obj.ToString().Trim());
            } catch (Exception) {
                return error;
            }
        }

        /// <summary>
        /// 扩展方法，将对象转换为double型，若遇错误，则返回设定值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="error">错误时返回值</param>
        /// <returns></returns>
        public static double ToDouble(this object obj, int error = 0) {
            try {
                return double.Parse(obj.ToString().Trim());
            } catch (Exception) {
                return error;
            }
        }

        /// <summary>
        /// 扩展方法，将对象转换为decimal型，若遇错误，则返回设定值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="error">错误时返回值</param>
        /// <returns></returns>
        public static decimal ToDecimal(this object obj, int error = 0) {
            try {
                return decimal.Parse(obj.ToString().Trim());
            } catch (Exception) {
                return error;
            }
        }

        /// <summary>
        /// 扩展方法，将对象转换为float型，若遇错误，则返回设定值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="error">错误时返回值</param>
        /// <returns></returns>
        public static float ToSingle(this object obj, int error = 0) {
            try {
                return float.Parse(obj.ToString().Trim());
            } catch (Exception) {
                return error;
            }
        }

        /// <summary>
        /// 扩展方法，将对象转换为DateTime型，若遇错误，则返回设定值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="error">错误时返回值</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object obj) {
            try {
                return DateTime.Parse(obj.ToString().Trim());
            } catch (Exception) {
                return DateTime.Parse("1900-01-01");
            }

        }

        /// <summary>
        /// 扩展方法，将对象转换为DateTime型，若遇错误，则返回设定值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="error">错误时返回值</param>
        /// <returns></returns>
        public static DateTime? ConvertDateTime(this object obj) {
            DateTime dt;
            if (DateTime.TryParse(obj.ToString().Trim(), out dt)) {
                return dt.Date;
            } else {
                return null;
            }
        }

        /// <summary>
        /// 扩展方法，将对象转换为DateTime?型，若遇错误，则返回null
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="error">错误时返回值</param>
        /// <returns></returns>
        public static DateTime? ToNullableDateTime(this object obj) {
            try {
                return DateTime.Parse(obj.ToString().Trim());
            } catch (Exception) {
                return null;
            }
        }

        /// <summary>
        /// 扩展方法，将ERP工时对象转换为分钟数，若遇错误，则返回0
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static string ToErpMinutes(this object obj, string error = "0") {
            try {
                if (obj.ToString().Contains(".")) {
                    var objArr = obj.ToString().Split('.');
                    return ((objArr[0].ToDecimal() * 60) + objArr[1].ToDecimal()).ToString();
                } else {
                    return (obj.ToString().ToDecimal() * 60).ToString();
                }
            } catch (Exception) {
                return error;
            }
        }

        /// <summary>
        /// 扩展方法，将分钟数对象转换为ERP工时，若遇错误，则返回0
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static string ToErpHours(this object obj, string error = "0") {
            try {
                if (obj.ToDecimal() % 60 < 10) {
                    return Math.Floor(obj.ToDecimal() / 60).ToString() + ".0" + Math.Floor(obj.ToDecimal() % 60).ToString();
                } else {
                    return Math.Floor(obj.ToDecimal() / 60).ToString() + "." + Math.Floor(obj.ToDecimal() % 60).ToString();
                }
            } catch (Exception) {
                return error;
            }
        }

        /// <summary>
        /// 拓展方法，ERP工时对象乘以倍数后转化为ERP工时
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="multiple"></param>
        /// <returns></returns>
        public static string MultiErpHours(this object obj, string multiple) {
            if (obj.ToString().Contains(".")) {
                var proTimeOldArr = obj.ToString().Split('.');
                var minutesSubs = ((proTimeOldArr[0].ToDecimal() * 60) + proTimeOldArr[1].ToDecimal()) * multiple.ToDecimal();
                if (minutesSubs % 60 < 10) {
                    return Math.Floor(minutesSubs / 60).ToString() + ".0" + Math.Floor(minutesSubs % 60).ToString();
                } else {
                    return Math.Floor(minutesSubs / 60).ToString() + "." + Math.Floor(minutesSubs % 60).ToString();
                }
            } else {
                var minutesSubs = obj.ToString().ToDecimal() * 60 * multiple.ToDecimal();
                if (minutesSubs % 60 < 10) {
                    return Math.Floor(minutesSubs / 60).ToString() + ".0" + Math.Floor(minutesSubs % 60).ToString();
                } else {
                    return Math.Floor(minutesSubs / 60).ToString() + "." + Math.Floor(minutesSubs % 60).ToString();
                }
            }
        }

        /// <summary>
        /// 反射获取字段值
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static object GetValue(this object entity, string fieldName) {
            try {
                return entity.GetType().GetProperty(fieldName).GetValue(entity, null);
            } catch (Exception) {
                throw;
            }
        }

        /// <summary>
        /// 反射设定字段值
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldvalue"></param>
        public static void SetValue(this object entity, string fieldName, string fieldvalue) {
            if (entity == null) { return; }
            Type entityType = entity.GetType();
            PropertyInfo pi = entityType.GetProperty(fieldName);
            if (IsType(pi.PropertyType, "System.Nullable`1[System.DateTime]")) {
                if (fieldvalue != "") {
                    try {
                        pi.SetValue(entity, (DateTime?)DateTime.ParseExact(fieldvalue, "yyyy-MM-dd", null), null);
                    } catch (Exception) {
                        pi.SetValue(entity, (DateTime?)DateTime.ParseExact(fieldvalue, "yyyy-MM-dd hh24:mi:ss", null), null);
                    }
                } else {
                    pi.SetValue(entity, null, null);
                }
            }
            if (IsType(pi.PropertyType, "System.String")) {
                pi.SetValue(entity, fieldvalue, null);
            }
            if (IsType(pi.PropertyType, "System.Nullable`1[System.Int32]")
                || IsType(pi.PropertyType, "System.Nullable`1[System.Int64]")
                || IsType(pi.PropertyType, "System.Nullable`1[System.Decimal]")) {
                pi.SetValue(entity, fieldvalue.ToDecimal(), null);
            }
        }

        /// <summary>
        /// 反射判断属性的数据类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static bool IsType(Type type, string typeName) {
            if (type.ToString() == typeName) {
                return true;
            }
            if (type.ToString() == "System.Object") {
                return false;
            }
            //递归一直找到Object，确定是否继承自typeName
            return IsType(type.BaseType, typeName);
        }

        public static object ToJson(this string Json) {
            return Json == null ? null : JsonConvert.DeserializeObject(Json);
        }
        public static string ToJson(this object obj) {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            return JsonConvert.SerializeObject(obj, timeConverter);
        }
        public static string ToJson(this object obj, string datetimeformats) {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = datetimeformats };
            return JsonConvert.SerializeObject(obj, timeConverter);
        }
        public static T ToObject<T>(this string Json) {
            return Json == null ? default(T) : JsonConvert.DeserializeObject<T>(Json);
        }
        public static List<T> ToList<T>(this string Json) {
            return Json == null ? null : JsonConvert.DeserializeObject<List<T>>(Json);
        }
        public static DataTable ToTable(this string Json) {
            return Json == null ? null : JsonConvert.DeserializeObject<DataTable>(Json);
        }
        public static JObject ToJObject(this string Json) {
            return Json == null ? JObject.Parse("{}") : JObject.Parse(Json.Replace("&nbsp;", ""));
        }

        /// <summary>
        /// IEnumerable聚合根据Key字段Distinct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector) {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (var element in source) {
                if (seenKeys.Add(keySelector(element))) {
                    yield return element;
                }
            }
        }
    }
}
