using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using zProject.zHelper;
using Newtonsoft.Json;

namespace zProject.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public JsonResult LoginIn(string phonenumber, string pwd)
        {
            //{ "result": true, 
            //"id": 1, 
            //"truename": "唐世平", 
            //"img": "http://www.daydayup.ink/avatar/18005885202.jpg", 
            //"token": "7698438612C5A071EBC1357D5D213F532D04A3A705E6A0A
            //A2672F14745C60A511A499BDAC9088D9542E4A19CF0FB0E59DB8F080D2F
            //AC40A6969C22A2C6088E7946377ED0D59ACB8B1A8C77743E8C9B226
            //B08991F749EE826E70900D3F24757D438FA5C0B0AB7319CA1149A4A45FEA539" }
            //组成post数据
            string postData = "{phonenumber:\"" + phonenumber + "\",pwd:\"" + pwd + "\"}";
            //处理返回的数据
            string s = zHelper.WebHelper.RequestDate("https://www.daydayup.ink/api/User/Login", postData);
            UserJson userJson = JavaScriptConvert.DeserializeObject<UserJson>(s);
            if (userJson.result.Equals("True"))
            {
                //登陆成功，保存token
                Session["userToken"] = userJson.token;

                return Json(0);
            }
            else
            {
                //登录失败，用户名密码不对
                return Json(userJson.err);
            }
        }

        //反序列化json使用
        public class UserJson
        {
            public string result { get; set; }
            public string id { get; set; }
            public string truename { get; set; }
            public string img { get; set; }
            public string token { get; set; }
            public string err { get; set; }
        }
    }
}
