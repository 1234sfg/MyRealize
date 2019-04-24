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
        //注意所有的删除和修改，需要先查！！！
        /// <summary>
        /// 主页---已完成
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 登录---已完成
        /// </summary>
        public ActionResult GMLogin(string GMLogin)
        {
            string TempPwd;
            string TempName;
            string TempPower;
            if (GMLogin == "登录")
            {
                string GMName = Request["GMName"];
                string GMPassword = Request["GMPassword"];
                //登录时加密
                GMPassword = Models.Utils.HashUtil.GetSha256FromString(GMPassword);
                SqlConnection conn = Dbconn.getConnn();
                //查询数据库有没有这个name
                SqlCommand SqlGetName = new SqlCommand();
                SqlGetName.Connection = conn;
                SqlGetName.CommandText = @"select name,password,power from Managers where name=@name";
                SqlGetName.Parameters.AddWithValue("@name", GMName);
                try
                {
                    conn.Open();//打开数据库 
                    SqlDataReader sdrPhone = SqlGetName.ExecuteReader();
                    bool flagPhone = sdrPhone.Read();   //有这个用户名
                    if (flagPhone == true)
                    {

                        TempName = sdrPhone.GetString(0);
                        TempPwd = sdrPhone.GetString(1);
                        TempPower = sdrPhone.GetString(2);
                        sdrPhone.Close();
                        if (GMPassword.Equals(TempPwd))//成功登录
                        {
                            SessionSG.SetSession("GMUsers", TempName);//设置session对象Users的值 为取到的用户名
                            SessionSG.SetSession("GMPower", TempPower);
                            if (TempPower == "超级管理员") { Response.Write("<script>window.location.href='../GM/Index';</script>"); }
                            else if (TempPower == "商品管理员") { Response.Write("<script>window.location.href='../GM/ProductList';</script>"); }
                            else if (TempPower == "仓库管理员") { Response.Write("<script>window.location.href='../GM/StockList';</script>"); }
                            else { Response.Write("<script>alert('你没有权限进行操作');</script>"); }
                        }
                        else
                        {
                            Response.Write("<script>alert('密码错误')</script>");
                            return View();
                        }
                    }
                    else
                    {
                        Response.Write("<script>alert('你输入的用户名不存在')</script>");
                        return View();
                    }
                }
                catch (Exception ee)
                {
                    Response.Write(ee.Message);
                    return View();
                }
                finally
                {
                    SqlGetName = null;
                    conn.Close();
                    conn = null;
                }
            }
            return View();
        }
        /// <summary> 
        /// 管理员信息---已完成
        /// </summary>
        public ActionResult GMList(string AddGMBtn, string GMManagersCURD)
        {
            string RequestPower = "超级管理员";
            try
            {
                //一般就在这块儿报错了
                //先要求有超级管理员权限，才能进行查看，否则显示权限不足，退回登录页面
                string GMUsersName = SessionSG.GetSession("GMUsers").ToString();
                string GMPowerName = SessionSG.GetSession("GMPower").ToString();
                SqlConnection conn = Dbconn.getConnn();
                if (GMPowerName.Equals(RequestPower))
                {
                    //添加---已完成
                    if (AddGMBtn == "添加")
                    {
                        string GMName = Request["GMName"];
                        string GMPassword = Request["GMPassword"];
                        string GMRePassword = Request["GMRePassword"];
                        string GMPower = Request["GMPower"];
                        string GMPhone = Request["GMPhone"];
                        if (GMPassword == GMRePassword)
                        {
                            GMPassword = Models.Utils.HashUtil.GetSha256FromString(GMPassword);
                            //添加管理员
                            SqlCommand AddGM = new SqlCommand();
                            AddGM.Connection = conn;
                            AddGM.CommandText = @"insert into Managers(name,password,power,phone)values(@name,@password,@power,@phone)";
                            AddGM.Parameters.AddWithValue("@name", GMName);
                            AddGM.Parameters.AddWithValue("@password", GMPassword);
                            AddGM.Parameters.AddWithValue("@power", GMPower);
                            AddGM.Parameters.AddWithValue("@phone", GMPhone);
                            //判断用户名是否存在
                            SqlCommand IfName = new SqlCommand();
                            IfName.Connection = conn;
                            IfName.CommandText = @"select * from Managers where name=@name";
                            IfName.Parameters.AddWithValue("@name", GMName);
                            try
                            {
                                conn.Open();//打开数据库
                                SqlDataReader NameReader = IfName.ExecuteReader();
                                bool ExistName = NameReader.Read();//上下三行，判断有没有读到这个用户名
                                NameReader.Close();
                                if (ExistName == false)
                                {

                                    AddGM.ExecuteNonQuery();
                                    Response.Write("<script>alert('添加成功！');window.location.href='../GM/GMList';</script>");

                                }
                                else
                                {
                                    Response.Write("<script>alert('用户名已存在');window.location.href='../GM/GMList';</script>");
                                }

                            }
                            catch (Exception ee) { Response.Write(ee.Message); }
                            finally
                            {
                                AddGM = null;
                                IfName = null;
                                conn.Close();
                                conn = null;
                            }
                        }
                    }
                    //删除---已完成
                    if (GMManagersCURD == "删除")
                    {
                        int GMId = int.Parse(Request["GMId"]);
                        SqlCommand DelGM = new SqlCommand();
                        DelGM.Connection = conn;
                        DelGM.CommandText = @"delete from Managers where id=@id";
                        DelGM.Parameters.AddWithValue("@id", GMId);
                        //判断数据是否存在
                        SqlCommand IfId = new SqlCommand();
                        IfId.Connection = conn;
                        IfId.CommandText = @"select * from Managers where id=@id";
                        IfId.Parameters.AddWithValue("@id", GMId);
                        try
                        {
                            conn.Open();//打开数据库
                            SqlDataReader IdReader = IfId.ExecuteReader();
                            bool ExistId = IdReader.Read();//上下三行，判断有没有读到这个用户名
                            IdReader.Close();
                            if (ExistId == false)
                            {
                                Response.Write("<script>alert('不存在这条记录');window.location.href='../GM/GMList';</script>");
                            }
                            else
                            {
                                DelGM.ExecuteNonQuery();
                                Response.Write("<script>alert('删除成功');window.location.href='../GM/GMList';</script>");
                            }
                        }
                        catch (Exception ee)
                        {
                            Response.Write(ee.Message);
                        }
                        finally
                        {
                            DelGM = null;
                            IfId = null;
                            conn.Close();
                            conn = null;
                        }
                    }
                    //修改、不要了 ，有改密码这个东西了
                    //if (GMManagersCURD == "修改") { Response.Write("<script>alert('修改');</script>"); }
                    SqlCommand GMInfo = new SqlCommand();//(sql语句，连接对象和字符串)
                    GMInfo.Connection = conn;
                    GMInfo.CommandText = "select * from Managers";
                    try
                    {
                        conn.Open();//打开数据库
                                    //读取基本信息
                        SqlDataReader GMInfoSdr = GMInfo.ExecuteReader();
                        var ManagersItem = new List<Managers> { };
                        while (GMInfoSdr.Read())
                        {
                            ManagersItem.Add(new Managers
                            {
                                //用户id和用户密码无法被查看
                                Id = GMInfoSdr.GetInt32(0),
                                Name = GMInfoSdr.GetString(1),
                                //Password = GMInfoSdr.GetString(2),
                                Power = GMInfoSdr.GetString(3),
                                Phone = GMInfoSdr.GetString(4)
                            });
                        }
                        GMInfoSdr.Close();
                        ViewBag.ManagersItem = Newtonsoft.Json.JsonConvert.SerializeObject(ManagersItem);
                    }
                    catch (Exception ee)
                    {
                        Response.Write(ee.Message);
                        return View();
                    }
                    finally
                    {
                        GMInfo = null;
                        conn.Close();//关闭数据库
                        conn = null;
                    }
                }
                else
                {
                    SessionSG.RemoveSession("GMUsers");
                    SessionSG.RemoveSession("GMPower");
                    Response.Write("<script>alert('权限不足，要求登录超级管理员');window.location.href='../GM/GMLogin'</script>");
                }
            }
            catch (Exception ee)
            {
                Response.Write("<script>alert('亲，请登录后再进行操作');window.location.href='../GM/GMLogin';</script>");
                Response.Write(ee.Message);
            }
            return View();
        }
        /// <summary>
        /// 管理改密码---已完成
        /// </summary>
        public ActionResult AlertGMPwd(string AlertPwd)
        {
            try
            {
                string GMUsers = SessionSG.GetSession("GMUsers").ToString();//一般就在这块儿报错了、
                SqlConnection conn = Dbconn.getConnn();

                if (AlertPwd == "确认修改")
                {
                    string NewPwd = Request["NewPwd"];
                    string ConfirmPwd = Request["ConfirmPwd"];
                    if (NewPwd.Equals(ConfirmPwd))
                    {
                        NewPwd = Models.Utils.HashUtil.GetSha256FromString(NewPwd);
                        SqlCommand SetNewPwd = new SqlCommand();
                        SetNewPwd.Connection = conn;
                        SetNewPwd.CommandText = @"update Managers set Password=@NewPwd where Name=@GMUsers";
                        SetNewPwd.Parameters.AddWithValue("@NewPwd", NewPwd);
                        SetNewPwd.Parameters.AddWithValue("@GMUsers", GMUsers);//加参数的方法,用的是session中的UserName
                        try
                        {
                            conn.Open();
                            SetNewPwd.ExecuteNonQuery();
                            SessionSG.RemoveSession("GMUsers");
                            SessionSG.RemoveSession("GMPower");
                            Response.Write("<script>alert('修改密码成功，请重新登录');window.location.href='../GM/GMLogin'</script>");
                        }
                        catch (Exception ee)
                        {
                            Response.Write(ee.Message);
                        }
                        finally
                        {
                            SetNewPwd = null;
                            conn.Close();
                            conn = null;

                        }
                    }
                    else { Response.Write("<script>alert(两次输入的密码不一致，请重新输入)</script>"); }
                }
            }
            catch (Exception ee)
            {
                Response.Write("<script>alert('亲，请登录后再进行操作');window.location.href='../GM/GMLogin'</script>");
                Response.Write(ee.Message);
            }
            return View();
        }
        /// <summary>
        /// 商品列表---已完成
        /// </summary> 
        public ActionResult ProductList(string ProductCRUD)
        {
            try
            {
                //这里是查看商品 
                SqlConnection conn = Dbconn.getConnn();
                string GMUsers = SessionSG.GetSession("GMUsers").ToString();
                string GMPower = SessionSG.GetSession("GMPower").ToString();
                string tempSuperGM = "超级管理员";
                string tempProGM = "商品管理员";
                if (GMPower== tempSuperGM|| GMPower== tempProGM) {
                    //修改商品---已完成
                    if (ProductCRUD == "修改") {
                        int ProductId = int.Parse(Request["ProductId"]);
                        string ProductType = Request["ProductType"];
                        string ProductBrand = Request["ProductBrand"];
                        string ProductName = Request["ProductName"];
                        string ProductPriceIn = Request["ProductPriceIn"];
                        string ProductPriceOut = Request["ProductPriceOut"];
                        string ProductModel = Request["ProductModel"];
                        string ProductDetail = Request["ProductDetail"];
                        string ProductPath = Request["ProductPath"];
                        //修改
                        SqlCommand AlertProduct = new SqlCommand();//(sql语句，连接对象和字符串)
                        AlertProduct.Connection = conn;
                        AlertProduct.CommandText = "Update  Products set ProductType=@ProductType," +
                            "ProductBrand=@ProductBrand," +
                            "ProductName=@ProductName," +
                            "ProductPriceIn=@ProductPriceIn," +
                            "ProductPriceOut=@ProductPriceOut," +
                            "ProductModel=@ProductModel," +
                            "ProductDetail=@ProductDetail," +
                            "ProductPath=@ProductPath " +
                            "where  ProductId=@ProductId";
                        AlertProduct.Parameters.AddWithValue("@ProductId", ProductId);
                        AlertProduct.Parameters.AddWithValue("@ProductType", ProductType);
                        AlertProduct.Parameters.AddWithValue("@ProductBrand", ProductBrand);
                        AlertProduct.Parameters.AddWithValue("@ProductName", ProductName);
                        AlertProduct.Parameters.AddWithValue("@ProductPriceIn", ProductPriceIn);
                        AlertProduct.Parameters.AddWithValue("@ProductPriceOut", ProductPriceOut);
                        AlertProduct.Parameters.AddWithValue("@ProductModel", ProductModel);
                        AlertProduct.Parameters.AddWithValue("@ProductDetail", ProductDetail);
                        AlertProduct.Parameters.AddWithValue("@ProductPath", ProductPath);
                        //查型号,先查到原来的，再查一下现在的。
                        SqlCommand GetModelBefore = new SqlCommand();
                        GetModelBefore.Connection = conn;
                        GetModelBefore.CommandText = "select ProductModel from Products where ProductId=@ProductId";
                        GetModelBefore.Parameters.AddWithValue("@ProductId", ProductId);
                        SqlCommand GetModelNow = new SqlCommand();
                        GetModelNow.Connection = conn;
                        GetModelNow.CommandText = "select * from Products where ProductModel=@ProductModel";
                        GetModelNow.Parameters.AddWithValue("@ProductModel", ProductModel);
                        //查到如果原来和现在是一样的、执行，查到原来和现在不一样，并且查到现在的不存在数据库中，也执行
                        try
                        {
                            conn.Open();//打开数据库 
                            SqlDataReader sdrModelBefore = GetModelBefore.ExecuteReader();
                            sdrModelBefore.Read();
                            string ModelBefore = sdrModelBefore.GetString(0);
                            sdrModelBefore.Close();
                            SqlDataReader sdrModelNow = GetModelNow.ExecuteReader();
                            bool Now = sdrModelNow.Read();
                            sdrModelNow.Close();
                            if (ProductModel== ModelBefore)
                            {
                                AlertProduct.ExecuteNonQuery();
                                Response.Write("<script>alert('修改成功');" +
                                        "window.location.href='../GM/ProductList'</script>");
                            }
                            else
                            {
                                if (Now == false)
                                {
                                    AlertProduct.ExecuteNonQuery();
                                    Response.Write("<script>alert('修改成功');" +
                                        "window.location.href='../GM/ProductList'</script>");
                                }
                                else if (Now == true)
                                {
                                    Response.Write("<script>alert('该型号的商品已存在');" +
                                        "window.location.href='../GM/ProductList'</script>");
                                }
                            }
                        }
                        catch (Exception ee)
                        {
                            Response.Write(ee);
                            return View();
                        }
                        finally
                        {
                            AlertProduct = null;
                            conn.Close();//关闭数据库
                            conn = null;
                        }
                    }
                    //删除商品---已完成
                    if (ProductCRUD == "删除") {
                        //先看看有没有这个数据，再删除
                        int ProductId = int.Parse(Request["ProductId"]);
                        SqlCommand DelProduct = new SqlCommand();
                        DelProduct.Connection = conn;
                        DelProduct.CommandText = @"delete from Products where ProductId=@ProductId";
                        DelProduct.Parameters.AddWithValue("@ProductId", ProductId);
                        //判断数据是否存在
                        SqlCommand IfProductId = new SqlCommand();
                        IfProductId.Connection = conn;
                        IfProductId.CommandText = @"select * from Products where ProductId=@ProductId";
                        IfProductId.Parameters.AddWithValue("@ProductId", ProductId);
                        try
                        {
                            conn.Open();//打开数据库
                            SqlDataReader IdReader = IfProductId.ExecuteReader();
                            bool ExistId = IdReader.Read();//上下三行，判断有没有读到这个用户名
                            IdReader.Close();
                            if (ExistId == false)
                            {
                                Response.Write("<script>alert('不存在这条记录');window.location.href='../GM/ProductList';</script>");
                            }
                            else
                            {
                                DelProduct.ExecuteNonQuery();
                                Response.Write("<script>alert('删除成功');window.location.href='../GM/ProductList';</script>");
                            }
                        }
                        catch (Exception ee)
                        {
                            Response.Write(ee.Message);
                        }
                        finally
                        {
                            DelProduct = null;
                            IfProductId = null;
                            conn.Close();
                            conn = null;
                        }
                    }
                    //添加商品---已完成
                    if (ProductCRUD == "添加") {
                        string ProductType = Request["ProductType"];
                        string ProductBrand = Request["ProductBrand"];
                        string ProductName = Request["ProductName"];
                        float ProductPriceIn = float.Parse(Request["ProductPriceIn"]);
                        float ProductPriceOut = float.Parse(Request["ProductPriceOut"]);
                        string ProductModel = Request["ProductModel"];
                        string ProductDetail = Request["ProductDetail"];
                        string ProductPath = Request["ProductPath"];
                        //Response.Write("<script>alert("+ ProductPriceIn+");</script>");
                        //添加商品，先要判断有没有这个型号
                        SqlCommand AddProduct = new SqlCommand();
                        AddProduct.Connection = conn;
                        AddProduct.CommandText = @"insert into Products(ProductType,ProductBrand,ProductName,ProductPriceIn,ProductPriceOut,ProductModel,ProductDetail,ProductPath)values(@ProductType,@ProductBrand,@ProductName,@ProductPriceIn,@ProductPriceOut,@ProductModel,@ProductDetail,@ProductPath)";
                        AddProduct.Parameters.AddWithValue("@ProductType", ProductType);
                        AddProduct.Parameters.AddWithValue("@ProductBrand", ProductBrand);
                        AddProduct.Parameters.AddWithValue("@ProductName", ProductName);
                        AddProduct.Parameters.AddWithValue("@ProductPriceIn", ProductPriceIn);
                        AddProduct.Parameters.AddWithValue("@ProductPriceOut", ProductPriceOut);
                        AddProduct.Parameters.AddWithValue("@ProductModel", ProductModel);
                        AddProduct.Parameters.AddWithValue("@ProductDetail", ProductDetail);
                        AddProduct.Parameters.AddWithValue("@ProductPath", ProductPath);
                        //判断型号是否存在
                        SqlCommand IfProductModel = new SqlCommand();
                        IfProductModel.Connection = conn;
                        IfProductModel.CommandText = @"select * from Products where ProductModel=@ProductModel";
                        IfProductModel.Parameters.AddWithValue("@ProductModel", ProductModel);
                        try
                        {
                            conn.Open();//打开数据库
                            SqlDataReader NameReader = IfProductModel.ExecuteReader();
                            bool ExistModel = NameReader.Read();//上下三行，判断有没有读到这个用户名
                            NameReader.Close();
                            if (ExistModel == false)
                            {
                                AddProduct.ExecuteNonQuery();
                                Response.Write("<script>alert('添加成功！');window.location.href='../GM/ProductList';</script>");
                            }
                            else
                            {
                                Response.Write("<script>alert('该型号的商品已存在');window.location.href='../GM/ProductList';</script>");
                            }

                        }
                        catch (Exception ee) { Response.Write(ee.Message);}
                        finally
                        {
                            AddProduct = null;
                            IfProductModel = null;
                            conn.Close();
                            conn = null;
                        }
                    }
                    //遍历
                    SqlCommand selPro = new SqlCommand();//(sql语句，连接对象和字符串)
                    selPro.Connection = conn;
                    selPro.CommandText = "select * from Products";
                    try
                    {
                        conn.Open();//打开数据库
                        SqlDataReader sdr = selPro.ExecuteReader();
                        var products = new List<Products> { };
                        while (sdr.Read())
                        {
                            products.Add(new Products
                            {
                                ProductId = sdr.GetInt32(0),
                                ProductType = sdr.GetString(1),
                                ProductBrand = sdr.GetString(2),
                                ProductName = sdr.GetString(3),
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
                        selPro = null;
                        conn.Close();//关闭数据库
                        conn = null;
                    }
                }
                else {
                    Response.Write("<script>alert('亲,请使用合适的管理员账号登录');window.location.href='../GM/GMLogin'</script>");
                }

            }
            catch (Exception ee)
            {
                Response.Write("<script>alert('亲,请登录');window.location.href='../GM/GMLogin'</script>");
                Response.Write(ee.Message);
            }
            return View();
        }
        /// <summary>
        /// 注销---已完成
        /// </summary>
        public ActionResult GMLogout()
        {
            SessionSG.RemoveSession("GMUsers");
            SessionSG.RemoveSession("GMPower");
            return RedirectToAction("Index");
        }
        
        
        /// <summary>
        /// 如果权限是仓库管理员，或者超级管理员！！！！
        /// 只可以查看商品！！！！
        /// 录入和添加到仓库。如果没在仓库中查到这个数据，则插入，并且添加数量为0！！！！
        /// 权限是仓库管理员，或者超级管理员！！！！
        /// 可以查看库存的数量
        /// 可以新商品入库（下拉选择）、、存在商品但是不在仓库的 ！！！
        /// 可以现有商品入库（下拉选择）、、存在仓库的！！！！！！
        /// </summary>
        public ActionResult StockList()
        {
            try
            {
                //这里是查看商品 
                SqlConnection conn = Dbconn.getConnn();
                string GMUsers = SessionSG.GetSession("GMUsers").ToString();
                string GMPower = SessionSG.GetSession("GMPower").ToString();
                string tempSuperGM = "超级管理员";
                string tempStoGM = "仓库管理员";
            }
            catch (Exception ee)
            {
                Response.Write("<script>alert('亲,请登录');window.location.href='../GM/GMLogin'</script>");
                Response.Write(ee.Message);
            }
            return View();
        }
        /// <summary>
        /// 查看所有订单。已下单的改红字，派送中的黄字。已取消的绿色字。！！！！！！！！！！！
        /// 功能-查看订单页  ！！！！！！！！！！！！！！！！！！！！！
        /// 可以选择派送，点击后，商品出库，修改商品数量。根据订单上的商品？？和商品数量！！！！！
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderList()
        {
            return View();
        }
        //附加功能，商品销售按数量排序（通过订单中的商品数量总和）！！！！！！！！！！！
        //退货原因！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
        //查看退货原因页   可以选择处理，完成处理给状态0改成1已处理！！！！！！！！！
        public ActionResult ReasonList()
        {
            return View();
        }
        /// <summary>
        /// 销售量--看现有和删除的订单
        /// </summary>
        public ActionResult SoldList()
        {
            return View();
        }
        /// <summary>
        /// 客户
        /// 客户留言
        /// </summary>
        public ActionResult UserMessages()
        {
            return View();
        }
        /// <summary>
        /// 退货原因
        /// </summary>
        public ActionResult UserReasons()
        {
            return View();
        }
    }
}