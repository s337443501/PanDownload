using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace PanDownload
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        HttpListener listener;

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = CommonHelper.Read("PanDownload", "Cookie");
                string StartCheck = CommonHelper.Read("PanDownload", "StartCheck");
                if (!string.IsNullOrEmpty(StartCheck))
                {
                    checkBox1.Checked = Convert.ToBoolean(StartCheck);
                }

                //提供一个简单的、可通过编程方式控制的 HTTP 协议侦听器。此类不能被继承。
                listener = new HttpListener();
                //指定身份验证 Anonymous匿名访问
                listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                //定义url及端口号，通常设置为配置文件
                listener.Prefixes.Add("http://127.0.0.1:786/");
                //启动监听器
                listener.Start();
                //异步监听客户端请求，当客户端的网络请求到来时会自动执行Result委托
                //该委托没有返回值，有一个IAsyncResult接口的参数，可通过该参数获取context对象
                listener.BeginGetContext(Result, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CommonHelper.BrowserStartUrl("http://tool.yijingying.com/pandownload/");
                FormClose();
            }
        }

        #region 异步监听
        private void Result(IAsyncResult ar)
        {
            try
            {
                //当接收到请求后程序流会走到这里

                //继续异步监听
                listener.BeginGetContext(Result, null);

                //获得context对象
                var endcontext = listener.EndGetContext(ar);
                var request = endcontext.Request;
                Console.WriteLine(request.Url);
                Console.WriteLine(request.RawUrl);
                Console.WriteLine();
                var response = endcontext.Response;

                //如果是js的ajax请求，还可以设置跨域的ip地址与参数
                endcontext.Response.AppendHeader("Access-Control-Allow-Origin", "*");//后台跨域请求，通常设置为配置文件
                endcontext.Response.AppendHeader("Access-Control-Allow-Headers", "ID,PW");//后台跨域参数设置，通常设置为配置文件
                endcontext.Response.AppendHeader("Access-Control-Allow-Method", "post");//后台跨域请求设置，通常设置为配置文件
                endcontext.Response.ContentType = "text/plain;charset=UTF-8";//告诉客户端返回的ContentType类型为纯文本格式，编码为UTF-8
                endcontext.Response.AddHeader("Content-type", "text/plain");//添加响应头信息
                endcontext.Response.ContentEncoding = Encoding.UTF8;
                string returnObj = null; //定义返回客户端的信息
                returnObj = HandleRequest(request, response); //处理客户端发送的请求并返回处理信息
                byte[] returnByteArr = null;
                if (!string.IsNullOrEmpty(returnObj))
                {
                    returnByteArr = Encoding.UTF8.GetBytes(returnObj);//设置客户端返回信息的编码
                }
                using (var stream = response.OutputStream)
                {
                    if (returnByteArr != null)
                    {
                        //把处理信息返回到客户端
                        stream.Write(returnByteArr, 0, returnByteArr.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        #endregion

        #region 数据返回
        private string HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                response.StatusDescription = "200"; //获取或设置返回给客户端的 HTTP 状态代码的文本说明。
                response.StatusCode = 200; //获取或设置返回给客户端的 HTTP 状态代码。

                var urlPath = request.Url.PathAndQuery;
                if (urlPath.StartsWith("/w/api/init"))
                {
                    return "{\"srecord\":{\"autoQuery\":true},\"loginurl\":{\"url\":\"http://pandownload.com/bdlogin.html\"},\"wke\":\"http://dl.pandownload.club/dl/node-190312.dll\",\"pcscfg\":{\"appid\":250528,\"ua\":\"\",\"ct\":0},\"flag\":1,\"ad\":{\"url\":\"https://pandownload.com/donate.html\",\"image\":\"http://pandownload.com/images/donate.png\",\"attribute\":\"width=\\\"88\\\" height=\\\"100\\\" padding=\\\"0,0,5,0\\\"\",\"rand\":100},\"bdc\":[\"aF5yaHQQfQt3E3lxdHdjVmF4f3hae3JsfFxIbGxbW152FHF0anpdW3R5UHR1H1hxdwdoTGRMcXR1THsRbF92XXJbdmpveF93Yl9TE3dod3tYaXYIc1oRXGZKbVR1aGkGcV57dmleen1wEAYEdnV9BXF3BQFxd3sCaGh2BHB0BgdpW3F2c3pbAXF3ewJoaAYEcHQGBHZ1fQVxdwUBcXd7AmhoBgRwdAYEdnV9BXF3BQFxd3sCaGgGBHB0BgR2dX0FcXcFAXF3ewJoaAYEcHQGBHZ1fQVxdwUBcXd7AmhoBgRwdAYEdnV9BXF3BQFxd3sCaGgGBHB0BgRpB21SYXYFFH0UDmpben5afkgCdmsFAH9ydxMIflxqEnUHWxVpdgUTcwdsTH0UcBBpdndddV1+XWh2altzWlpLaVxbWH1cXEh1XWMXfAZ5FnR0fV99aXlcagZrWHUHaRZ9EXVJcnRTXH1MYhB9BlFddVoNDw==\"],\"timestamp\":1586998638,\"code\":0,\"message\":\"success\"}";
                }
                else if (urlPath.StartsWith("/w/api/script/list"))
                {
                    return "{\"scripts\":[{\"name\":\"search_pandown.lua\",\"remove\":true},{\"name\":\"search_ncckl.lua\",\"remove\":true},{\"name\":\"search_quzhuanpan.lua\",\"remove\":true},{\"name\":\"anime_01.lua\",\"remove\":true},{\"name\":\"anime_02.lua\",\"remove\":true},{\"name\":\"anime_dilidili.lua\",\"remove\":true},{\"name\":\"anime\",\"remove\":true},{\"name\":\"default\",\"remove\":true},{\"name\":\"s\",\"id\":2,\"url\":\"http://pandownload.com/static/scripts/s008\",\"md5\":\"8dfd9a6c08d06bec27ae358f315cca8f\"},{\"name\":\"download_pcs.lua\",\"id\":1000,\"url\":\"http://pandownload.com/static/scripts/download_pcs.lua\",\"md5\":\"38770cd3e9bcd62f7212941b51ca1378\"}],\"code\":0,\"message\":\"success\"}";
                }
                else if (urlPath.StartsWith("/w/api/latest"))
                {
                    return "{\"version\":\"2.2.6\",\"url\":\"\",\"web\":\"\",\"detail\":\"\",\"md5\":\"35C4E9BEB2B6EA1D496E7001A60C1839\",\"code\":0,\"message\":\"success\"}";
                }
                else if (urlPath.StartsWith("/api/GetCookie.php"))
                {
                    return textBox1.Text.Trim();
                }
                else
                {
                    return null;

                    #region 软件代理其他网址
                    //var url = request.RawUrl;
                    //if (url == "http://127.0.0.1:9347/disk/home")
                    //{
                    //    return "<script type=\"text/javascript\">\r\ntypeof initPrefetch === 'function' && initPrefetch('977251abc3540c83b6d895ea520de953', '');\r\n</script>\r\n";
                    //}
                    //var cookie = request.Headers["Cookie"];

                    //var postData = "";
                    //if (request.HttpMethod == "POST" && request.InputStream != null)
                    //{
                    //    var byteList = new List<byte>();
                    //    var byteArr = new byte[2048];
                    //    int readLen = 0;
                    //    int len = 0;
                    //    //接收客户端传过来的数据并转成字符串类型
                    //    do
                    //    {
                    //        readLen = request.InputStream.Read(byteArr, 0, byteArr.Length);
                    //        len += readLen;
                    //        byteList.AddRange(byteArr);
                    //    } while (readLen != 0);
                    //    postData = Encoding.UTF8.GetString(byteList.ToArray(), 0, len);

                    //    //获取得到数据data可以进行其他操作
                    //}

                    //HttpHelper http = new HttpHelper();
                    //HttpItem item = new HttpItem()
                    //{
                    //    URL = url,//URL     必需项
                    //    Encoding = request.ContentEncoding,//编码格式（utf-8,gb2312,gbk）     可选项 默认类会自动识别
                    //    Method = request.HttpMethod,//URL     可选项 默认为Get
                    //    Timeout = 100000,//连接超时时间     可选项默认为100000
                    //    ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000
                    //    IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写
                    //    Cookie = cookie,//字符串Cookie     可选项
                    //    UserAgent = request.UserAgent,//用户的浏览器类型，版本，操作系统     可选项有默认值
                    //    Accept = "text/html, application/xhtml+xml, */*",//    可选项有默认值
                    //    ContentType = request.ContentType,//返回类型    可选项有默认值
                    //    Allowautoredirect = true,//是否根据３０１跳转     可选项
                    //    Connectionlimit = 1024,//最大连接数     可选项 默认为1024
                    //    Postdata = postData,//Post数据     可选项GET时不需要写
                    //    PostDataType = PostDataType.String,//默认为传入String类型，也可以设置PostDataType.Byte传入Byte类型数据
                    //    ResultType = ResultType.String,//返回数据类型，是Byte还是String
                    //};
                    ////得到HTML代码
                    //HttpResult result = http.GetHtml(item);

                    ////返回的Html内容
                    //return result.Html;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                response.StatusDescription = "404";
                response.StatusCode = 404;
                return null;
            }
        }
        #endregion

        #region 软件关闭最小化至托盘菜单
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormClose();
        }

        public void FormClose()
        {
            notifyIcon1.Visible = false;
            int ProcessById = Process.GetCurrentProcess().Id;
            Process exit = Process.GetProcessById(ProcessById);
            exit.Kill();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.Activate();
        }
        #endregion

        #region 启动 PanDownload
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo pinfo = new ProcessStartInfo();
                pinfo.UseShellExecute = true;
                pinfo.FileName = "PanDownload.exe";
                Process.Start(pinfo);

                this.ShowInTaskbar = false;
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CommonHelper.Write("PanDownload", "StartCheck", checkBox1.Checked.ToString());
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            CommonHelper.Write("PanDownload", "Cookie", textBox1.Text.Trim());
        }
    }
}
