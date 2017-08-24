using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraTreeList;


namespace Zao.CrsPlat.DevExpressHelper.ZaoUtil
{
    public class ZaoTreeOp_T<T> where T : Dos.ORM.Entity,new() {
        public ZaoTreeOp_T() {

        }
        public List<T> Source { get; set; }
        public TreeList TreeListAim { get; set; }



    }
}
