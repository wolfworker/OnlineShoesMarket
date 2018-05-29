using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShoesMarket.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult OrderList()
        {
            return View();
        }

        public JsonResult GetOrderList()
        {
            DataTable dt = new DataTable(); // 实例化数据表
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationSettings.AppSettings["connectionstring"]; ;
                conn.Open(); // 打开数据库连接

                String sql = "SELECT * FROM [SHOES].[dbo].[Order]"; // 查询语句
                SqlDataAdapter myda = new SqlDataAdapter(sql, conn); // 实例化适配器
                myda.Fill(dt); // 保存数据 
                conn.Close(); // 关闭数据库连接
            }

            var result = new List<Entity>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    result.Add(new Entity
                    {
                        ID = item["ID"].ToString(),
                        ProductID = item["ProductID"].ToString(),
                        ProductName = item["ProductName"].ToString(),
                        UnitPrice = item["UnitPrice"].ToString(),
                        ReceivePhone = item["ReceivePhone"].ToString(),
                        ReceiveAddress = item["ReceiveAddress"].ToString(),
                        ReceiveName = item["ReceiveName"].ToString(),
                        CreateTime = item["CreateTime"].ToString(),
                        Count = int.Parse(item["Count"].ToString())
                    });
                }
            }

            var res = new test { rows = result, total = result.Count };
            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
    public class Entity
    {

        public string ID;
        public string ProductID;
        public string ProductName;
        public string UnitPrice;
        public string ReceiveName;
        public string ReceivePhone;
        public string ReceiveAddress;
        public string CreateTime;
        public int Count;
    }

    public class test
    {
        public int total;

        public List<Entity> rows;
    }
}
