using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OnlineShoesMarket.Controllers
{
    public class BackendController : Controller
    {
        public ActionResult OrderList()
        {
            return View();
        }

        public JsonResult GetOrderList(OrderRequest request)
        {
            DataTable dt = new DataTable(); // 实例化数据表
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationSettings.AppSettings["connectionstring"]; ;
                conn.Open(); // 打开数据库连接

                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT * FROM [SHOES].[dbo].[Order] where 1=1 "); // 查询语句

                if (request != null)
                {
                    if (!string.IsNullOrEmpty(request.OrderNo))
                    {
                        sql.Append($" AND ID = '{request.OrderNo}'");
                    }
                    if (!string.IsNullOrEmpty(request.ReceiveName))
                    {
                        sql.Append($" AND ReceiveName LIKE '%{request.ReceiveName}%'");
                    }
                    if (!string.IsNullOrEmpty(request.ReceivePhone))
                    {
                        sql.Append($" AND ReceivePhone = '{request.ReceivePhone}'");
                    }
                    if (!string.IsNullOrEmpty(request.ProductName))
                    {
                        sql.Append($" AND ProductName LIKE '%{request.ProductName}%'");
                    }
                    if (!string.IsNullOrEmpty(request.Address))
                    {
                        sql.Append($" AND ReceiveAddress LIKE '%{request.Address}%'");
                    }
                    if (!string.IsNullOrEmpty(request.Status))
                    {
                        sql.Append($" AND Status = '{request.Status}'");
                    }
                }


                SqlDataAdapter myda = new SqlDataAdapter(sql.ToString(), conn); // 实例化适配器
                myda.Fill(dt); // 保存数据 
                conn.Close(); // 关闭数据库连接
            }

            var result = new List<OrderDetail>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    result.Add(new OrderDetail
                    {
                        ID = item["ID"].ToString(),
                        ProductID = item["ProductID"].ToString(),
                        ProductName = item["ProductName"].ToString(),
                        UnitPrice = decimal.Parse(item["UnitPrice"].ToString()),
                        ReceivePhone = item["ReceivePhone"].ToString(),
                        ReceiveAddress = item["ReceiveAddress"].ToString(),
                        ReceiveName = item["ReceiveName"].ToString(),
                        CreateTime = DateTime.Parse(item["CreateTime"].ToString()),
                        Count = int.Parse(item["Count"].ToString()),
                        Status = short.Parse(item["Status"].ToString()),
                        TotalPrice = string.IsNullOrEmpty(item["TotalPrice"].ToString()) ? decimal.Parse("0") : decimal.Parse(item["TotalPrice"].ToString())
                    });
                }
            }

            var res = new test { rows = result, total = result.Count };
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrderDetail(string orderid)
        {
            DataTable dt = new DataTable(); // 实例化数据表
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationSettings.AppSettings["connectionstring"]; ;
                conn.Open(); // 打开数据库连接

                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT * FROM [SHOES].[dbo].[Order] where ID=" + orderid); // 查询语句

                SqlDataAdapter myda = new SqlDataAdapter(sql.ToString(), conn); // 实例化适配器
                myda.Fill(dt); // 保存数据 
                conn.Close(); // 关闭数据库连接
            }

            if (dt != null && dt.Rows.Count > 0)
            {
                var item = dt.Rows[0];
                var detail = new OrderDetail
                {
                    ID = item["ID"].ToString(),
                    ProductID = item["ProductID"].ToString(),
                    ProductName = item["ProductName"].ToString(),
                    UnitPrice = decimal.Parse(item["UnitPrice"].ToString()),
                    ReceivePhone = item["ReceivePhone"].ToString(),
                    ReceiveAddress = item["ReceiveAddress"].ToString(),
                    ReceiveName = item["ReceiveName"].ToString(),
                    Color = item["Color"].ToString(),
                    Size = item["Size"].ToString(),
                    PayType = short.Parse(item["PayType"].ToString()),
                    Remarks = item["Remarks"].ToString(),
                    ReceiveCityID = item["ReceiveCityID"].ToString(),
                    CreateUser = item["CreateUser"].ToString(),
                    UpdateUser = item["UpdateUser"].ToString(),
                    UpdateTime = DateTime.Parse(item["UpdateTime"].ToString()),
                    CreateTime = DateTime.Parse(item["CreateTime"].ToString()),
                    Count = int.Parse(item["Count"].ToString()),
                    Status = short.Parse(item["Status"].ToString()),
                    TotalPrice = string.IsNullOrEmpty(item["TotalPrice"].ToString()) ? decimal.Parse("0") : decimal.Parse(item["TotalPrice"].ToString())
                };
                return Json(detail, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }


        public JsonResult UpdateOrderStatus(int status, string orderId)
        {
            var result = new { isSuccess = true, message = string.Empty };
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = ConfigurationSettings.AppSettings["connectionstring"]; ;
                    conn.Open(); // 打开数据库连接
                    var excuteSql = $@" update[SHOES].[dbo].[Order] set Status = {status}, 
                                        UpdateTime = GETDATE(), UpdateUser = 'admin' where ID = {orderId}; ";
                    SqlCommand cmd = new SqlCommand(excuteSql, conn);
                    var count = cmd.ExecuteNonQuery();
                    conn.Close(); // 关闭数据库连接
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                result = new { isSuccess = false, message = ex.Message };
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }

    public class OrderDetail
    {
        public string ID { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public string ReceiveName { get; set; }
        public string ReceivePhone { get; set; }
        public string ReceiveAddress { get; set; }
        public int Count { get; set; }
        public short Status { get; set; }
        public decimal TotalPrice { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string ReceiveCityID { get; set; }
        public short PayType { get; set; }
        public string Remarks { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUser { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateUser { get; set; }
    }

    public class test
    {
        public int total;

        public List<OrderDetail> rows;
    }

    public class OrderRequest
    {
        public string OrderNo { get; set; }
        public string ReceivePhone { get; set; }
        public string ReceiveName { get; set; }
        public string ProductName { get; set; }
        public string Status { get; set; }
        public string Address { get; set; }
    }
}
