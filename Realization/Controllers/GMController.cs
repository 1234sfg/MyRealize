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
                    GMInfo.CommandText = @"select * from Managers";
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
                    else { Response.Write("<script>alert('两次输入的密码不一致，请重新输入')</script>"); }
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
                    if (ProductCRUD == "修改")
                    {
                        int ProductId = int.Parse(Request["ProductId"]);
                        SessionSG.SetSession("AlertId", ProductId);
                        Response.Write("<script>window.location.href='../GM/AlertProduct'</script>");
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
                        string ProviderName = Request["ProviderName"]; 

                        //添加商品，先要判断有没有这个型号
                        SqlCommand AddProduct = new SqlCommand();
                        AddProduct.Connection = conn;
                        AddProduct.CommandText = @"insert into Products(ProductType,ProductBrand,ProductName,ProductPriceIn,ProductPriceOut,ProductModel,ProductDetail,ProductPath,ProviderName)values(@ProductType,@ProductBrand,@ProductName,@ProductPriceIn,@ProductPriceOut,@ProductModel,@ProductDetail,@ProductPath,@ProviderName)";
                        AddProduct.Parameters.AddWithValue("@ProductType", ProductType);
                        AddProduct.Parameters.AddWithValue("@ProductBrand", ProductBrand);
                        AddProduct.Parameters.AddWithValue("@ProductName", ProductName);
                        AddProduct.Parameters.AddWithValue("@ProductPriceIn", ProductPriceIn);
                        AddProduct.Parameters.AddWithValue("@ProductPriceOut", ProductPriceOut);
                        AddProduct.Parameters.AddWithValue("@ProductModel", ProductModel);
                        AddProduct.Parameters.AddWithValue("@ProductDetail", ProductDetail);
                        AddProduct.Parameters.AddWithValue("@ProductPath", ProductPath);
                        AddProduct.Parameters.AddWithValue("@ProviderName", ProviderName);

                        //判断型号是否存在
                        SqlCommand IfProductModel = new SqlCommand();
                        IfProductModel.Connection = conn;
                        IfProductModel.CommandText = @"select * from Products where ProductModel=@ProductModel";
                        IfProductModel.Parameters.AddWithValue("@ProductModel", ProductModel);
                        try
                        {
                            conn.Open();//打开数据库
                            SqlDataReader ModelReader = IfProductModel.ExecuteReader();
                            bool ExistModel = ModelReader.Read();//上下三行，判断有没有读到这型号
                            ModelReader.Close(); 
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
                    selPro.CommandText = @"select * from Products";
                    SqlCommand selProv = new SqlCommand();//(sql语句，连接对象和字符串)
                    selProv.Connection = conn;
                    selProv.CommandText = @"select * from Providers";  
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
                                ProductPath = sdr.GetString(8),
                                ProviderName = sdr.GetString(9), 
                            });
                        }
                        sdr.Close();
                        ViewBag.ProductList = Newtonsoft.Json.JsonConvert.SerializeObject(products); 
                        SqlDataReader Providersdr = selProv.ExecuteReader();
                        var providers = new List<Providers> { };
                        while (Providersdr.Read())
                        {
                            providers.Add(new Providers
                            {
                                ProviderId = Providersdr.GetInt32(0),
                                ProviderName = Providersdr.GetString(1),
                                ProviderPhone = Providersdr.GetString(2),
                                ProviderAddress = Providersdr.GetString(3),
                            });
                        }
                        Providersdr.Close();
                        ViewBag.ProvidersList = Newtonsoft.Json.JsonConvert.SerializeObject(providers);
                    }
                    catch (Exception ee)
                    {
                        Response.Write(ee);
                    }
                    finally
                    {
                        selProv = null;
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
        /// 商品修改---已完成
        /// </summary>
        public ActionResult AlertProduct(string ProductCRUD)
        {
            try
            {
                //这里是查看商品 
                SqlConnection conn = Dbconn.getConnn();
                string GMUsers = SessionSG.GetSession("GMUsers").ToString();
                string GMPower = SessionSG.GetSession("GMPower").ToString();
                try {
                    int GetProductId = int.Parse(SessionSG.GetSession("AlertId").ToString());
                    string tempSuperGM = "超级管理员";
                    string tempProGM = "商品管理员";
                    if (GMPower == tempSuperGM || GMPower == tempProGM)
                    {
                        //修改商品---已完成
                        if (ProductCRUD == "修改")
                        {
                            int ProductId = int.Parse(Request["ProductId"]);
                            string ProductType = Request["ProductType"];
                            string ProductBrand = Request["ProductBrand"];
                            string ProductName = Request["ProductName"];
                            string ProductPriceIn = Request["ProductPriceIn"];
                            string ProductPriceOut = Request["ProductPriceOut"];
                            string ProductModel = Request["ProductModel"];
                            string ProductDetail = Request["ProductDetail"];
                            string ProductPath = Request["ProductPath"];
                            string ProviderName = Request["ProviderName"];
                            //修改
                            SqlCommand AlertProduct = new SqlCommand();//(sql语句，连接对象和字符串)
                            AlertProduct.Connection = conn;
                            AlertProduct.CommandText = @"Update  Products set " +
                                "ProductType=@ProductType," +
                                "ProductBrand=@ProductBrand," +
                                "ProductName=@ProductName," +
                                "ProductPriceIn=@ProductPriceIn," +
                                "ProductPriceOut=@ProductPriceOut," +
                                "ProductModel=@ProductModel," +
                                "ProductDetail=@ProductDetail," +
                                "ProductPath=@ProductPath," +
                                "ProviderName=@ProviderName " +
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
                            AlertProduct.Parameters.AddWithValue("@ProviderName", ProviderName);
                            //查型号,先查到原来的，再查一下现在的。
                            SqlCommand GetModelBefore = new SqlCommand();
                            GetModelBefore.Connection = conn;
                            GetModelBefore.CommandText = @"select ProductModel from Products where ProductId=@ProductId";
                            GetModelBefore.Parameters.AddWithValue("@ProductId", ProductId);
                            SqlCommand GetModelNow = new SqlCommand();
                            GetModelNow.Connection = conn;
                            GetModelNow.CommandText = @"select * from Products where ProductModel=@ProductModel";
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
                                if (ProductModel == ModelBefore)
                                {
                                    AlertProduct.ExecuteNonQuery();
                                    SessionSG.RemoveSession("AlertId");
                                    Response.Write("<script>alert('修改成功');" +
                                        "window.location.href='../GM/ProductList'</script>");
                                }
                                else
                                {
                                    if (Now == false)
                                    {
                                        AlertProduct.ExecuteNonQuery();
                                        SessionSG.RemoveSession("AlertId");
                                        Response.Write("<script>alert('修改成功');" +
                                        "window.location.href='../GM/ProductList'</script>");
                                    }
                                    else if (Now == true)
                                    {
                                        SessionSG.RemoveSession("AlertId");
                                        Response.Write("<script>alert('该型号的商品已存在');" +
                                        "window.location.href='../GM/ProductList'</script>");
                                    }
                                }
                            }
                            catch (Exception ee)
                            {
                                Response.Write(ee.Message);
                                return View();
                            }
                            finally
                            {
                                AlertProduct = null;
                                conn.Close();//关闭数据库
                                conn = null;
                            }
                        }
                        SqlCommand selPro = new SqlCommand();//(sql语句，连接对象和字符串)
                        selPro.Connection = conn;
                        selPro.CommandText = @"select * from Products where ProductId=@ProductId";
                        selPro.Parameters.AddWithValue("@ProductId", GetProductId);
                        SqlCommand selProv = new SqlCommand();//(sql语句，连接对象和字符串)
                        selProv.Connection = conn;
                        selProv.CommandText = @"select * from Providers";
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
                                    ProductPath = sdr.GetString(8),
                                    ProviderName = sdr.GetString(9),
                                });
                            }
                            sdr.Close();
                            ViewBag.ProductList = Newtonsoft.Json.JsonConvert.SerializeObject(products);
                            SqlDataReader Providersdr = selProv.ExecuteReader();
                            var providers = new List<Providers> { };
                            while (Providersdr.Read())
                            {
                                providers.Add(new Providers
                                {
                                    ProviderId = Providersdr.GetInt32(0),
                                    ProviderName = Providersdr.GetString(1),
                                    ProviderPhone = Providersdr.GetString(2),
                                    ProviderAddress = Providersdr.GetString(3),
                                });
                            }
                            Providersdr.Close();
                            ViewBag.ProvidersList = Newtonsoft.Json.JsonConvert.SerializeObject(providers);

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
                    else
                    {
                        Response.Write("<script>alert('亲,请使用合适的管理员账号登录');window.location.href='../GM/GMLogin'</script>");
                    }
                }catch(Exception ee) { Response.Write(ee.Message);
                    Response.Write("<script>alert('无效修改');window.location.href='../GM/ProductList'</script>");
                }
            }
            catch (Exception ee)
            {
                Response.Write(ee.Message);
                Response.Write("<script>alert('亲,请登录');window.location.href='../GM/GMLogin'</script>");
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
        /// 添加到仓库---已完成 
        /// </summary>
        public ActionResult StockList(string StockCRUD)
        {
            try
            {
                //这里是查看库存 
                SqlConnection conn = Dbconn.getConnn();
                string GMUsers = SessionSG.GetSession("GMUsers").ToString();
                string GMPower = SessionSG.GetSession("GMPower").ToString();
                string tempSuperGM = "超级管理员";
                string tempStoGM = "仓库管理员";
                if (GMPower == tempSuperGM || GMPower == tempStoGM)
                {
                    //现有商品展示---已完成
                    SqlCommand selStock = new SqlCommand();
                    selStock.Connection = conn;
                    selStock.CommandText = @"select ProductId,ProductModel,ProductAmount from Stocks";
                    //新商品（不在仓库的）在页面上只用显示型号和ID就行了。
                    SqlCommand selProNotStock = new SqlCommand();
                    selProNotStock.Connection = conn;
                    selProNotStock.CommandText = @"select ProductId,ProductModel from Products where not exists" +
                        "(select ProductId from Stocks where Products.ProductId=Stocks.ProductId)"; 
                    //添加商品进仓库---已完成
                    if (StockCRUD == "上架")
                    {
                        int ProductId = int.Parse(Request["ProductId"]);
                        string ProductModel = Request["ProductModel"];
                        SqlCommand InsertStock = new SqlCommand();
                        InsertStock.Connection = conn;
                        InsertStock.CommandText = @"Insert into Stocks(ProductId,ProductModel)values(@ProductId,@ProductModel)";
                        InsertStock.Parameters.AddWithValue("@ProductId", ProductId);
                        InsertStock.Parameters.AddWithValue("@ProductModel", ProductModel);
                        try {
                            conn.Open();
                            InsertStock.ExecuteNonQuery();
                            Response.Write("<script>alert('上架成功！');window.location.href='../GM/StockList';</script>");
                        }
                        catch (Exception ee)
                        {
                            Response.Write(ee.Message);
                        }
                        finally
                        {
                            InsertStock = null;
                            conn.Close();
                            conn = null;
                        }
                    }
                    //商品下架（商品非空不能下架）获取一下ProductID和ProductAmount
                    if (StockCRUD == "进货")
                    { 
                        int ProductId=int.Parse(Request["ProductId"]);
                        int AlertAmount = int.Parse(Request["AlertAmount"]);
                        string DateNow = DateTime.Now.ToString();
                        SqlCommand AddAmount = new SqlCommand();
                        AddAmount.Connection = conn;
                        AddAmount.CommandText = @"update Stocks set ProductAmount+=@AlertAmount where ProductId=@ProductId";
                        AddAmount.Parameters.AddWithValue("@AlertAmount", AlertAmount);
                        AddAmount.Parameters.AddWithValue("@ProductId", ProductId);
                        SqlCommand AddStock = new SqlCommand();
                        AddStock.Connection = conn;
                        AddStock.CommandText = @"insert into AddStock (ProductId,Amount,Date)values(@ProductId,@Amount,@Date)";
                        AddStock.Parameters.AddWithValue("@ProductId", ProductId);
                        AddStock.Parameters.AddWithValue("@Amount", AlertAmount); 
                        AddStock.Parameters.AddWithValue("@Date", DateNow);
                        try { 
                        conn.Open();
                        AddStock.ExecuteNonQuery();
                        AddAmount.ExecuteNonQuery();
                        Response.Write("<script>alert('入库成功');window.location.href='../GM/StockList';</script>");
                        }
                        catch (Exception ee) {
                            Response.Write(ee.Message);
                        }
                        finally
                        {
                            AddStock = null;
                            AddAmount = null;
                            conn.Close();
                            conn = null;
                        }
                    }
                    if (StockCRUD == "下架")
                    {
                        int temp = 0;
                        int ProductId = int.Parse(Request["ProductId"]);
                        int ProductAmount = int.Parse(Request["ProductAmount"]);
                        if (temp.Equals(ProductAmount)) {
                            SqlCommand DelStock = new SqlCommand();
                            DelStock.Connection = conn;
                            DelStock.CommandText = @"delete from Stocks where ProductId=@ProductId";
                            DelStock.Parameters.AddWithValue("@ProductId", ProductId);
                            try {
                                conn.Open();
                                DelStock.ExecuteNonQuery();
                                Response.Write("<script>alert('下架成功');window.location.href='../GM/StockList';</script>");
                            } catch(Exception ee) { Response.Write(ee.Message); } finally {
                                DelStock = null;
                                conn.Close();
                                conn = null;
                            }
                        }
                        else
                        {
                            Response.Write("<script>alert('商品尚有库存，无法下架');window.location.href='../GM/StockList';</script>");
                        }
                    }
                    try {
                        conn.Open();//打开数据库
                        //现有商品
                        SqlDataReader sdrNow = selStock.ExecuteReader();
                        var stocks = new List<Stocks> { };
                        while (sdrNow.Read())
                        {
                            stocks.Add(new Stocks
                            {
                                ProductId = sdrNow.GetInt32(0),
                                ProductModel = sdrNow.GetString(1),
                                ProductAmount = sdrNow.GetInt32(2)
                            });
                        }
                        sdrNow.Close();
                        ViewBag.StocksList = Newtonsoft.Json.JsonConvert.SerializeObject(stocks);
                        //新商品
                        SqlDataReader sdrBefore = selProNotStock.ExecuteReader();
                        var ProNotStocks = new List<Products> { };
                        while (sdrBefore.Read())
                        {
                            ProNotStocks.Add(new Products
                            {
                                ProductId = sdrBefore.GetInt32(0),
                                ProductModel = sdrBefore.GetString(1)
                            });
                        }
                        sdrBefore.Close();
                        ViewBag.ProNotStocksList = Newtonsoft.Json.JsonConvert.SerializeObject(ProNotStocks);
                    }
                    catch (Exception ee)
                    {
                        Response.Write(ee.Message);
                    }
                    finally
                    {
                        selStock = null;
                        selProNotStock = null;
                        conn.Close();//关闭数据库
                        conn = null;
                    }
                }
                else
                {
                    Response.Write("<script>alert('亲,请使用合适的管理员账号登录');window.location.href='../GM/GMLogin'</script>");
                }
            }
            catch (Exception ee)
            {
                Response.Write("<script>alert('亲,请登录');window.location.href='../GM/GMLogin';</script>");
                Response.Write(ee.Message);
            }
            return View();
        }
        /// <summary>
        /// 供应商管理---已完成
        /// </summary>
        public ActionResult ProviderList(string ProviderCRUD)
        {
            try
            {
                //这里是查看商品 
                SqlConnection conn = Dbconn.getConnn();
                string GMUsers = SessionSG.GetSession("GMUsers").ToString();
                string GMPower = SessionSG.GetSession("GMPower").ToString();
                string tempSuperGM = "超级管理员";
                if (GMPower == tempSuperGM)
                {
                    if (ProviderCRUD == "添加")
                    {
                        string ProviderName = Request["ProviderName"];
                        string ProviderPhone = Request["ProviderPhone"];
                        string ProviderAddress = Request["ProviderAddress"];
                        //添加管理员
                        SqlCommand AddProvider = new SqlCommand();
                        AddProvider.Connection = conn;
                        AddProvider.CommandText = @"insert into Providers(ProviderName,ProviderPhone,ProviderAddress)values(@ProviderName,@ProviderPhone,@ProviderAddress)";
                        AddProvider.Parameters.AddWithValue("@ProviderName", ProviderName);
                        AddProvider.Parameters.AddWithValue("@ProviderPhone", ProviderPhone);
                        AddProvider.Parameters.AddWithValue("@ProviderAddress", ProviderAddress);
                        //判断供应商是否存在
                        SqlCommand IfName = new SqlCommand();
                        IfName.Connection = conn;
                        IfName.CommandText = @"select * from Providers where ProviderName=@ProviderName";
                        IfName.Parameters.AddWithValue("@ProviderName", ProviderName);
                        try
                        {
                            conn.Open();//打开数据库
                            SqlDataReader NameReader = IfName.ExecuteReader();
                            bool ExistName = NameReader.Read();//上下三行，判断有没有读到这个用户名
                            NameReader.Close();
                            if (ExistName == false)
                            {

                                AddProvider.ExecuteNonQuery();
                                Response.Write("<script>alert('添加成功！');window.location.href='../GM/ProviderList';</script>");

                            }
                            else
                            {
                                Response.Write("<script>alert('该供应商已存在');window.location.href='../GM/ProviderList';</script>");
                            }

                        }
                        catch (Exception ee) { Response.Write(ee.Message); }
                        finally
                        {
                            AddProvider = null;
                            IfName = null;
                            conn.Close();
                            conn = null;
                        }
                    }
                    if (ProviderCRUD == "删除")
                    {
                        int ProviderId = int.Parse(Request["ProviderId"]);
                        SqlCommand DelProvider = new SqlCommand();
                        DelProvider.Connection = conn;
                        DelProvider.CommandText = @"delete from Providers where ProviderId=@ProviderId";
                        DelProvider.Parameters.AddWithValue("@ProviderId", ProviderId);
                        try
                        {
                            conn.Open();//打开数据库
                            DelProvider.ExecuteNonQuery();
                            Response.Write("<script>alert('删除成功');window.location.href='../GM/ProviderList';</script>");
                        }
                        catch (Exception ee)
                        {
                            Response.Write(ee.Message);
                        }
                        finally
                        {
                            DelProvider = null;
                            conn.Close();
                            conn = null;
                        }
                    }
                    SqlCommand selProv = new SqlCommand();//(sql语句，连接对象和字符串)
                    selProv.Connection = conn;
                    selProv.CommandText = @"select * from Providers";
                    try
                    {
                        conn.Open();//打开数据库
                        SqlDataReader sdr = selProv.ExecuteReader();
                        var providers = new List<Providers> { };
                        while (sdr.Read())
                        {
                            providers.Add(new Providers
                            {
                                ProviderId = sdr.GetInt32(0),
                                ProviderName = sdr.GetString(1),
                                ProviderPhone = sdr.GetString(2),
                                ProviderAddress = sdr.GetString(3),
                            });
                        }
                        sdr.Close();
                        ViewBag.ProvidersList = Newtonsoft.Json.JsonConvert.SerializeObject(providers);
                    }
                    catch (Exception ee)
                    {
                        Response.Write(ee);
                    }
                    finally
                    {
                        selProv = null;
                        conn.Close();//关闭数据库
                        conn = null;
                    }
                }
                else
                {
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
        /// 查看所有订单、发货---已完成
        /// </summary> 
        public ActionResult OrderList(string OrderCURD)
        {
            try
            {
                //这里是查看订单 
                SqlConnection conn = Dbconn.getConnn();
                string GMUsers = SessionSG.GetSession("GMUsers").ToString();
                string GMPower = SessionSG.GetSession("GMPower").ToString();
                string tempSuperGM = "超级管理员"; 
                if (GMPower == tempSuperGM)
                { 
                    //全部订单
                    SqlCommand SelOrders = new SqlCommand();
                    SelOrders.Connection = conn;
                    SelOrders.CommandText = @"select * from Orders";
                    //发货
                    if (OrderCURD== "发货") {
                        int OrderId = int.Parse(Request["OrderId"]);
                        int ProductId = int.Parse(Request["ProductId"]);
                        int Quantity = int.Parse(Request["Quantity"]);
                        string OrderStatus = Request["OrderStatus"]; 
                        string NewStatus = "派送中";
                        //查询库存数量
                        SqlCommand SelStacks = new SqlCommand();
                        SelStacks.Connection = conn;
                        SelStacks.CommandText = @"select ProductAmount from Stocks where ProductId=@ProductId";
                        SelStacks.Parameters.AddWithValue("@ProductId", ProductId);
                        //修改订单状态
                        SqlCommand AlertOrders = new SqlCommand();
                        AlertOrders.Connection = conn;
                        AlertOrders.CommandText = @"update Orders set OrderStatus=@NewStatus where OrderId=@OrderId";
                        AlertOrders.Parameters.AddWithValue("@NewStatus", NewStatus);
                        AlertOrders.Parameters.AddWithValue("@OrderId", OrderId);
                        //修改库存数量
                        SqlCommand AlertStocks = new SqlCommand();
                        AlertStocks.Connection = conn;
                        AlertStocks.CommandText = @"update Stocks set ProductAmount=@ProductAmount where ProductId=@ProductId"; 
                        AlertStocks.Parameters.AddWithValue("@ProductId", ProductId);
                        try
                        {
                            conn.Open();//打开数据库
                            SqlDataReader sdrOrder = SelStacks.ExecuteReader();
                            sdrOrder.Read();
                            int StockQuantity = sdrOrder.GetInt32(0);
                            sdrOrder.Close();
                            
                            if (Quantity <= StockQuantity) {
                                StockQuantity = StockQuantity - Quantity;
                                AlertStocks.Parameters.AddWithValue("@ProductAmount", StockQuantity);
                                //执行
                                AlertOrders.ExecuteNonQuery();//修改
                                AlertStocks.ExecuteNonQuery();//修改 
                                Response.Write("<script>alert('出库成功！');window.location.href='../GM/OrderList';</script>");  
                            }
                            else {
                                Response.Write("<script>alert('库存不足，请及时进货后再发货');window.location.href='../GM/OrderList'</script>");
                            }
                        }
                        catch (Exception ee)
                        {
                            Response.Write(ee.Message);
                        }
                        finally
                        {
                            AlertOrders = null;
                            AlertStocks = null;
                            SelOrders = null;
                            conn.Close();
                            conn = null;
                        }    
                    } 
                    try
                    {
                        conn.Open();//打开数据库
                        SqlDataReader sdrOrderReader = SelOrders.ExecuteReader();
                        var OrderItems = new List<Orders> { };
                        while (sdrOrderReader.Read())
                        {
                            OrderItems.Add(new Orders
                            {
                                OrderId = sdrOrderReader.GetInt32(0),
                                UserName = sdrOrderReader.GetString(1),
                                ProductId = sdrOrderReader.GetInt32(2),
                                ProductName = sdrOrderReader.GetString(3),
                                ProductPrice = sdrOrderReader.GetDouble(4),
                                Quantity = sdrOrderReader.GetInt32(5),
                                AllPrice = sdrOrderReader.GetDouble(6),
                                OrderTime = sdrOrderReader.GetDateTime(7),
                                OrderStatus = sdrOrderReader.GetString(8),
                                ShopTo = sdrOrderReader.GetString(9),
                            });
                        }
                        sdrOrderReader.Close();
                        ViewBag.OrderItems = Newtonsoft.Json.JsonConvert.SerializeObject(OrderItems); 
                    }
                    catch (Exception ee)
                    {
                        Response.Write(ee.Message);
                    }
                    finally
                    {
                        SelOrders = null;
                        conn.Close();
                        conn = null;
                    }
                }
                else
                {
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
        /// 退货原因---已完成
        /// </summary>
        public ActionResult ReasonList( string ReasonCURD)
        {
            try
            {
                //这里是查看退货原因
                SqlConnection conn = Dbconn.getConnn();
                string GMUsers = SessionSG.GetSession("GMUsers").ToString();
                string GMPower = SessionSG.GetSession("GMPower").ToString();
                string tempSuperGM = "超级管理员";
                if (GMPower == tempSuperGM)
                {
                    //看所有的原因
                    SqlCommand SelReasons = new SqlCommand();
                    SelReasons.Connection = conn;
                    SelReasons.CommandText = @"select Dorders.ProductId,DOrders.ProductName,DOrders.Quantity,DOrders.UserName,Reasons.OrderId,Reasons.Reason,Reasons.Status from Reasons,DOrders WHERE DOrders.OrderId=Reasons.OrderId";
                    //点击已阅
                    if (ReasonCURD=="已阅") {
                        int OrderId = int.Parse(Request["OrderId"]);
                        string NewStatus = "已处理";
                        SqlCommand AlertReasonStatus = new SqlCommand();
                        AlertReasonStatus.Connection = conn;
                        AlertReasonStatus.CommandText = @"update Reasons set Status=@NewStatus where OrderId=@OrderId";
                        AlertReasonStatus.Parameters.AddWithValue("@NewStatus", NewStatus);
                        AlertReasonStatus.Parameters.AddWithValue("@OrderId", OrderId);
                        try
                        {
                            conn.Open();//打开数据库
                            AlertReasonStatus.ExecuteNonQuery();
                            Response.Write("<script>alert('操作成功');window.location.href='../GM/ReasonList';</script>");
                        }
                        catch (Exception ee)
                        {
                            Response.Write(ee.Message);
                        }
                        finally
                        {
                            AlertReasonStatus = null;
                            conn.Close();
                            conn = null;
                        }
                    }
                    try
                    {
                        conn.Open();//打开数据库
                        SqlDataReader sdrReasons = SelReasons.ExecuteReader();
                        var ReasonList = new List<ReasonListDOrder> { };
                        while (sdrReasons.Read())
                        {
                            ReasonList.Add(new ReasonListDOrder
                            { 
                                ProductId= sdrReasons.GetInt32(0),
                                ProductName = sdrReasons.GetString(1),
                                Quantity= sdrReasons.GetInt32(2),
                                UserName = sdrReasons.GetString(3),
                                OrderId = sdrReasons.GetInt32(4),
                                Reason = sdrReasons.GetString(5),
                                Status = sdrReasons.GetString(6)
                            });
                        }
                        sdrReasons.Close();
                        ViewBag.ReasonList = Newtonsoft.Json.JsonConvert.SerializeObject(ReasonList);
                    }
                    catch (Exception ee)
                    {
                        Response.Write(ee.Message);
                    }
                    finally
                    {
                        SelReasons = null;
                        conn.Close();
                        conn = null;
                    }
                }
                else
                {
                    Response.Write("<script>alert('亲,请使用合适的管理员账号登录');window.location.href='../GM/GMLogin'</script>");
                }
            }
            catch (Exception ee)
            {
                Response.Write(ee.Message);
                Response.Write("<script>alert('亲,请登录');window.location.href='../GM/GMLogin'</script>");
                
            }
            return View();
        }
        /// <summary>
        /// 现有订单,今年至今销售，去年销售，上个月销售---已完成
        /// </summary>
        public ActionResult SoldList()
        {
            try
            {
                //这里是查看订单 
                SqlConnection conn = Dbconn.getConnn();
                string GMUsers = SessionSG.GetSession("GMUsers").ToString();
                string GMPower = SessionSG.GetSession("GMPower").ToString();
                string tempSuperGM = "超级管理员";
                DateTime dt = DateTime.Now;
                if (GMPower == tempSuperGM)
                {
                    //查询完成的订单
                    SqlCommand SelOrders = new SqlCommand();
                    SelOrders.Connection = conn;
                    SelOrders.CommandText = @"select * from Orders";
                    string LastYearStart= dt.AddYears(-1).AddMonths(-dt.Month + 1).AddDays(-dt.Day + 1).ToString("yyyy-MM-dd");//去年第一天
                    string YearStart= dt.AddMonths(-dt.Month + 1).AddDays(-dt.Day + 1).ToString("yyyy-MM-dd");//本年第一天
                    string LastMonthStart= dt.AddMonths(-1).AddDays(-dt.Day + 1).ToString("yyyy-MM-dd"); //上月第一天
                    string MonthStart= dt.AddDays(-(dt.Day) + 1).ToString("yyyy-MM-dd"); //本月第一天
                    //统计一定时间内销售量
                    SqlCommand CountSoldTheYear = new SqlCommand();
                    CountSoldTheYear.Connection = conn;
                    CountSoldTheYear.CommandText = @"select SUM(Quantity) AS AllQuantity,ProductName from Orders where OrderTime >@DateStart and OrderTime<@DateEnd GROUP BY ProductName ORDER BY AllQuantity DESC ";
                    SqlCommand CountSoldLastYear = new SqlCommand();
                    CountSoldLastYear.Connection = conn;
                    CountSoldLastYear.CommandText = @"select SUM(Quantity) AS AllQuantity,ProductName from Orders where OrderTime >@DateStart and OrderTime<@DateEnd GROUP BY ProductName ORDER BY AllQuantity DESC ";
                    SqlCommand CountSoldLastMonth = new SqlCommand();
                    CountSoldLastMonth.Connection = conn;
                    CountSoldLastMonth.CommandText = @"select SUM(Quantity) AS AllQuantity,ProductName from Orders where OrderTime >@DateStart and OrderTime<@DateEnd GROUP BY ProductName ORDER BY AllQuantity DESC ";
                    try
                    {
                        conn.Open();//打开数据库
                        //完成的订单
                        SqlDataReader sdrOrderReader = SelOrders.ExecuteReader();
                        var OrderItems = new List<Orders> { };
                        while (sdrOrderReader.Read())
                        {
                            OrderItems.Add(new Orders
                            {
                                OrderId = sdrOrderReader.GetInt32(0),
                                UserName = sdrOrderReader.GetString(1),
                                ProductId = sdrOrderReader.GetInt32(2),
                                ProductName = sdrOrderReader.GetString(3),
                                ProductPrice = sdrOrderReader.GetDouble(4),
                                Quantity = sdrOrderReader.GetInt32(5),
                                AllPrice = sdrOrderReader.GetDouble(6),
                                OrderTime = sdrOrderReader.GetDateTime(7),
                                OrderStatus = sdrOrderReader.GetString(8),
                                ShopTo = sdrOrderReader.GetString(9),
                            });
                        }
                        sdrOrderReader.Close();
                        ViewBag.OrderItems = Newtonsoft.Json.JsonConvert.SerializeObject(OrderItems);
                        //前一年的销售情况
                        CountSoldLastYear.Parameters.AddWithValue("@DateStart", LastYearStart);
                        CountSoldLastYear.Parameters.AddWithValue("@DateEnd", YearStart);
                        SqlDataReader LastYearSDR = CountSoldLastYear.ExecuteReader();
                        var LastYearSold = new List<CountSoldList> { };
                        while (LastYearSDR.Read())
                        {
                            LastYearSold.Add(new CountSoldList
                            {
                                AllQuantity = LastYearSDR.GetInt32(0),  
                                ProductName = LastYearSDR.GetString(1), 
                            });
                        }
                        LastYearSDR.Close();
                        ViewBag.LastYearSold = Newtonsoft.Json.JsonConvert.SerializeObject(LastYearSold);
                        //本年至今的销售情况
                        CountSoldTheYear.Parameters.AddWithValue("@DateStart", YearStart);
                        CountSoldTheYear.Parameters.AddWithValue("@DateEnd", dt);
                        SqlDataReader TheYearSDR = CountSoldTheYear.ExecuteReader();
                        var TheYearSold = new List<CountSoldList> { };
                        while (TheYearSDR.Read())
                        {
                            TheYearSold.Add(new CountSoldList
                            {
                                AllQuantity = TheYearSDR.GetInt32(0),
                                ProductName = TheYearSDR.GetString(1),
                            });
                        }
                        TheYearSDR.Close();
                        ViewBag.TheYearSold = Newtonsoft.Json.JsonConvert.SerializeObject(TheYearSold);
                        //上个月的销售情况
                        CountSoldLastMonth.Parameters.AddWithValue("@DateStart", LastMonthStart);
                        CountSoldLastMonth.Parameters.AddWithValue("@DateEnd", MonthStart);
                        SqlDataReader LastMonthSDR = CountSoldLastMonth.ExecuteReader();
                        var LastMonthSold = new List<CountSoldList> { };
                        while (LastMonthSDR.Read())
                        {
                            LastMonthSold.Add(new CountSoldList
                            {
                                AllQuantity = LastMonthSDR.GetInt32(0), 
                                ProductName = LastMonthSDR.GetString(1),
                            });
                        }
                        LastMonthSDR.Close();
                        ViewBag.LastMonthSold = Newtonsoft.Json.JsonConvert.SerializeObject(LastMonthSold);
                    }
                    catch (Exception ee)
                    {
                        Response.Write(ee.Message);
                        Response.Write(ee);
                    }
                    finally
                    {
                        CountSoldLastMonth = null;
                        CountSoldLastYear = null;
                        CountSoldTheYear = null;
                        SelOrders = null;
                        conn.Close();
                        conn = null;
                    }
                }
                else
                {
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
        /// 客户留言---已完成
        /// </summary>
        public ActionResult UserMessages()
        {
            try
            {
                //这里是查看订单 
                SqlConnection conn = Dbconn.getConnn();
                string GMUsers = SessionSG.GetSession("GMUsers").ToString();
                string GMPower = SessionSG.GetSession("GMPower").ToString();
                string tempSuperGM = "超级管理员";
                if (GMPower == tempSuperGM)
                {
                    SqlCommand SelMessages = new SqlCommand();//(sql语句，连接对象和字符串)
                    SelMessages.Connection = conn;
                    SelMessages.CommandText = @"select * from Messages";
                    try
                    {
                        conn.Open();//打开数据库  
                        SqlDataReader sdrMessage = SelMessages.ExecuteReader();//读取留言信息
                        var MessageList = new List<Messages> { };
                        while (sdrMessage.Read())
                        {
                            MessageList.Add(new Messages
                            { 
                                Id = sdrMessage.GetInt32(0),
                                UserName = sdrMessage.GetString(1),
                                Message = sdrMessage.GetString(2),
                            });
                        }
                        sdrMessage.Close();
                        ViewBag.MessageList = Newtonsoft.Json.JsonConvert.SerializeObject(MessageList);
                    }
                    catch (Exception ee)
                    {
                        Response.Write(ee.Message);
                    }
                    finally
                    {
                        SelMessages = null;
                        conn.Close();//关闭数据库
                        conn = null;
                    }
                }
                else
                {
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
        /// 点击查看用户信息---已完成
        /// </summary>
        public ActionResult UserList(string UserName)
        {
            try
            {
                //这里是查看用户个人信息
                SqlConnection conn = Dbconn.getConnn();
                string GMUsers = SessionSG.GetSession("GMUsers").ToString();
                string GMPower = SessionSG.GetSession("GMPower").ToString();
                string tempSuperGM = "超级管理员";
                if (GMPower == tempSuperGM)
                {
                    SqlCommand sqlBaseInfo = new SqlCommand();//(sql语句，连接对象和字符串)
                    sqlBaseInfo.Connection = conn;
                    sqlBaseInfo.CommandText = @"select * from Users where UserName=@UserName";
                    sqlBaseInfo.Parameters.AddWithValue("@Username", UserName);
                    try
                    {
                        conn.Open();//打开数据库
                                    //读取基本信息
                        SqlDataReader sdrBaseInfo = sqlBaseInfo.ExecuteReader();
                        var UserItem = new List<Users> { };
                        while (sdrBaseInfo.Read())
                        {
                            UserItem.Add(new Users
                            {
                                //用户id和用户密码无法被查看
                                Id = sdrBaseInfo.GetInt32(0),
                                UserName = sdrBaseInfo.GetString(1),
                                //UserPassword = sdr.GetString(2),
                                UserEmail = sdrBaseInfo.GetString(3),
                                UserPhone = sdrBaseInfo.GetString(4),
                            });
                        }
                        sdrBaseInfo.Close();
                        ViewBag.UserItem = Newtonsoft.Json.JsonConvert.SerializeObject(UserItem);
                    }
                    catch (Exception ee)
                    {
                        Response.Write(ee.Message);
                    }
                    finally
                    {
                        sqlBaseInfo = null; 
                        conn.Close();//关闭数据库
                        conn = null;
                    }
                }
                else
                {
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
    }
}