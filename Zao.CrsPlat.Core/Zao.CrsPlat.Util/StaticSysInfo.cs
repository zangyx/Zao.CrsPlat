using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zao.CrsPlat.Util
{
    public static class StaticSysInfo
    {
        private static string login;
        private static string ipAdress;
        /// <summary>
        /// 登陆用户名
        /// </summary>
        public static string Login { get => login; set => login = value; }
        /// <summary>
        /// 登陆ip地址
        /// </summary>
        public static string IpAdress { get => ipAdress; set => ipAdress = value; }
    }
}
