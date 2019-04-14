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
        /// 页面-主页
        /// 浏览商品信息
        /// </summary>
        public ActionResult Index()
        {
            SqlConnection conn = Dbconn.getConnn();
            SqlCommand sqlcommand = new SqlCommand();//(sql语句，连接对象和字符串)
            sqlcommand.Connection = conn;
            sqlcommand.CommandText = "select * from Products";
            try
            {
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
                        ProductName = sdr.GetString(3),
                        ProductPriceIn = sdr.GetDouble(4),
                        ProductPriceOut = sdr.GetDouble(5),
                        ProductModel = sdr.GetString(6),
                        ProductDetail = sdr.GetString(7),
                        ProductPath = sdr.GetString(8)
                    });
                }
                sdr.Close();
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
        /// 页面-关于店家
        /// </summary>
        /// <returns></returns>
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        /// <summary>
        /// 页面-店家联系方式
        /// </summary>
        /// <returns></returns>
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        /// <summary>
        /// 功能-注册页 
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

                    SqlCommand sqlcommand1 = new SqlCommand();//sql语句，连接对象和字符串  1用于注册，插入数据
                    sqlcommand1.Connection = conn;
                    sqlcommand1.CommandText = @"insert into Users(username,userpassword,useremail,userphone)values(@Username,@Userpwd,@Useremail,@Userphone)";
                    sqlcommand1.Parameters.AddWithValue("@Username", Username);
                    sqlcommand1.Parameters.AddWithValue("@Userpwd", Userpwd);
                    sqlcommand1.Parameters.AddWithValue("@Useremail", Useremail);
                    sqlcommand1.Parameters.AddWithValue("@Userphone", Userphone);

                    SqlCommand sqlcommand2 = new SqlCommand();//2号判断用户名是否存在
                    sqlcommand2.Connection = conn;
                    sqlcommand2.CommandText = @"select * from Users where username=@Username";
                    sqlcommand2.Parameters.AddWithValue("@Username", Username);

                    SqlCommand sqlcommand3 = new SqlCommand();//3号判断手机是否存在
                    sqlcommand3.Connection = conn;
                    sqlcommand3.CommandText = @"select * from Users where userphone=@Userphone";
                    sqlcommand3.Parameters.AddWithValue("@Userphone", Userphone);

                    SqlCommand sqlcommand4 = new SqlCommand();//4号判断邮箱是否存在
                    sqlcommand4.Connection = conn;
                    sqlcommand4.CommandText = @"select * from Users where useremail=@Useremail";
                    sqlcommand4.Parameters.AddWithValue("@Useremail", Useremail);

                    try
                    {
                        conn.Open();//打开数据库

                        SqlDataReader sqldatareader2 = sqlcommand2.ExecuteReader();
                        bool flag2 = sqldatareader2.Read();//上下三行，判断有没有读到这个用户名
                        sqldatareader2.Close();

                        if (flag2 == false)
                        {
                            SqlDataReader sqldatareader3 = sqlcommand3.ExecuteReader();
                            bool flag3 = sqldatareader3.Read();//上下三行，判断有没有读到这个手机
                            sqldatareader3.Close();

                            if (flag3 == false)
                            {
                                SqlDataReader sqldatareader4 = sqlcommand4.ExecuteReader();
                                bool flag4 = sqldatareader4.Read();//上下三行，判断有没有读到这个手机
                                sqldatareader4.Close();
                                if (flag4 == false)
                                {
                                    sqlcommand1.ExecuteNonQuery();
                                    SessionSG.SetSession("Users", Username);//设置session对象Users的值
                                    Response.Write("<script>alert('注册成功！已经为您自动登录');window.location.href='../Home/Index';</script>");//通过页面js的方法跳出弹窗，并跳转页面

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
                        sqlcommand1 = null;
                        sqlcommand2 = null;
                        sqlcommand3 = null;
                        sqlcommand4 = null;
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
        /// 功能-登录页 
        /// </summary>
        /// <returns></returns>
        public ActionResult Login(string Login_Btn_Click)
        {
            string TempPwd;
            string TempName;
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
                        TempPwd = sdrPhone.GetString(0);
                        TempName = sdrPhone.GetString(1);
                        sdrPhone.Close();
                        if (Userpwd.Equals(TempPwd))//成功登录
                        {
                            SessionSG.SetSession("Users", TempName);//设置session对象Users的值 为取到的用户名
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            Response.Write("<script>alert('密码错误')</script>");
                            return View();
                        }
                    }
                    else
                    {
                        Response.Write("<script>alert('你输入的账号不存在')</script>");
                        return View();
                    }
                }
                catch (Exception ee)
                {
                    Response.Write(ee);
                    return View();
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
        /// 功能-链接-注销
        /// 注销后回到首页
        /// </summary>
        public ActionResult Logout()
        {
            SessionSG.RemoveSession("Users");
            return RedirectToAction("Index");
        }
        /// <summary>
        /// 用户基本信息遍历，邮箱电话修改。和用户地址 遍历 修改 添加删除
        /// </summary>
        public ActionResult Imformation(string AddressCURD, string Alert_Phone, string Alert_Email)
        {
            string UserName;
            try
            {
                try
                {
                    string tryUserName = SessionSG.GetSession("Users").ToString();//一般就在这块儿报错了、 
                }
                catch (Exception ee) {
                    Response.Write("<script>alert('亲，请登录后再进行操作');window.location.href='../Home/Login'</script>");
                    Response.Write(ee.Message);
                }
                UserName = SessionSG.GetSession("Users").ToString();
                SqlConnection conn = Dbconn.getConnn();
                //修改基本信息-手机
                if (Alert_Phone == "修改") {
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
                            if (flag == false) {
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
                    SqlCommand alertSqlcommand = new SqlCommand();//sql语句，连接对象和字符串  用于修改
                    alertSqlcommand.Connection = conn;
                    alertSqlcommand.CommandText = @"Update  Addresses set Name=@Name,Phone=@Phone,Address=@Address where Id=@Id";
                    alertSqlcommand.Parameters.AddWithValue("@Name", Name);
                    alertSqlcommand.Parameters.AddWithValue("@Phone", Phone);
                    alertSqlcommand.Parameters.AddWithValue("@Address", Address);
                    alertSqlcommand.Parameters.AddWithValue("@Id", Id);
                    try
                    {
                        conn.Open();
                        alertSqlcommand.ExecuteNonQuery();
                    }
                    catch (Exception ee)
                    {
                        Response.Write(ee.Message);
                    }
                    finally
                    {
                        alertSqlcommand = null;
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
                        try
                        {
                            conn.Open();
                            deleteSqlcommand.ExecuteNonQuery();
                        }
                        catch (Exception deleteError)
                        {
                            Response.Write(deleteError.Message);
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
                sqlBaseInfo.CommandText = "select * from Users where UserName=@UserName";
                sqlBaseInfo.Parameters.AddWithValue("@Username", UserName);
                //查看收件地址
                SqlCommand sqlAddress = new SqlCommand();//(sql语句，连接对象和字符串)
                sqlAddress.Connection = conn;
                sqlAddress.CommandText = "select * from Addresses where UserName=@UserName";
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
                    return View();
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
                Response.Write(ee.Message);
            }

            return View();
        }
        /// <summary>
        /// 密码修改
        /// </summary>
        public ActionResult AlertUserPwd(string AlertPwd) {
            string UserName;
            try
            {
                string tryUserName = SessionSG.GetSession("Users").ToString();//一般就在这块儿报错了、 
            }
            catch (Exception ee)
            {
                Response.Write("<script>alert('亲，请登录后再进行操作');window.location.href='../Home/Login'</script>");
                Response.Write(ee.Message);
            }
            UserName = SessionSG.GetSession("Users").ToString();
            SqlConnection conn = Dbconn.getConnn();

            if (AlertPwd == "确认修改") {
                string NewPwd = Request["NewPwd"];
                string ConfirmPwd = Request["ConfirmPwd"];
                if (NewPwd.Equals(ConfirmPwd)) {
                    NewPwd = Models.Utils.HashUtil.GetSha256FromString(NewPwd);
                    SqlCommand SetNewPwd = new SqlCommand();
                    SetNewPwd.Connection = conn;
                    SetNewPwd.CommandText = @"update Users set UserPassword=@NewPwd where UserName=@UserName";
                    SetNewPwd.Parameters.AddWithValue("@NewPwd", NewPwd);
                    SetNewPwd.Parameters.AddWithValue("@UserName", UserName);//加参数的方法,用的是session中的UserName
                    try {
                        conn.Open();
                        SetNewPwd.ExecuteNonQuery();
                        SessionSG.RemoveSession("Users");
                        Response.Write("<script>alert('修改密码成功，请重新登录');window.location.href='../Home'</script>");
                    } catch (Exception ee) {
                        Response.Write(ee.Message);
                    } finally {
                        SetNewPwd = null;
                        conn.Close();
                        conn = null;

                    }
                }
                else { Response.Write("<script>alert(两次输入的密码不一致，请重新输入)</script>"); }
            }
            return View();
        }
        /// <summary>
        /// 购物车 点购买下单，先判断库存，足够则存到订单中
        /// </summary>
        public ActionResult Carts(){
            return View();
        }
        /// <summary>
        /// 我的订单遍历订单数据， 点取消订单，存到Dorder中，订单状态改为已下单，此时可以取消订单，
        /// 如果订单状态是派送中，无法取消订单。
        /// 需要填写原因，原因存到原因表reason中，订单数据存到dorder中
        /// //dorders(被取消的订单、配合退货原因,原因表存进去后，默认状态，0未查看)
        /// </summary>
        public ActionResult MyOrders()
        {
            return View();
        }
        /// <summary>
        /// 到底要不要分开填写，从页面上看吧
        /// </summary>
        //public ActionResult MyReason()
        //{
        //    return View();
        //}
        /// <summary>
        /// 留言板  看全体和个人  传过去两个数组得了，用js显示不同的
        /// </summary>
        public ActionResult Messages() {
            return View();
        }

        /// <summary>
        /// 库存给用户看，有多少东西
        ///在支付前获取，判断库存是否小于购买量。而不是直接显示。
        /// </summary>
        public ActionResult Products()
        {
            return View();
        }
    }
}