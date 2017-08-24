using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zao.CrsPlat.DevExpressHelper.ZaoUtil
{
    public static class ZaoStringTool {

        public static string BreakLongString(string subjectString, int lineLength) {
            StringBuilder sb = new StringBuilder(subjectString);
            int offset = 0;
            ArrayList indexList = BuildInsertIndexList(subjectString, lineLength);
            for (int i = 0; i < indexList.Count; i++) {
                sb.Insert((int)indexList[i] + offset, '\n');
                offset++;
            }
            return sb.ToString();
        }

        public static bool IsChinese(char c) {
            return (int)c >= 0x4E00 && (int)c <= 0x9FA5;
        }

        public static ArrayList BuildInsertIndexList(string str, int maxLen) {
            int nowLen = 0;
            ArrayList list = new ArrayList();
            for (int i = 0; i < str.Length; i++) {
                if (IsChinese(str[i])) {
                    nowLen += 2;
                } else {
                    nowLen++;
                }
                if (nowLen > maxLen) {
                    nowLen = 0;
                    list.Add(i);
                }
            }
            return list;
        }

    }
}
