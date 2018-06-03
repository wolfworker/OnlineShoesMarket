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
    public class OrderController : Controller
    {
        public ActionResult Submit()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SubmitOrder(OrderDetail entity)
        {
            
            if (entity == null)
            {
                return Json(new { isSuccess = false, message = "未能成功下单：数据为空！" }, JsonRequestBehavior.AllowGet);
            }

            var validRequest = true;
            if (string.IsNullOrEmpty(entity.ProductName) || entity.UnitPrice == 0 || string.IsNullOrEmpty(entity.ReceiveName)
                || string.IsNullOrEmpty(entity.ReceivePhone) || string.IsNullOrEmpty(entity.ReceiveAddress) || entity.Count < 1
                || string.IsNullOrEmpty(entity.Color) || string.IsNullOrEmpty(entity.Size)
                )
            {
                validRequest = false;
            }

            if (!validRequest)
            {
                return Json(new { isSuccess = false, message = "未能成功下单：请填写完整下单数据！" }, JsonRequestBehavior.AllowGet);
            }
            entity.ID = DateTime.Now.ToString("yyyyMMddHHmmss")+new Random().Next(100,999);
            entity.CreateTime = DateTime.Now;
            entity.UpdateTime = DateTime.Now;
            entity.CreateUser = "admin";
            entity.UpdateUser = "admin";
            entity.Status = 1;
            entity.TotalPrice = entity.TotalPrice == 0 ? (entity.UnitPrice * entity.Count) : entity.TotalPrice;
            var result = new object();
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = ConfigurationManager.AppSettings["connectionstring"];
                    conn.Open(); // 打开数据库连接
                    var excuteSql = $@"INSERT INTO [SHOES].[dbo].[Order]
                                   ([ID] ,[ProductID] ,[ProductName] ,[UnitPrice] ,[Color] ,[Size] ,[Count]
                                   ,[TotalPrice] ,[ReceiveName] ,[ReceivePhone] ,[ReceiveCityID]
                                   ,[ReceiveAddress] ,[PayType] ,[Status] ,[Remarks] ,[CreateTime]
                                   ,[CreateUser] ,[UpdateTime] ,[UpdateUser])
                             VALUES
                                   ('{entity.ID}' ,'{entity.ProductID}' ,'{entity.ProductName}' ,{entity.UnitPrice}
                                   ,'{entity.Color}' ,'{entity.Size}' ,{entity.Count} ,{entity.TotalPrice}
                                   ,'{entity.ReceiveName}' ,'{entity.ReceivePhone}' ,'{entity.ReceiveCityID}'
                                   ,'{entity.ReceiveAddress}' ,{entity.PayType} ,{entity.Status} ,'{entity.Remarks}'
                                   ,'{entity.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")}' ,'{entity.CreateUser}' ,'{entity.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")}' ,'{entity.UpdateUser}'); ";

                    SqlCommand cmd = new SqlCommand(excuteSql, conn);
                    var count = cmd.ExecuteNonQuery();
                    conn.Close(); // 关闭数据库连接
                    if(count > 0)
                    {
                        result = new { isSuccess = true, message = string.Empty };
                    }
                    else
                    {
                        result = new { isSuccess = false, message = "服务器异常，请重试！" };
                    }
                }
            }
            catch (Exception ex)
            {
                result = new { isSuccess = false, message = "处理异常:"+ex.Message };
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getcitoken()
        {
            var result = new testss { code = "1",message = "success",data=new Token { citoken = "1527928820-2346931945-5fc1382a20e950448328fa4891a585c3" } };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
    public class testss {

        public string code { get; set; }
        public string message { get; set; }
        public Token data { get; set; }
    }

    public class Token
    {
        public string citoken { get; set; }
    }
}
