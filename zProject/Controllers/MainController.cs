using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using zProject.zHelper;
using Newtonsoft.Json;

namespace zProject.Controllers
{
    public class MainController : Controller
    {
        //原始职责
        public static List<Duty> publicDuties;
        //treeview使用的节点表
        public static List<Node> publicNode;
        //构建节点表时使用，节点计数
        public static int jishu = 0;
        //保存真正的职责id
        public static string trueId;
        public ActionResult Index()
        {
            GetAllDuty();

            return View();
        }

        /// <summary>
        /// 添加职责
        /// </summary>
        /// <returns>添加成功与否的信息</returns>
        /// <param name="dutyName">职责名称</param>
        /// <param name="parentId">父id</param>
        /// <param name="dutyRole">角色名称</param>
        [HttpPost]
        public JsonResult AddDuty(string dutyName, string parentId, string dutyRole)
        {
            //http://www.daydayup.ink/api/User/AddDuty
            //{name:“职责的名称”,parentid:“父节点id(可为null)”，role:“角色名称（可任意的几个字符）”}
            //组成post数据
            string postData = "{name:\"" + dutyName + "\",parentid:\"" + parentId + "\",role:\"" + dutyRole + "\"}";
            //获取token
            string token = Session["userToken"].ToString();
            //处理返回的数据
            string data = WebHelper.BRequestDate("http://www.daydayup.ink/api/User/AddDuty", postData, token);
            Duties duties = JavaScriptConvert.DeserializeObject<Duties>(data);
            string postResult = duties.result;
            if (postResult.Equals("True"))
            {
                //成功，返回0
                return Json(0);
            }
            else
            {
                //失败,返回提示信息
                string returninfo = duties.err;
                return Json(returninfo);
            }
        }

        /// <summary>
        /// 修改职责
        /// </summary>
        /// <returns>修改成功与否的信息</returns>
        /// <param name="dutyName">职责名称</param>
        /// <param name="currentId">欲修改的职责id</param>
        /// <param name="dutyRole">角色名称</param>
        [HttpPost]
        public JsonResult EditDuty(string dutyName, string currentId, string dutyRole)
        {
            //http://www.daydayup.ink/api/User/EditDuty
            //{name:“职责的新名称”,id:“编辑的节点id”，role:“新角色名称（可任意的几个字符）”}
            string postData = "{name:\"" + dutyName + "\",id:\"" + currentId + "\",role:\"" + dutyRole + "\"}";
            string token = Session["userToken"].ToString();

            string data = WebHelper.BRequestDate("http://www.daydayup.ink/api/User/EditDuty", postData, token);
            Duties duties = JavaScriptConvert.DeserializeObject<Duties>(data);
            string postResult = duties.result;
            if (postResult.Equals("True"))
            {
                //成功，返回0
                return Json(0);
            }
            else
            {
                //失败,返回提示信息
                string returninfo = duties.err;
                return Json(returninfo);
            }
        }

        /// <summary>
        /// 删除职责
        /// </summary>
        /// <returns>删除成功与否的信息</returns>
        /// <param name="removeDutyId">欲删除的职责id</param>
        [HttpPost]
        public JsonResult RemoveDuty(string removeDutyId)
        {
            //http://www.daydayup.ink/api/User/RemoveDuty
            //{ id:“要删除的职责id” }
            string postData = "{id:\"" + removeDutyId + "\"}";
            string token = Session["userToken"].ToString();
            string data = WebHelper.BRequestDate("http://www.daydayup.ink/api/User/RemoveDuty", postData, token);
            Duties duties = JavaScriptConvert.DeserializeObject<Duties>(data);
            string postResult = duties.result;
            if (postResult.Equals("True"))
            {
                //成功，返回0
                return Json(0);
            }
            else
            {
                //失败,返回提示信息
                string returninfo = duties.err;
                return Json(returninfo);
            }
        }

        /// <summary>
        /// 移动职责
        /// </summary>
        /// <returns>0：没有父节点，上移失败，结果无变化。1：移动成功</returns>
        /// <param name="moveDutyId">Move duty identifier.</param>
        /// <param name="boolStr">Bool string.</param>
        [HttpPost]
        public JsonResult MoveDuty(string moveDutyId, string boolStr)
        {
            //获取真正的id 
            GetMoveDutyIdByRecursion(Convert.ToInt32(moveDutyId), publicNode);
            //移动服务器端
            //http://www.daydayup.ink/api/User/MoveDuty
            //{ id:“待移动的职责id”，ismoveup:“true或者false（上移或下移）”}
            string postData = "{id:\"" + trueId + "\",ismoveup:" + boolStr + "}";
            string data = WebHelper.BRequestDate("http://www.daydayup.ink/api/User/MoveDuty", postData, Session["userToken"].ToString());
            //返回的信息
            Duties duties = JavaScriptConvert.DeserializeObject<Duties>(data);
            //移动网页端

            Duty currentDuty = publicDuties.Find(s => s.id == trueId);
            if (boolStr.Equals("true"))
            {
                if (currentDuty.parentid != null)
                {
                    //有父节点，上移，移动到父节点。没有父节点，不动
                    //服务器端上移有问题  上移有时会移动到同级之下
                    Duty parentDuty = publicDuties.Find(s => s.id == currentDuty.parentid);
                    currentDuty.parentid = parentDuty.parentid;
                    //欲移动节点的位置
                    int moveId = publicDuties.FindIndex(s => s.id == currentDuty.id);
                    //修改欲移动节点
                    publicDuties[moveId] = currentDuty;

                    return Json(1);//上移成功
                }
                else
                {
                    return Json(0);//上移失败 已经是最高节点
                }

            }
            else if (boolStr.Equals("false"))
            {
                //下移  下移这点没弄明白服务器端运行的逻辑
                //Duty childrenDuty = publicDuties.Find(s => s.parentid == currentDuty.id);
                //if (childrenDuty == null)
                //{
                //    //这点有疑问，服务器里面没有子节点，是往上移动，我按转服务器的逻辑来写的
                //    //没有子节点，有一个同级节点：移动到该节点下
                //    //没有子节点，上移
                //    List<Duty> parentDuty = publicDuties.FindAll(s => s.id == currentDuty.parentid);
                //    if (parentDuty == null)
                //    {
                //        return Json(0);
                //    }
                //    else
                //    {
                //        currentDuty.parentid = parentDuty[0].parentid;
                //    }
                //}
                //else
                //{
                //    //有子节点 往下移
                //    //当前节点的父节点
                //    childrenDuty.parentid = currentDuty.parentid;
                //    currentDuty.parentid = childrenDuty.id;
                //    //修改欲移动节点的子节点
                //    int childrenId = publicDuties.FindIndex(s => s.id == childrenDuty.id);
                //    publicDuties[childrenId] = childrenDuty;
                //}
                //调用服务器端
                GetAllDuty();
                return Json(2);//下移成功
            }
            //无意义
            return Json(9999);
        }

