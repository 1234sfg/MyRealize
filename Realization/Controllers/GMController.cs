using Realization.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Realization.Controllers
{
    public class GMController : Controller
    {

        /// <summary>
        /// 主页可以查看所有功能导航，点击直接判断有没有session，要求登录
        /// 功能-查/登录页  判断密码，判断账号
        /// </summary>
        public ActionResult Index()
        {
            //这里是查看商品
            SqlConnection conn = Dbconn.getConnn();
            SqlCommand sqlcommand = new SqlCommand();//(sql语句，连接对象和字符串)
            sqlcommand.Connection = conn;
            sqlcommand.CommandText = "select * from Products";
            try { 
            conn.Open();//打开数据库
            SqlDataReader sdr = sqlcommand.ExecuteReader();
            var products = new List<Products> { };
            while (sdr.Read())
            {
                products.Add(new Products
                {
                    ProductId = sdr.GetInt32(0),
                    ProductType = sdr.GetString(1),
                    ProductBrand = sdr.GetString(2),
                    ProductName= sdr.GetString(3),
                    ProductPriceIn = sdr.GetDouble(4),
                    ProductPriceOut = sdr.GetDouble(5),
                    ProductModel = sdr.GetString(6),
                    ProductDetail = sdr.GetString(7),
                    ProductPath = sdr.GetString(8)
            });
            }
            ViewBag.ProductList = Newtonsoft.Json.JsonConvert.SerializeObject(products);
            }
            catch (Exception ee)
            {
                Response.Write(ee);
                return View();
            }
            finally
            {
                sqlcommand = null;
                conn.Close();//关闭数据库
                conn = null;
            }
            return View();
        }
        /// <summary>
        /// 登录
        /// 先进去，直接登录超级管理员。
        /// 然后可以添加用户，给他权限（用下拉列表框）
        /// 仓库管理员：可以进仓库管理页面
        /// </summary>
        public ActionResult GMLogin() {
            return View();
        }
        /// <summary>
        /// 查看所有管理员。账号和权限。可以删除管理员
        /// //功能-注销用户，删除页
        /// </summary>
        public ActionResult GMList()
        {
            return View();
        }
        /// <summary>
        /// 功能-修改密码页
        /// </summary>
        public ActionResult ReSetPwd()
        {
            return View();
        }
        /// <summary>
        /// 如果权限是？？
        /// 可以查看
        /// 可以修改，删除
        /// 如果权限是？？
        /// 只可以查看商品
        /// </summary>
        public ActionResult ProductList()
        {
            return View();
        }
        /// <summary>
        /// 录入商品(录入商品的同时，会添加到库存中，数量默认为0，不给填，在数据库中默认0)
        /// 判断是否有这个商品型号，可以型号不同，别的都相同
        /// </summary>
        /// <returns></returns>
        public ActionResult AddProduct()
        {
            return View();
        }
        /// <summary>
        /// 权限是？？
        /// 可以查看库存的数量和进货，一键添加，在页面上设定
        /// 不需要插入新，通过录入商品自动插入。
        /// </summary>
        public ActionResult StockList()
        {
            return View();
        }
        /// <summary>
        /// 查看所有订单。已下单的改红字，派送中的黄字。已取消的绿色字
        /// 功能-查看订单页  
        /// 可以选择派送，点击后，商品出库，修改商品数量。根据订单上的商品？？和商品数量。
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderList() {
            return View();
        }
        //附加功能，商品销售按数量排序（通过订单中的商品数量总和）
        //退货原因
        //查看退货原因页   可以选择处理，完成处理给状态0改成1已处理
        public ActionResult ReasonList() {
            return View();
        }
        /// <summary>
        /// 注销
        /// </summary>
        public ActionResult GMLogout()
        {
            SessionSG.RemoveSession("GMUsers");
            return RedirectToAction("Index");
        }
    }
}