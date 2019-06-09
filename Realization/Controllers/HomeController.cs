using System;
using System.Collections.Generic;
using System.Data.SqlClient;//连数据库需要
using System.Web.Mvc;
using System.Collections;//数组的add方法
using Realization.Models;

namespace Realization.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        ///  关于---已完成
        /// </summary>
        /// <returns></returns>
        public ActionResult About()
        {
            return View();
        }
        /// <summary>
        ///  联系方式---已完成
        /// </summary>
        /// <returns></returns>
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page."; 
            return View();
        }
        /// <summary>
        /// 注册页---已完成 
        /// </summary>
        public ActionResult Register(string Register_Btn_Click)
        {
            if (Register_Btn_Click == "注册")
            {
                string Username = Request["UserNAME"];
                string Userpwd = Request["UserPWD"];
                string Userpwds = Request["UserPWDS"];
                string Useremail = Request["UserEMAIL"];
                string Userphone = Request["UserPHONE"]; 
                if (Userpwds == Userpwd)
                {
                    //注册时加密
                    Userpwd = Models.Utils.HashUtil.GetSha256FromString(Userpwd);
                    SqlConnection conn = Dbconn.getConnn(); 
                    SqlCommand AddUser = new SqlCommand();//sql语句，连接对象和字符串  1用于注册，插入数据
                    AddUser.Connection = conn;
                    AddUser.CommandText = @"insert into Users(username,userpassword,useremail,userphone)values(@Username,@Userpwd,@Useremail,@Userphone)";
                    AddUser.Parameters.AddWithValue("@Username", Username);
                    AddUser.Parameters.AddWithValue("@Userpwd", Userpwd);
                    AddUser.Parameters.AddWithValue("@Useremail", Useremail);
                    AddUser.Parameters.AddWithValue("@Userphone", Userphone); 
                    SqlCommand IfName = new SqlCommand();//2号判断用户名是否存在
                    IfName.Connection = conn;
                    IfName.CommandText = @"select * from Users where username=@Username";
                    IfName.Parameters.AddWithValue("@Username", Username); 
                    SqlCommand IfPhone = new SqlCommand();//3号判断手机是否存在
                    IfPhone.Connection = conn;
                    IfPhone.CommandText = @"select * from Users where userphone=@Userphone";
                    IfPhone.Parameters.AddWithValue("@Userphone", Userphone); 
                    SqlCommand IfEmail = new SqlCommand();//4号判断邮箱是否存在
                    IfEmail.Connection = conn;
                    IfEmail.CommandText = @"select * from Users where useremail=@Useremail";
                    IfEmail.Parameters.AddWithValue("@Useremail", Useremail); 
                    try
                    {
                        conn.Open();//打开数据库 
                        SqlDataReader NameReader = IfName.ExecuteReader();
                        bool NameExist = NameReader.Read();//上下三行，判断有没有读到这个用户名
                        NameReader.Close(); 
                        if (NameExist == false)
                        {
                            SqlDataReader PhoneReader = IfPhone.ExecuteReader();
                            bool PhoneExist = PhoneReader.Read();//上下三行，判断有没有读到这个手机
                            PhoneReader.Close(); 
                            if (PhoneExist == false)
                            {
                                SqlDataReader EmailReader = IfEmail.ExecuteReader();
                                bool EmailExist = EmailReader.Read();//上下三行，判断有没有读到这邮箱
                                EmailReader.Close();
                                if (EmailExist == false)
                                {
                                    AddUser.ExecuteNonQuery();
                                    SessionSG.SetSession("Users", Username);//设置session对象Users的值
                                    Response.Write("<script>alert('注册成功！已经为您自动登录');" +
                                        "window.location.href='../Home/Index';</script>");//通过页面js的方法跳出弹窗，并跳转页面
                                }
                                else
                                {
                                    Response.Write("<script>alert('该邮箱已被注册！')</script>");

                                }
                            }
                            else
                            {
                                Response.Write("<script>alert('该手机已被注册！')</script>");

                            }
                        }
                        else
                        {
                            Response.Write("<script>alert('该用户名已经存在！')</script>");

                        }
                    }
                    catch (Exception ee)
                    {
                        Response.Write(ee);
                    }
                    finally
                    {
                        AddUser = null;
                        IfName = null;
                        IfPhone = null;
                        IfEmail = null;
                        conn.Close();
                        conn = null;
                    }
                }
                else
                {
                    Response.Write("<script>alert('两次密码输入不一致!请重新输入')</script>");
                }
            }
            return View();
        }
        /// <summary>
        /// 登录页---已完成 
        /// </summary>
        /// <returns></returns>
        public ActionResult Login(string Login_Btn_Click)
        { 
            if (Login_Btn_Click == "登录")
            {
                string UserPhone = Request["UserPhone"];//获取页面上name为UserPhone的值
                string Userpwd = Request["UserPWD"];//获取页面上name为UserPWD的值
                //登录时加密
                Userpwd = Models.Utils.HashUtil.GetSha256FromString(Userpwd);
                SqlConnection conn = Dbconn.getConnn();
                //查询数据库有没有这个手机
                SqlCommand sqlcomPhone = new SqlCommand();
                sqlcomPhone.Connection = conn;
                sqlcomPhone.CommandText = @"select userpassword,username,userphone from Users where UserPhone=@UserPhone";
                sqlcomPhone.Parameters.AddWithValue("@UserPhone", UserPhone);//加参数的方法,获取到的页面上手机号
                try
                {
                    conn.Open();//打开数据库 
                    SqlDataReader sdrPhone = sqlcomPhone.ExecuteReader();
                    bool flagPhone = sdrPhone.Read();   //有这个手机
                    if (flagPhone == true)
                    {
                        string TempPwd = sdrPhone.GetString(0);
                        string TempName = sdrPhone.GetString(1);
                        sdrPhone.Close();
                        if (Userpwd.Equals(TempPwd))//成功登录
                        {
                            SessionSG.SetSession("Users", TempName);//设置session对象Users的值 为取到的用户名
                            return RedirectToAction("Index");//登录成功后回主页
                        }
                        else
                        {
                            Response.Write("<script>alert('密码错误')</script>");
                        }
                    }
                    else
                    {
                        Response.Write("<script>alert('你输入的账号不存在')</script>");
                    }
                }
                catch (Exception ee)
                {
                    Response.Write(ee.Message);
                }
                finally
                {
                    sqlcomPhone = null;
                    conn.Close();
                    conn = null;
                }
            }
            return View();
        }
        /// <summary> 
        /// 注销---已完成
        /// </summary>
        public ActionResult Logout()
        {
            SessionSG.RemoveSession("Users");
            return RedirectToAction("Index");
        }
        /// <summary>
        /// 用户信息---已完成
        /// </summary>
        public ActionResult Imformation(string AddressCURD, string Alert_Phone, string Alert_Email)
        {
            try
            {
                string UserName = SessionSG.GetSession("Users").ToString();//一般就在这块儿报错了 
                SqlConnection conn = Dbconn.getConnn();
                //修改基本信息-手机
                if (Alert_Phone == "修改")
                {
                    string Phone = Request["Phone"];
                    //先查一下数据库原来的电话
                    SqlCommand GetPhoneBefore = new SqlCommand();
                    GetPhoneBefore.Connection = conn;
                    GetPhoneBefore.CommandText = @"select UserPhone from Users where UserName=@UserName";
                    GetPhoneBefore.Parameters.AddWithValue("@UserName", UserName);//加参数的方法,用的是session中的UserName
                    //再查一下现在的电话
                    SqlCommand GetPhoneNow = new SqlCommand();
                    GetPhoneNow.Connection = conn;
                    GetPhoneNow.CommandText = @"select * from Users where userphone=@Userphone";
                    GetPhoneNow.Parameters.AddWithValue("@Userphone", Phone);//加参数的方法,获取到的页面上手机号
                    //修改电话
                    SqlCommand AlertPhone = new SqlCommand();
                    AlertPhone.Connection = conn;
                    AlertPhone.CommandText = @"update Users set UserPhone=@UserPhone where UserName=@UserName";
                    AlertPhone.Parameters.AddWithValue("@Userphone", Phone);
                    AlertPhone.Parameters.AddWithValue("@UserName", UserName);
                    try
                    {
                        conn.Open();//打开数据库 
                        SqlDataReader sdrPhoneBefore = GetPhoneBefore.ExecuteReader();
                        sdrPhoneBefore.Read();
                        string PhoneBefore = sdrPhoneBefore.GetString(0);
                        sdrPhoneBefore.Close();
                        if (PhoneBefore.Equals(Phone)) { Response.Write("<script>alert('您没有进行任何修改')</script>"); }
                        else
                        {
                            SqlDataReader sdrPhoneNow = GetPhoneNow.ExecuteReader();
                            bool flag = sdrPhoneNow.Read();
                            sdrPhoneNow.Close();
                            if (flag == false)
                            {
                                AlertPhone.ExecuteNonQuery();
                                Response.Write("<script>alert('请记好您修改后的手机号，将用作登录使用');</script>");
                            }
                            else { Response.Write("<script>alert('这个号码已经被使用')</script>"); }

                        }
                    }
                    catch (Exception ee) { Response.Write(ee.Message); }
                    finally
                    {
                        GetPhoneBefore = null;
                        GetPhoneNow = null;
                        AlertPhone = null;
                        conn.Close();
                        conn = null;
                        Response.Write("<script>window.location.href='../Home/Imformation'</script>");
                    }

                }
                //修改基本信息-邮箱
                if (Alert_Email == "修改")
                {
                    string Email = Request["Email"];
                    //先查一下数据库原来的邮箱
                    SqlCommand GetEmailBefore = new SqlCommand();
                    GetEmailBefore.Connection = conn;
                    GetEmailBefore.CommandText = @"select UserEmail from Users where UserName=@UserName";
                    GetEmailBefore.Parameters.AddWithValue("@UserName", UserName);//加参数的方法,用的是session中的UserName
                                                                                  //再查一下现在的邮箱
                    SqlCommand GetEmailNow = new SqlCommand();
                    GetEmailNow.Connection = conn;
                    GetEmailNow.CommandText = @"select * from Users where UserEmail=@UserEmail";
                    GetEmailNow.Parameters.AddWithValue("@UserEmail", Email);//加参数的方法,获取到的页面上邮箱号
                                                                             //修改邮箱
                    SqlCommand AlertEmail = new SqlCommand();
                    AlertEmail.Connection = conn;
                    AlertEmail.CommandText = @"update Users set UserEmail=@UserEmail where UserName=@UserName";
                    AlertEmail.Parameters.AddWithValue("@UserEmail", Email);
                    AlertEmail.Parameters.AddWithValue("@UserName", UserName);
                    try
                    {
                        conn.Open();//打开数据库 
                        SqlDataReader sdrEmailBefore = GetEmailBefore.ExecuteReader();
                        sdrEmailBefore.Read();
                        string EmailBefore = sdrEmailBefore.GetString(0);
                        sdrEmailBefore.Close();
                        if (EmailBefore.Equals(Email)) { Response.Write("<script>alert('您没有进行任何修改')</script>"); }
                        else
                        {
                            SqlDataReader sdrEmailNow = GetEmailNow.ExecuteReader();
                            bool flag = sdrEmailNow.Read();
                            sdrEmailNow.Close();
                            if (flag == false)
                            {
                                AlertEmail.ExecuteNonQuery();

                            }
                            else { Response.Write("<script>alert('这个邮箱已经被使用')</script>"); }

                        }
                    }
                    catch (Exception ee) { Response.Write(ee.Message); }
                    finally
                    {
                        GetEmailBefore = null;
                        GetEmailNow = null;
                        AlertEmail = null;
                        conn.Close();
                        conn = null;
                        Response.Write("<script>window.location.href='../Home/Imformation'</script>");
                    }

                }
                //添加收件人信息
                if (AddressCURD == "添加")
                {
                    string Name = Request["Name"];
                    string Phone = Request["Phone"];
                    string Address = Request["Address"];
                    SqlCommand InsertAddress = new SqlCommand();//sql语句，连接对象和字符串  1用于注册，插入数据
                    InsertAddress.Connection = conn;
                    InsertAddress.CommandText = @"insert into Addresses(Username,Name,Phone,Address)values(@UserName,@Name,@Phone,@Address)";
                    InsertAddress.Parameters.AddWithValue("@UserName", UserName);
                    InsertAddress.Parameters.AddWithValue("@Name", Name);
                    InsertAddress.Parameters.AddWithValue("@Phone", Phone);
                    InsertAddress.Parameters.AddWithValue("@Address", Address);
                    try
                    {
                        conn.Open();//打开数据库
                        InsertAddress.ExecuteNonQuery();
                        Response.Write("<script>alert('收件人添加成功！');window.location.href='../Home/Imformation';</script>");
                    }
                    catch (Exception eee)
                    {
                        Response.Write(eee);
                        return View();
                    }
                    finally
                    {
                        InsertAddress = null;
                        conn.Close();
                        conn = null;
                    }
                }
                //修改收件人信息
                if (AddressCURD == "修改")
                {
                    int Id = int.Parse(Request["AddressId"]);
                    string Name = Request["Name"];
                    string Phone = Request["Phone"];
                    string Address = Request["Address"];
                    SqlCommand AlertAddress = new SqlCommand();//sql语句，连接对象和字符串  用于修改
                    AlertAddress.Connection = conn;
                    AlertAddress.CommandText = @"Update  Addresses set Name=@Name,Phone=@Phone,Address=@Address where Id=@Id";
                    AlertAddress.Parameters.AddWithValue("@Name", Name);
                    AlertAddress.Parameters.AddWithValue("@Phone", Phone);
                    AlertAddress.Parameters.AddWithValue("@Address", Address);
                    AlertAddress.Parameters.AddWithValue("@Id", Id);
                    try
                    {
                        conn.Open();
                        AlertAddress.ExecuteNonQuery();
                    }
                    catch (Exception ee)
                    {
                        Response.Write(ee.Message);
                    }
                    finally
                    {
                        AlertAddress = null;
                        conn.Close();//关闭数据库
                        conn = null;
                        Response.Write("<script>window.location.href='../Home/Imformation'</script>");
                    }
                }
                //删除收件人信息
                if (AddressCURD == "删除")
                {   //window.location.href='../Home/Imformation';
                    try
                    {
                        int Id = int.Parse(Request["AddressId"]);
                        SqlCommand deleteSqlcommand = new SqlCommand();//sql语句，连接对象和字符串  用于删除
                        deleteSqlcommand.Connection = conn;
                        deleteSqlcommand.CommandText = @"Delete from Addresses where Id=@Id";
                        deleteSqlcommand.Parameters.AddWithValue("@Id", Id);
                        //判断数据是否存在
                        SqlCommand IfId = new SqlCommand();
                        IfId.Connection = conn;
                        IfId.CommandText = @"select * from Addresses where id=@id";
                        IfId.Parameters.AddWithValue("@id", Id);
                        try
                        {
                            conn.Open();
                            SqlDataReader IdReader = IfId.ExecuteReader();
                            bool ExistId = IdReader.Read();//上下三行，判断有没有读到这个用户名
                            IdReader.Close();
                            if (ExistId == false)
                            {
                                Response.Write("<script>alert('不存在这条记录');</script>");
                            }
                            else
                            {
                                deleteSqlcommand.ExecuteNonQuery(); 
                                Response.Write("<script>alert('删除成功');</script>");
                            }
                            
                        }
                        catch (Exception ee)
                        {
                            Response.Write(ee.Message);
                        }
                        finally
                        {
                            deleteSqlcommand = null;
                            conn.Close();//关闭数据库
                            conn = null;
                            Response.Write("<script>window.location.href='../Home/Imformation'</script>");
                        }
                    }
                    catch (Exception deleteEE)
                    {
                        Response.Write(deleteEE.Message);
                    }
                }
                //查询基本信息：账号邮箱手机
                SqlCommand sqlBaseInfo = new SqlCommand();//(sql语句，连接对象和字符串)
                sqlBaseInfo.Connection = conn;
                sqlBaseInfo.CommandText = @"select * from Users where UserName=@UserName";
                sqlBaseInfo.Parameters.AddWithValue("@Username", UserName);
                //查看收件地址
                SqlCommand sqlAddress = new SqlCommand();//(sql语句，连接对象和字符串)
                sqlAddress.Connection = conn;
                sqlAddress.CommandText = @"select * from Addresses where UserName=@UserName";
                sqlAddress.Parameters.AddWithValue("@Username", UserName);
                //读取基本信息和收件地址，传给页面
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
                    //读取地址信息
                    SqlDataReader sdrAddress = sqlAddress.ExecuteReader();
                    var UserAddressItems = new List<Addresses> { };
                    while (sdrAddress.Read())
                    {
                        UserAddressItems.Add(new Addresses
                        {
                            Id = sdrAddress.GetInt32(0),
                            UserName = sdrAddress.GetString(1),
                            Name = sdrAddress.GetString(2),
                            Phone = sdrAddress.GetString(3),
                            Address = sdrAddress.GetString(4),
                        });
                    }
                    sdrAddress.Close();
                    ViewBag.UserAddressItems = Newtonsoft.Json.JsonConvert.SerializeObject(UserAddressItems);
                }
                catch (Exception ee)
                {
                    Response.Write(ee.Message); 
                }
                finally
                {
                    sqlBaseInfo = null;
                    sqlAddress = null;
                    conn.Close();//关闭数据库
                    conn = null;
                }
                //从页面获取收件人信息，插入数据库

            }
            catch (Exception ee)
            {
                Response.Write("<script>alert('亲，请登录后再进行操作');window.location.href='../Home/Login'</script>");
                Response.Write(ee.Message);
            }
            return View();
        }
        /// <summary>
        /// 密码修改---已完成
        /// </summary>
        public ActionResult AlertUserPwd(string AlertPwd)
        {
            try
            {
                string UserName = SessionSG.GetSession("Users").ToString();//一般就在这块儿报错了、 
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
                        SetNewPwd.CommandText = @"update Users set UserPassword=@NewPwd where UserName=@UserName";
                        SetNewPwd.Parameters.AddWithValue("@NewPwd", NewPwd);
                        SetNewPwd.Parameters.AddWithValue("@UserName", UserName);//加参数的方法,用的是session中的UserName
                        try
                        {
                            conn.Open();
                            SetNewPwd.ExecuteNonQuery();
                            SessionSG.RemoveSession("Users");
                            Response.Write("<script>alert('修改密码成功，请重新登录');window.location.href='../Home'</script>");
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
                Response.Write(ee.Message);
                Response.Write("<script>alert('亲，请登录后再进行操作');window.location.href='../Home/Login'</script>");
            } 
            return View();
        }
        /// <summary>
        /// 添加到购物车---已完成
        /// </summary>
        public ActionResult ListProduct(string ProductType, string UserOrderCRUD)
        {
            try {
            //根据点的不同链接，返回不同的的商品列表
            ViewBag.ProductType = ProductType.ToString();
            //string SelKey = ProductType;
            SqlConnection conn = Dbconn.getConnn(); 
                //添加到购物车
                if (UserOrderCRUD == "添加到购物车") { 
                    try
                    { 
                        string UserName = SessionSG.GetSession("Users").ToString();
                        int ProductId = int.Parse(Request["ProductId"]);
                        string ProductName = Request["ProductName"];
                        float ProductPrice = float.Parse(Request["ProductPriceOut"]);
                        int Quantity = int.Parse(Request["ProductQuantity"]); 
                        SqlCommand AddToCart = new SqlCommand();
                        AddToCart.Connection = conn;
                        AddToCart.CommandText = @"Insert into Carts(UserName,ProductId,ProductName,ProductPrice,Quantity)values(@UserName,@ProductId,@ProductName,@ProductPrice,@Quantity)";
                        AddToCart.Parameters.AddWithValue("@UserName", UserName);
                        AddToCart.Parameters.AddWithValue("@ProductId", ProductId);
                        AddToCart.Parameters.AddWithValue("@ProductName", ProductName);
                        AddToCart.Parameters.AddWithValue("@ProductPrice", ProductPrice);
                        AddToCart.Parameters.AddWithValue("@Quantity", Quantity);
                        try { 
                            conn.Open();
                            AddToCart.ExecuteNonQuery();
                            Response.Write("<script>alert('添加到购物车成功');window.location.href='../Home/ListProduct?ProductType=" + ProductType + "';</script>"); 
                        }
                        catch (Exception ee) {
                            Response.Write(ee.Message);
                        } finally {
                            AddToCart = null;
                            conn.Close();
                            conn = null;
                        }
                    }
                    catch (Exception ee)
                    {
                        Response.Write("<script>alert('亲，请登录后再进行操作');window.location.href='../Home/Login'</script>");
                        Response.Write(ee.Message);
                    }
                }
                //遍历这个类型的商品
                SqlCommand SelProductListStock = new SqlCommand();//(sql语句，连接对象和字符串)
                SelProductListStock.Connection = conn;
                SelProductListStock.CommandText = @"select Products.ProductId," +
                    "Products.ProductType," +
                    "Products.ProductBrand," +
                    "Products.ProductName," +
                    "Products.ProductPriceOut," +
                    "Products.ProductModel," +
                    "Products.ProductDetail," +
                    "Products.ProductPath," +
                    "Stocks.ProductAmount " +
                    " from Products,Stocks where" +
                    " Products.ProductId=Stocks.ProductId and Products.ProductType = @ProductType";
                SelProductListStock.Parameters.AddWithValue("@ProductType", ProductType);
                try
            {
                conn.Open();//打开数据库
                SqlDataReader sdr = SelProductListStock.ExecuteReader();
                var products = new List<ProductListStock> { };
                while (sdr.Read())
                {
                    products.Add(new ProductListStock
                    {
                        ProductId = sdr.GetInt32(0),
                        ProductType = sdr.GetString(1),
                        ProductBrand = sdr.GetString(2),
                        ProductName = sdr.GetString(3),
                        ProductPriceOut = sdr.GetDouble(4),
                        ProductModel = sdr.GetString(5),
                        ProductDetail = sdr.GetString(6),
                        ProductPath = sdr.GetString(7),
                        ProductAmount = sdr.GetInt32(8),
                    });
                }
                sdr.Close();
                ViewBag.ProductList = Newtonsoft.Json.JsonConvert.SerializeObject(products);
            }
            catch (Exception ee)
            {
                Response.Write(ee.Message);
                Response.Write(ee);
                return View();
            }
            finally
            {
                SelProductListStock = null;
                conn.Close();//关闭数据库
                conn = null;
            }
            }
            catch (Exception ee) {
               
                Response.Write(ee.Message);
                Response.Write(ee);
                //Response.Write("<script>window.location.href='../Home/Index'</script>");
            }
            return View();
        }
        /// <summary>
        /// 购物车---已完成
        /// </summary>
        public ActionResult Carts(string CartsCURD)
        {
            try {
                string UserName = SessionSG.GetSession("Users").ToString();
                SqlConnection conn = Dbconn.getConnn();
                if (CartsCURD == "下单") {
                    int Id = int.Parse(Request["Id"]);
                    int ProductId = int.Parse(Request["ProductId"]);
                    string ProductName = Request["ProductName"];
                    //UserName = UserName;
                    double ProductPrice = double.Parse(Request["ProductPrice"]);
                    int Quantity = int.Parse(Request["Quantity"]);
                    double AllPrice = ProductPrice * Quantity; 
                    DateTime OrderTime= DateTime.Now;
                    string OrderStatus = "已下单";
                    string ShopTo = Request["ShopTo"];  
                    SqlCommand InsertOrder = new SqlCommand(); 
                    InsertOrder.Connection = conn;
                    InsertOrder.CommandText = @"insert into Orders(Username,ProductId,ProductName,ProductPrice,Quantity,AllPrice,OrderTime,OrderStatus,ShopTo)values(@UserName,@ProductId,@ProductName,@ProductPrice,@Quantity,@AllPrice,@OrderTime,@OrderStatus,@ShopTo)";
                    InsertOrder.Parameters.AddWithValue("@UserName", UserName);
                    InsertOrder.Parameters.AddWithValue("@ProductId", ProductId);
                    InsertOrder.Parameters.AddWithValue("@ProductName", ProductName);
                    InsertOrder.Parameters.AddWithValue("@ProductPrice", ProductPrice);
                    InsertOrder.Parameters.AddWithValue("@Quantity", Quantity);
                    InsertOrder.Parameters.AddWithValue("@AllPrice", AllPrice);
                    InsertOrder.Parameters.AddWithValue("@OrderTime", OrderTime);
                    InsertOrder.Parameters.AddWithValue("@OrderStatus", OrderStatus);
                    InsertOrder.Parameters.AddWithValue("@ShopTo", ShopTo); 
                    SqlCommand EndCarts = new SqlCommand();
                    EndCarts.Connection = conn;
                    EndCarts.CommandText = @"Delete from Carts where Id=@Id";
                    EndCarts.Parameters.AddWithValue("@Id", Id);  
                    try
                    {
                        conn.Open();//打开数据库
                        InsertOrder.ExecuteNonQuery();
                        EndCarts.ExecuteNonQuery();
                        Response.Write("<script>alert('下单成功！');window.location.href='../Home/Carts';</script>");
                    }
                    catch (Exception ee)
                    {
                        Response.Write(ee); 
                    }
                    finally
                    {
                        EndCarts = null;
                        InsertOrder = null;
                        conn.Close();
                        conn = null;
                    }
                }
                if (CartsCURD == "删除") { 
                    int Id = int.Parse(Request["Id"]);
                    SqlCommand DeleteCarts = new SqlCommand();
                    DeleteCarts.Connection = conn;
                    DeleteCarts.CommandText = @"Delete from Carts where Id=@Id";
                    DeleteCarts.Parameters.AddWithValue("@Id", Id);
                    try
                    {
                        conn.Open();//打开数据库 
                        DeleteCarts.ExecuteNonQuery();
                        Response.Write("<script>alert('删除成功！');window.location.href='../Home/Carts';</script>");
                    }
                    catch (Exception ee)
                    {
                        Response.Write(ee.Message); 
                    }
                    finally
                    {
                        DeleteCarts = null;
                        conn.Close();
                        conn = null;
                    }

                }
                //查询基本信息：账号邮箱手机
                SqlCommand SelMyCart = new SqlCommand();//(sql语句，连接对象和字符串)
                SelMyCart.Connection = conn;
                SelMyCart.CommandText = @"select * from Carts  where UserName=@UserName";
                SelMyCart.Parameters.AddWithValue("@Username", UserName);//查询基本信息：账号邮箱手机
                //查询他的送货地址
                SqlCommand SelAddress = new SqlCommand();
                SelAddress.Connection = conn;
                SelAddress.CommandText = @"select * from Addresses  where UserName=@UserName";
                SelAddress.Parameters.AddWithValue("@Username", UserName);//查询基本信息：账号邮箱手机
                try
                {
                    conn.Open();//打开数据库
                    SqlDataReader sdrCartReader = SelMyCart.ExecuteReader();
                    var MyCartItems = new List<Carts> { };
                    while (sdrCartReader.Read())
                    {
                        MyCartItems.Add(new Carts
                        {
                            Id = sdrCartReader.GetInt32(0),
                            UserName = sdrCartReader.GetString(1),
                            ProductId = sdrCartReader.GetInt32(2),
                            ProductName = sdrCartReader.GetString(3),
                            ProductPrice = sdrCartReader.GetDouble(4),
                            Quantity= sdrCartReader.GetInt32(5)
                        });
                    }
                    sdrCartReader.Close();
                    ViewBag.MyCartItems = Newtonsoft.Json.JsonConvert.SerializeObject(MyCartItems);

                    SqlDataReader sdrAddress = SelAddress.ExecuteReader();
                    var AddressItems = new List<Addresses> { };
                    while (sdrAddress.Read())
                    {
                        AddressItems.Add(new Addresses
                        {
                            Id = sdrAddress.GetInt32(0),
                            UserName = sdrAddress.GetString(1),
                            Name = sdrAddress.GetString(2),
                            Phone = sdrAddress.GetString(3),
                            Address = sdrAddress.GetString(4),
                        });
                    }
                    sdrAddress.Close();
                    ViewBag.AddressItems = Newtonsoft.Json.JsonConvert.SerializeObject(AddressItems);
                }
                catch (Exception ee)
                { 
                    Response.Write(ee.Message);
                }

                } catch (Exception ee) {
                Response.Write(ee.Message);
                Response.Write("<script>alert('亲，请登录后再进行操作');window.location.href='../Home/Login'</script>");
            }
            return View();
        }
        /// <summary> 
        /// 订单管理---已完成
        /// </summary>
        public ActionResult MyOrders(string OrderCURD)
        {
            try
            { 
                string UserName = SessionSG.GetSession("Users").ToString();
                SqlConnection conn = Dbconn.getConnn();
                //取消订单
                if (OrderCURD == "取消订单") { 
                    int OrderId = int.Parse(Request["OrderId"]); 
                    int ProductId = int.Parse(Request["ProductId"]); 
                    string ProductName = Request["ProductName"]; 
                    float ProductPrice = float.Parse(Request["ProductPrice"]);
                    int Quantity = int.Parse(Request["Quantity"]);
                    float AllPrice = float.Parse(Request["AllPrice"]);
                    DateTime OrderTime = DateTime.Parse(Request["OrderTime"]);
                    string OrderStatus = Request["OrderStatus"];
                    string ShopTo =  Request["ShopTo"];
                    string Reason =  Request["Reason"];
                    string Status = "待处理";
                    //先把原因存进去
                    SqlCommand InsertReason = new SqlCommand();//(sql语句，连接对象和字符串)
                    InsertReason.Connection = conn;
                    InsertReason.CommandText = @"Insert into Reasons(OrderId,Reason,Status)values(@OrderId,@Reason,@Status)";
                    InsertReason.Parameters.AddWithValue("@OrderId", OrderId);
                    InsertReason.Parameters.AddWithValue("@Reason", Reason);
                    InsertReason.Parameters.AddWithValue("@Status", Status);
                    //把订单存起来
                    SqlCommand InsertDOrders = new SqlCommand();//(sql语句，连接对象和字符串)
                    InsertDOrders.Connection = conn;
                    InsertDOrders.CommandText = @"Insert into DOrders(OrderId,UserName,ProductId,ProductName,ProductPrice,Quantity,AllPrice,OrderTime,ShopTo)values(@OrderId,@UserName,@ProductId,@ProductName,@ProductPrice,@Quantity,@AllPrice,@OrderTime,@ShopTo)";
                    InsertDOrders.Parameters.AddWithValue("@OrderId", OrderId);
                    InsertDOrders.Parameters.AddWithValue("@UserName", UserName);
                    InsertDOrders.Parameters.AddWithValue("@ProductId", ProductId);
                    InsertDOrders.Parameters.AddWithValue("@ProductName", ProductName);
                    InsertDOrders.Parameters.AddWithValue("@ProductPrice", ProductPrice);
                    InsertDOrders.Parameters.AddWithValue("@Quantity", Quantity);
                    InsertDOrders.Parameters.AddWithValue("@AllPrice", AllPrice);
                    InsertDOrders.Parameters.AddWithValue("@OrderTime", OrderTime);
                    InsertDOrders.Parameters.AddWithValue("@ShopTo", ShopTo);
                    //把订单删了。
                    SqlCommand DeleteOrders = new SqlCommand();//(sql语句，连接对象和字符串)
                    DeleteOrders.Connection = conn;
                    DeleteOrders.CommandText= @"Delete from Orders where OrderId=@OrderId";
                    DeleteOrders.Parameters.AddWithValue("@OrderId", OrderId);
                    if (OrderStatus=="已下单") {
                        try
                        {
                            conn.Open();
                            InsertReason.ExecuteNonQuery();//先把原因存进去
                            InsertDOrders.ExecuteNonQuery();//把订单存到已被取消的订单中
                            DeleteOrders.ExecuteNonQuery();//删除订单
                            Response.Write("<script>alert('取消成功！');window.location.href='../Home/MyOrders';</script>");
                        }
                        catch (Exception ee) { Response.Write(ee.Message); }
                        finally
                        {
                            InsertReason = null;
                            InsertDOrders = null;
                            DeleteOrders = null;
                            conn.Close();
                            conn = null;
                        }
                    }
                    else if(OrderStatus=="派送中") { Response.Write("<script>alert('订单派送中，无法取消');window.location.href='../Home/MyOrders'</script>"); }
                    else if(OrderStatus=="已完成") { Response.Write("<script>alert('订单已完成，无法取消');window.location.href='../Home/MyOrders'</script>"); }
                    else { Response.Write("<script>alert('状态异常，无法取消');window.location.href='../Home/MyOrders'</script>"); }
                }
                //完成订单
                if (OrderCURD == "确认收货")
                {
                    int OrderId = int.Parse(Request["OrderId"]);
                    string OrderStatus = Request["OrderStatus"];
                    string NewStatus = "已完成";
                    if (OrderStatus == "派送中")
                    {
                        SqlCommand AlertOrders = new SqlCommand();
                        AlertOrders.Connection = conn;
                        AlertOrders.CommandText = @"update Orders set OrderStatus=@NewStatus where OrderId=@OrderId";
                        AlertOrders.Parameters.AddWithValue("@NewStatus", NewStatus); 
                        AlertOrders.Parameters.AddWithValue("@OrderId", OrderId); 
                        try
                        {
                            conn.Open();
                            AlertOrders.ExecuteNonQuery();//修改
                            Response.Write("<script>alert('收货成功！');window.location.href='../Home/MyOrders';</script>");
                        }
                        catch (Exception ee) { Response.Write(ee.Message); }
                        finally
                        {
                            AlertOrders = null; 
                            conn.Close();
                            conn = null;
                        }
                    }
                    else if (OrderStatus == "已下单") { Response.Write("<script>alert('订单尚未派送，无法完成');window.location.href='../Home/MyOrders'</script>"); }
                    else if (OrderStatus == "已完成") { Response.Write("<script>alert('订单已完成');window.location.href='../Home/MyOrders'</script>"); }
                    else { Response.Write("<script>alert('状态异常，无法取消');window.location.href='../Home/MyOrders'</script>"); }
                }
                //全部订单
                SqlCommand SelMyOrder = new SqlCommand();
                SelMyOrder.Connection = conn;
                SelMyOrder.CommandText = @"select * from Orders  where UserName=@UserName ";
                SelMyOrder.Parameters.AddWithValue("@Username", UserName);  
                try {
                    conn.Open();//打开数据库
                    SqlDataReader sdrOrderReader = SelMyOrder.ExecuteReader();
                    var MyOrderItems = new List<Orders> { };
                    while (sdrOrderReader.Read())
                    {
                        MyOrderItems.Add(new Orders
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
                    ViewBag.MyOrderItems = Newtonsoft.Json.JsonConvert.SerializeObject(MyOrderItems);

                }
                catch (Exception ee)
                {
                    Response.Write(ee.Message); 
                } 
                finally
                {
                    SelMyOrder = null;
                    conn.Close();
                    conn = null;
                }
            }
            catch (Exception ee)
            {
                Response.Write(ee.Message);
                Response.Write("<script>alert('亲，请登录后再进行操作');window.location.href='../Home/Login'</script>");
            }
            return View();
        } 
        /// <summary>
        /// 留言板---已完成
        /// </summary>
        public ActionResult Messages(string MessageCURD)
        {
            try
            {
                string UserName = SessionSG.GetSession("Users").ToString();//一般就在这块儿报错了 
                SqlConnection conn = Dbconn.getConnn();
                //添加留言
                if (MessageCURD == "留言")
                { 
                    string Message = Request["Message"];
                    SqlCommand AddMessages = new SqlCommand();//sql语句，连接对象和字符串  1用于注册，插入数据
                    AddMessages.Connection = conn;
                    AddMessages.CommandText = @"insert into Messages(Username,Message)values(@Username,@Message)"; ;
                    AddMessages.Parameters.AddWithValue("@Username", UserName);
                    AddMessages.Parameters.AddWithValue("@Message", Message);
                    try
                    {
                        conn.Open();
                        AddMessages.ExecuteNonQuery();
                        Response.Write("<script>alert('留言成功！');window.location.href='../Home/Messages';</script>");

                    }
                    catch (Exception ee)
                    {
                        Response.Write(ee.Message);
                    }
                    finally
                    {
                        AddMessages = null;
                        conn.Close();//关闭数据库
                        conn = null;
                    }
                }
                //删除留言
                if (MessageCURD == "删除")
                {
                    string TUserName= Request["UserName"];
                    int Id = int.Parse(Request["MessageId"]);
                    if (UserName.Equals(TUserName))
                    {
                        SqlCommand DelMessages = new SqlCommand();//sql语句，连接对象和字符串  1用于注册，插入数据
                        DelMessages.Connection = conn;
                        DelMessages.CommandText = @"Delete from Messages where Id=@Id"; 
                        DelMessages.Parameters.AddWithValue("@Id", Id); 
                        try
                        {
                            conn.Open();
                            DelMessages.ExecuteNonQuery();
                            Response.Write("<script>alert('删除成功！');window.location.href='../Home/Messages';</script>");

                        }
                        catch (Exception ee)
                        {
                            Response.Write(ee.Message);
                        }
                        finally
                        {
                            DelMessages = null;
                            conn.Close();//关闭数据库
                            conn = null;
                        }
                    }
                    else { Response.Write("<script>alert('您无法删除别人的留言');window.location.href='../Home/Messages'</script>"); }
                }
                //查看全部留言
                SqlCommand SelMessages = new SqlCommand();//(sql语句，连接对象和字符串)
                SelMessages.Connection = conn;
                SelMessages.CommandText = @"select * from Messages";
                // where UserName=@UserName
                //SelMessages.Parameters.AddWithValue("@UserName", UserName);
                try
                {
                    conn.Open();//打开数据库  
                    SqlDataReader sdrMessage = SelMessages.ExecuteReader();//读取留言信息
                    var MessageList = new List<Messages> { };
                    while (sdrMessage.Read())
                    {
                        MessageList.Add(new Messages
                        {
                            //用户id和用户密码无法被查看
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
            catch (Exception ee)
            {
                Response.Write("<script>alert('亲，请登录后再进行操作');window.location.href='../Home/Login'</script>");
                Response.Write(ee.Message);
            }
            return View();
        }
        /// <summary>
        /// 页面-主页---已完成
        /// </summary>
        public ActionResult Index()
        { 
            SqlConnection conn = Dbconn.getConnn();
            SqlCommand sqlcommand = new SqlCommand();//(sql语句，连接对象和字符串)
            sqlcommand.Connection = conn;
            sqlcommand.CommandText = @"select Products.ProductId," +
                "Products.ProductType," +
                "Products.ProductBrand," +
                "Products.ProductName," +
                "Products.ProductPriceOut," +
                "Products.ProductModel," +
                "Products.ProductDetail," +
                "Products.ProductPath," +
                "Stocks.ProductAmount " +
                " from Products,Stocks where Products.ProductId=Stocks.ProductId";
            try
            {
                conn.Open();//打开数据库
                SqlDataReader sdr = sqlcommand.ExecuteReader();
                var products = new List<ProductListStock> { };
                while (sdr.Read())
                {
                    products.Add(new ProductListStock
                    {
                        ProductId = sdr.GetInt32(0),
                        ProductType = sdr.GetString(1),
                        ProductBrand = sdr.GetString(2),
                        ProductName = sdr.GetString(3),
                        ProductPriceOut = sdr.GetDouble(4),
                        ProductModel = sdr.GetString(5),
                        ProductDetail = sdr.GetString(6),
                        ProductPath = sdr.GetString(7),
                        ProductAmount = sdr.GetInt32(8),
                    });
                }
                sdr.Close();
                ViewBag.ProductList = Newtonsoft.Json.JsonConvert.SerializeObject(products);
            }
            catch (Exception ee)
            {
                Response.Write(ee.Message);
                Response.Write(ee);
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
        /// 点击查看商品信息---已完成
        /// </summary> 
        public ActionResult ProductList(string ProductId)
        {
            SqlConnection conn = Dbconn.getConnn();
            SqlCommand TheProduct = new SqlCommand();//(sql语句，连接对象和字符串)
            TheProduct.Connection = conn;
            TheProduct.CommandText = @"select Products.ProductId," +
                "Products.ProductType," +
                "Products.ProductBrand," +
                "Products.ProductName," +
                "Products.ProductPriceOut," +
                "Products.ProductModel," +
                "Products.ProductDetail," +
                "Products.ProductPath," +
                "Stocks.ProductAmount " +
                "from Products,Stocks where " +
                "Products.ProductId=Stocks.ProductId " +
                "and Products.ProductId=@ProductId";
            TheProduct.Parameters.AddWithValue("@ProductId", ProductId);
            try
            {
                conn.Open();//打开数据库
                SqlDataReader sdr = TheProduct.ExecuteReader();
                var products = new List<ProductListStock> { };
                while (sdr.Read())
                {
                    products.Add(new ProductListStock
                    {
                        ProductId = sdr.GetInt32(0),
                        ProductType = sdr.GetString(1),
                        ProductBrand = sdr.GetString(2),
                        ProductName = sdr.GetString(3),
                        ProductPriceOut = sdr.GetDouble(4),
                        ProductModel = sdr.GetString(5),
                        ProductDetail = sdr.GetString(6),
                        ProductPath = sdr.GetString(7),
                        ProductAmount = sdr.GetInt32(8),
                    });
                }
                sdr.Close();
                ViewBag.ProductList = Newtonsoft.Json.JsonConvert.SerializeObject(products);
            }
            catch (Exception ee)
            {
                Response.Write(ee.Message);
                Response.Write(ee);
            }
            finally
            {
                TheProduct = null;
                conn.Close();//关闭数据库
                conn = null;
            }
            return View();
        }
    }
}