        /// <summary>
        /// 获取服务器职责信息并保存以供使用
        /// </summary>
        public void GetAllDuty()
        {
            string token = Session["userToken"].ToString();
            string data = WebHelper.ARequestDate("http://www.daydayup.ink/api/User/GetDuties", token);
            RootDuty rootDuty = JavaScriptConvert.DeserializeObject<RootDuty>(data);
            List<Duties> dutiesList = rootDuty.duties;
            var listduty = new List<Duty>();
            foreach (Duties i in dutiesList)
            {
                var oModel = new Duty
                {
                    id = i.id,
                    name = i.name,
                    parentid = i.parentid,
                    index = i.index,
                    role = i.role,
                    level = i.level
                };
                listduty.Add(oModel);
            }

            publicDuties = listduty;
        }

        /// <summary>
        /// 递归获取真正的职责id
        /// </summary>
        /// <param name="flag">前端的节点id</param>
        /// <param name="l">当前层次的Node列表</param>
        private void GetMoveDutyIdByRecursion(int flag, List<Node> l)
        {
            for (int i = 0; i < l.Count; i++)
            {
                if (flag == l[i].nodeId)
                {
                    trueId = l[i].nodePath;
                    break;
                }
                if (l[i].nodes.Count > 0)
                {
                    GetMoveDutyIdByRecursion(flag, l[i].nodes);
                }
            }
        }


        /// <summary>
        /// 递归
        /// </summary>
        /// <returns>The recursion.</returns>
        /// <param name="pid">Pid.</param>
        /// <param name="obj">Object.</param>
        private List<Node> BuildNodeByRecursion(string pid, List<Duty> obj)
        {
            var nodes = new List<Node>();

            for (int i = 0; i < obj.Count; i++)
            {
                if (obj[i].parentid == pid)
                {
                    //设置每一个节点的属性,jishu是节点id,nodePath节点数据来源id，节点来源id的父id，职责的个人名称+角色名称，treeview中的子节点，用递归读取
                    nodes.Add(new Node(jishu++, obj[i].id, obj[i].parentid, obj[i].id + " " + obj[i].name + " " + obj[i].role, BuildNodeByRecursion(obj[i].id, obj)));
                }
            }
            if (jishu == publicDuties.Count)
            {
                //计数值达到duty一共的个数就初始化
                jishu = 0;
            }
            return nodes;
        }

        /// <summary>
        /// 前端treeview调用 获取显示数据
        /// </summary>
        /// <returns>The start.</returns>
        [HttpPost]
        public JsonResult ReStart()
        {
            //构建前端显示需要的数据结构  pid为空的是根节点
            publicNode = BuildNodeByRecursion(null, publicDuties);

            return Json(publicNode, JsonRequestBehavior.AllowGet);
        }

        //通用
        public class Duties
        {
            public string id { get; set; }
            public string name { get; set; }
            public string parentid { get; set; }
            public string index { get; set; }
            public string role { get; set; }
            public string level { get; set; }
            public string moduleid { get; set; }
            public string result { get; set; }
            public string err { get; set; }

        }
        //精简的duty
        public class Duty
        {
            public string id { get; set; }
            public string name { get; set; }
            public string parentid { get; set; }
            public string index { get; set; }
            public string role { get; set; }
            public string level { get; set; }
        }
        //根duty信息
        public class RootDuty
        {
            public List<Duties> duties { get; set; }
            public string result { get; set; }
        }
        //treeview
        public class Node
        {
            public Node() { }
            public Node(int nodeid, string path, string pid, string name, List<Node> n)
            {
                nodeId = nodeid;
                nodePath = path;
                pId = pid;
                text = name;
                nodes = n;
            }
            public int nodeId;  //treeview的节点Id
            public string nodePath;  //treeview节点数据来源id
            public string text;   //职责的个人名称
            public string pId;      //来源id的父id
            public List<Node> nodes;    //treeview中的子节点，用递归读取
        }
    }
}