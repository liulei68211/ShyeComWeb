using GTHYWebservice;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using webJm.ApiMode;
using System.Data.SqlClient;
using ShyeComWeb.Model;
using System.Reflection;
using System.Text;
using System.IO;
using System.Net;
using System.Collections;
using Baidu.Aip.Ocr;
namespace ShyeComWeb
{
    /// <summary>
    /// syWebService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
   /*最新更新时间 219-08-01  接收派车单信息时 更新本地货运单bclose = 3*/
    public class syWebService : System.Web.Services.WebService
    {
        private dataInfoModel datainfos = new dataInfoModel();
        private SqlTransaction SqlTran;

        /// <summary>
        /// json字符串格式化
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string ConvertJsonString(string str)
        {
            //格式化json字符串
            JsonSerializer serializer = new JsonSerializer();
            TextReader tr = new StringReader(str);
            JsonTextReader jtr = new JsonTextReader(tr);
            object obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter textWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }
            else
            {
                return str;
            }
        }

        #region 金马已拍照信息 
        [WebMethod(Description = "查询已拍照信息")]
        public string searchTempInfo(string gateCode)
        {
            DBoperationcs dbop = new DBoperationcs();
            CMoperation cmop = new CMoperation();
            string returnXml = "";
            try
            {
                string selectSql = "select a.pk_NumInfo , a.C_CarryNo ,a.C_DlgtUser , a.ts ," +
                 "a.C_TelePhone ,(case when a.pk_ClassType = 1 then '采购' when a.pk_ClassType =  5 then '销售' end) 业务类型, " +
                 "a.C_FactoryID , c.cCustName ,a.C_MateriID , b.cInvName ,a.Accid , e.cAccName  " +
                 "from CM_NumInfoTemp a  " +
                 "left join EI_InvbasDoc b on a.C_MateriID=b.pk_invbasdoc " +
                 "left join  CB_cubasdoc c on c.pk_cubasdoc=a.C_FactoryID " +
                 "left join RI_AccID e on e.iAccID=a.Accid " +
                 "where a.def3 = 0  and  GateCode = '" + gateCode + "' order by a.ts";
                DataTable dt = dbop.GetTable(selectSql);
                if (dt.Rows.Count > 0)
                {
                    //table 转xml
                    returnXml = cmop.ConvertDataTableToXML(dt, "");
                }
            }
            catch (Exception ex)
            {

            }
            return returnXml;
        }
        #endregion

        #region 金马叫号布信息 
        [WebMethod(Description = "查询已叫号信息")]
        public string searchTempInfoB(string gateCode)
        {
            DBoperationcs dbop = new DBoperationcs();
            CMoperation cmop = new CMoperation();
            string returnXml = "";
            try
            {
                string selecSql = "select a.orderID ,a.pk_NumInfo,a.C_CarryNo ,a.C_DlgtUser ,a.ts,a.C_TelePhone,"+
                                  "(case when a.pk_ClassType = 1 then '采购' when a.pk_ClassType = 5 then '销售' end) 业务类型," +
                                  "a.C_FactoryID,c.cCustName , a.C_MateriID ,b.cInvName ,a.ts, a.Accid," +
                                  "e.cAccName from CM_NumInfoTemp a " +
                                  "left join CM_MeasureInfo f on a.cMeaID = f.C_BL_No " +
                                  "left join CM_MeasTemp g on g.C_MeasID = a.cMeaID " +
                                  "left join EI_InvbasDoc b on a.C_MateriID = b.pk_invbasdoc " +
                                  "left join CB_cubasdoc c on c.pk_cubasdoc = a.C_FactoryID  " +
                                  "left join RI_AccID e on e.iAccID = a.Accid   where a.def3 = 5 " +
                                  "and a.GateCode = '"+gateCode+"' and (f.C_Status = 0 or f.C_Status is null) " +
                                  "and (g.bFinish = 0) order by a.ts";
                DataTable dt = dbop.GetTable(selecSql);
                if (dt.Rows.Count > 0)
                {
                    //table 转xml
                    returnXml = cmop.ConvertDataTableToXML(dt, "");
                }
            }
            catch (Exception ex)
            {

            }
            return returnXml;
        }
        #endregion

        #region 金马排队 前门岗
        [WebMethod(Description ="查询已拍照信息 临时表 前门岗")]
        public string searchTempInfoFront(string gateCode,string  accCode  )
        {
            DBoperationcs dbop = new DBoperationcs();
            CMoperation cmop = new CMoperation();
            string returnXml = "";
            try
            {
                string selectSql = "select a.pk_NumInfo , a.C_CarryNo ,a.C_DlgtUser , a.ts ," +
                 "a.C_TelePhone ,(case when a.pk_ClassType = 1 then '采购' when a.pk_ClassType =  5 then '销售' end) 业务类型, " +
                 "a.C_FactoryID , c.cCustName ,a.C_MateriID , b.cInvName ,a.Accid , e.cAccName  " +
                 "from CM_NumInfoTemp a  " +
                 "left join EI_InvbasDoc b on a.C_MateriID=b.pk_invbasdoc " +
                 "left join  CB_cubasdoc c on c.pk_cubasdoc=a.C_FactoryID " +
                 "left join RI_AccID e on e.iAccID=a.Accid " +
                 "where a.def3 = 0 and a.cAccCode='" + accCode + "' and  GateCode = '" + gateCode + "' order by a.ts";
                DataTable dt= dbop.GetTable(selectSql);
                if (dt.Rows.Count > 0)
                {
                    //table 转xml
                    returnXml = cmop.ConvertDataTableToXML(dt, "");
                }
            }
            catch (Exception ex)
            {

            }
            return returnXml;
        }
        #endregion

        #region 金马排队 后门岗 (前门岗 洗煤 焦炭)
        [WebMethod(Description = "查询已拍照信息 临时表 后门岗")]
        public string searchTempInfoBack(string gateCode, int invID )
        {
            DBoperationcs dbop = new DBoperationcs();
            CMoperation cmop = new CMoperation();
            string returnXml = "";
            try
            {
                string selectSql = "select a.pk_NumInfo , a.C_CarryNo ,a.C_DlgtUser , a.ts ," +
                 "a.C_TelePhone ,(case when a.pk_ClassType = 1 then '采购' when a.pk_ClassType =  5 then '销售' end) 业务类型, " +
                 "a.C_FactoryID , c.cCustName ,a.C_MateriID , b.cInvName ,a.Accid , e.cAccName  " +
                 "from CM_NumInfoTemp a  " +
                 "left join EI_InvbasDoc b on a.C_MateriID=b.pk_invbasdoc " +
                 "left join  CB_cubasdoc c on c.pk_cubasdoc=a.C_FactoryID " +
                 "left join RI_AccID e on e.iAccID=a.Accid " +
                 "where a.def3 = 0 and a.C_MateriID= " + invID + " and  GateCode = '" + gateCode + "' order by a.ts";
                DataTable dt = dbop.GetTable(selectSql);
                if (dt.Rows.Count > 0)
                {
                    //table 转xml
                    returnXml = cmop.ConvertDataTableToXML(dt, "");
                }
            }
            catch (Exception ex)
            {

            }
            return returnXml;
        }
        #endregion

 
        #region 叫号
        [WebMethod(Description ="叫号")]
        public bool subUpdate(string idArr)
        {
            DBoperationcs dbop = new DBoperationcs();
            CMoperation cm = new CMoperation();
            bool returnStr = false;

           idArr = idArr.TrimEnd(',');
           string[] idArrs = idArr.Split(',');
            try
            {
                for (int i=0;i< idArrs.Length;i++)
                {
                    //生成排队号
                    string orderCode = cm.GetMeasureOrder();
                    string updateNum = "update CM_NumInfoTemp set def3 = 5,orderID = '" + orderCode + "' where pk_NumInfo = " + idArrs[i] + "";
                    dbop.getsqlcom(updateNum);
                }
                //查询已发布信息
                string sql = "select orderID,C_CarryNo 车号,C_TelePhone 电话,GateCode " +
                             "from CM_NumInfoTemp  where  pk_NumInfo in(" + idArr + ") order by orderID";
                DataTable dt = dbop.GetTable(sql);
                if (dt.Rows.Count >0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        
                        string gateCode = "";
                        if (dt.Rows[i]["GateCode"].ToString() == "01")
                        {
                            gateCode = "0";
                        }
                        else
                        {
                            gateCode = "1";
                        }
                        //调用微信消息接口 发布消息
                        string para = dt.Rows[i]["车号"].ToString() + "|" + dt.Rows[i]["电话"].ToString() + "|" + (i+1).ToString() + "|" + gateCode + "|"+1;
                        //string url = "http://gthy.guotaiyun.cn/CM/JMCar?para=" + para;  //你需要访问的 web 地址
                        string url = "http://gthy.guotaiyun.cn/CM/JMCall?para=" + para;  //你需要访问的 web 地址
                        System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
                        request.Timeout = 6000;
                        System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
                        System.IO.Stream rStream = response.GetResponseStream();
                        System.IO.StreamReader sr = new System.IO.StreamReader(rStream, Encoding.UTF8); //如果内容乱码，试着修改 Encoding.UTF8 枚举
                        StringBuilder strBuilder = new StringBuilder();
                        strBuilder.Append(sr.ReadToEnd());
                        string result = strBuilder.ToString();
                        if (result == "1")
                        {
                            returnStr = true;
                        }
                        else
                        {
                            returnStr = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return returnStr;
        }
        #endregion

        #region 微信接口
        [WebMethod(Description = "微信接口")]
        public bool wechatSend(string idArr)
        {
            bool returnStr = false;
            idArr = idArr.TrimEnd(',');
            string[] idArrs = idArr.Split(',');
            try
            {

                //调用微信消息接口 发布消息
                string para = "豫U19693" + "|" + "17603902692"+ "|" + "20200318001" + "|" + 1;
                string url = "http://gthy.guotaiyun.cn/CM/JMCar?para=" + para;  //你需要访问的 web 地址
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
                request.Timeout = 6000;
                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
                System.IO.Stream rStream = response.GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(rStream, Encoding.UTF8); //如果内容乱码，试着修改 Encoding.UTF8 枚举
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.Append(sr.ReadToEnd());
                string result = strBuilder.ToString();
                if (result == "1")
                {
                    returnStr = true;
                }
                else
                {
                    returnStr = false;
                }
            }
            catch (Exception ex)
            {

            }
            return returnStr;
        }
        #endregion

        #region 百度云车号识别 后门岗
        [WebMethod(Description = "百度云车号识别")]
        public string carNumIdentification(byte[] image)
        {
            //string url = "https://aip.baidubce.com/rest/2.0/ocr/v1/license_plate";
            string result = "";
             //JsonResult ajxares = new JsonResult();
             //ajxares.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
             //二进制流转byte[]
             // byte[] image = Encoding.UTF8.GetBytes(images);
             ArrayList list = new ArrayList();
            string clientId = "yf3TW6DbsxsGt2aRxpFMFxGf";
            // 百度云中开通对应服务应用的 Secret Key
            string clientSecret = "aIQPt204EaKp7vswVDppiQhouUHQ0kWk";
            //var image = System.IO.File.ReadAllBytes("图片文件路径");
            ////// 调用车牌识别，可能会抛出网络等异常，请使用try/catch捕获
            ////var result = client.LicensePlate(image);
            //Byte[] image = System.IO.File.ReadAllBytes("F://1.jpg");
            var client = new Ocr(clientId, clientSecret);
            try
            {
                 result = client.PlateLicense(image).ToString();
                //WriteID(result, "baiud");
            }
            catch (Exception ex)
            {
               // WriteYCLog(ex,"dddd");
            }
           
            return result;
        }
        #endregion

        #region 百度云车号识别前门岗
        [WebMethod(Description = "百度云车号识别2")]
        public string carNumIdentificationFront(byte[] image)
        {
            //string url = "https://aip.baidubce.com/rest/2.0/ocr/v1/license_plate";
            string result = "";
            //JsonResult ajxares = new JsonResult();
            //ajxares.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //二进制流转byte[]
            // byte[] image = Encoding.UTF8.GetBytes(images);
            ArrayList list = new ArrayList();
            // string clientId = "yf3TW6DbsxsGt2aRxpFMFxGf"; 
            string clientId = "GzyENu76DzKVLuxf5X3VwkME";
            // 百度云中开通对应服务应用的 Secret Key
            //string clientSecret = "aIQPt204EaKp7vswVDppiQhouUHQ0kWk";
            string clientSecret = "i0E6hxgzOOj3Q9aNtWpFqTpW5mp5ZmuY";
            //var image = System.IO.File.ReadAllBytes("图片文件路径");
            ////// 调用车牌识别，可能会抛出网络等异常，请使用try/catch捕获
            ////var result = client.LicensePlate(image);
            //Byte[] image = System.IO.File.ReadAllBytes("F://1.jpg");
            var client = new Ocr(clientId, clientSecret);
            try
            {
                result = client.PlateLicense(image).ToString();
                //WriteID(result, "baiud");
            }
            catch (Exception ex)
            {
                // WriteYCLog(ex,"dddd");
            }

            return result;
        }
        #endregion

        #region 保存派车单信息到排队数据表
        [WebMethod(Description = "保存派车单信息到排队数据表")]
        public int saveCarInfo(string carCode, string gateCode)
        {
            DBoperationcs dbop = new DBoperationcs();
            CMoperation cm = new CMoperation();
            int result = 0;
            string web_time = webTime();
          
            //查询派车单信息
            DataTable dt = dispathIndo(carCode);
            if (dt.Rows.Count > 0)
            {
                //判断派车单是否已经添加到排队表中
                string selectCar = "select C_CarryNo from CM_NumInfoTemp where cMeaID = '" + dt.Rows[0]["C_MeasID"] + "'";
                DataTable dtExit = dbop.GetTable(selectCar);
                if (dtExit.Rows.Count > 0)
                {
                    //已经排队
                    result = 11;
                }
                else
                {
                    //生成排队号
                    string orderCode = cm.GetMeasureOrder();
                    string insertSql = "insert into CM_NumInfoTemp(cAccCode,cMeaID,C_CarryNo,pk_ClassType,C_MateriID,C_FactoryID,C_DlgtUser,C_TelePhone,GateCode,I_Store_id,ts,Accid,orderID) " +
                                "values ('" + dt.Rows[0]["cAccCode"] + "','" + dt.Rows[0]["C_MeasID"] + "','" + dt.Rows[0]["cCarNum"] + "','" + dt.Rows[0]["C_ClassType"] + "', " +
                                "'" + dt.Rows[0]["pk_invbasdoc"] + "','" + dt.Rows[0]["pk_cubasdoc"] + "', " +
                                "'" + dt.Rows[0]["cDriver"] + "','" + dt.Rows[0]["cDriverPhone"] + "','" + gateCode + "',1,'" + web_time + "','" + dt.Rows[0]["pk_acc"] + "','"+orderCode+"')";
                    //dbop.getsqlcom(insertSql);
                    if (dbop.getsqlcom(insertSql) > 0)
                    {
                        if (gateCode == "01")
                        {
                            gateCode = "0";
                        }
                        if (gateCode == "02") 
                        {
                            gateCode = "1";
                        }
                        //调用微信消息接口 发布消息
                        string para = carCode + "|" + dt.Rows[0]["cDriverPhone"] + "|" + orderCode + "|" + gateCode;
                        string url = "http://gthy.guotaiyun.cn/CM/JMCar?para=" + para;  //你需要访问的 web 地址
                        System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
                        request.Timeout = 6000;
                        System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
                        System.IO.Stream rStream = response.GetResponseStream();
                        System.IO.StreamReader sr = new System.IO.StreamReader(rStream, Encoding.UTF8); //如果内容乱码，试着修改 Encoding.UTF8 枚举
                        StringBuilder strBuilder = new StringBuilder();
                        strBuilder.Append(sr.ReadToEnd());
                        string results = strBuilder.ToString();
                        if (results == "1")
                        {
                            result = 1;
                        }
                        else
                        {
                            result = 0;
                        }

                        //调用短信接口
                        
                    }
                }
            }
            else
            {
                result = 0;
            }

            return result;
        }
        #endregion
        #region 查询排队信息
        [WebMethod(Description = "查询排队信息")]
        public string lineUpInfo(string doorCode)
        {
            string returnXml = "";
            DBoperationcs dbop = new DBoperationcs();
            CMoperation cmop = new CMoperation();
            string web_time = webTime().Substring(0, 10);
            string selectSql = "select a.pk_NumInfo,a.C_CarryNo,a.ts,b.cInvName  from CM_NumInfoTemp a " +
                                "left join EI_InvbasDoc b on a.C_MateriID=b.pk_invbasdoc " +
                               "where a.def3 = 0  and a.GateCode = '" + doorCode + "' " +
                               "order by a.pk_NumInfo desc";
            DataTable dt = dbop.GetTable(selectSql);
            if (dt.Rows.Count > 0)
            {
                //table 转xml
                returnXml = cmop.ConvertDataTableToXML(dt, "");
            }
            else
            {
                returnXml = "null";
            }
            return returnXml;
        }
        #endregion
        #region 查询金马派车单信息
        private DataTable dispathIndo(string carCode)
        {
            DBoperationcs dbop = new DBoperationcs();
            DataTable dt = null;
            string web_time = webTime().Substring(0, 10);
            try
            {
                string selectSql = "select C_MeasID,cCarNum,C_ClassType,pk_invbasdoc,pk_cubasdoc,C_MeasID,cDriver,cDriverPhone,pk_Acc,cAccCode from CM_MeasTemp where  cCarNum  = '" + carCode + "' " +
                                "and (bRead = 0 and  bFinish = 0) order by autoid desc";
                dt = dbop.GetTable(selectSql);
            }
            catch (Exception ex)
            {
                dt = null;
            }
            return dt;
        }
        #endregion
        #region 查询手持用户信息
        [WebMethod(Description = "查询用户信息手持")]
        public string selectAppUserJM(string userCode, string passWord)
        {
            string doorCode = "";
            DBoperationcs dbop = new DBoperationcs();
            string selectSql = "select doorCode,cUserName from CB_AppUser where cUserCode = '" + userCode + "' and cPassWord = '" + passWord + "'";
            DataTable dt = dbop.GetTable(selectSql);
            if (dt.Rows.Count > 0)
            {
                doorCode = dt.Rows[0]["doorCode"].ToString(); ;
            }

            return doorCode;
        }
        #endregion

        #region post数据
        private static string Post(string Url, string Data)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            //请求内容为json格式
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] bytes = Encoding.UTF8.GetBytes(Data);
            //请求内容为form表单
            // request.ContentType = "application/x-www-form-urlencoded";//‘
            request.ContentLength = bytes.Length;
            //字符流 发送请求数据
            Stream myResponseStream = request.GetRequestStream();
            myResponseStream.Write(bytes, 0, bytes.Length);
            //接收返回数据
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader myStreamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();

            myStreamReader.Close();
            myResponseStream.Close();

            if (response != null)
            {
                response.Close();
            }
            if (request != null)
            {
                request.Abort();
            }
            return retString;
        }
        #endregion

        //版本最新 2019-07-05 11:05
        [WebMethod(Description = "查询服务器时间")]
        public string webTime()
        {
            DBoperationcs dbop = new DBoperationcs();
            string web_time = "";
            string select_sql = "select convert(varchar,getdate(),120)";
            DataTable dt_time = dbop.GetTable(select_sql);
            if (dt_time.Rows.Count > 0)
            {
                web_time = dt_time.Rows[0][0].ToString();
            }
            return web_time;
        }
        [WebMethod(Description = "上传货运单到国泰衡云")]
        public string sendHydInfo(string vouchNum)
        {
            string result = "false";
            string accCode = "";
            string selectSql = "";
            WebReference.WebService1 webSer = new WebReference.WebService1();
            DBoperationcs syDbop = new DBoperationcs();
            JObject requestObj = new JObject();
            JArray arrayEV = new JArray();
            #region 查询账套号 cAcc_Id测试 006  正式 001
            string select_acc = "select cHYAccId from EDP_SYS.dbo.ES_Account where cAcc_Id = '001'";
            DataTable dt_acc = syDbop.GetTable(select_acc);
            if (dt_acc.Rows.Count > 0)
            {
                accCode = dt_acc.Rows[0]["cHYAccId"].ToString();
            }
            #endregion
            //判断货运单是否已上传
            if (vouchNum.Substring(0,2) == "SL")
            {
                //销售
                selectSql = "select a.cAccCode,a.PLhid,a.cPLCode,a.Factory,a.SendAddress,a.iMaker,a.dNewDate,a.ts,a.iSendPerson,a.dSendDate,b.PLbid,b.pk_invbasdoc,b.fQuantity,a.iAccID,a.cU8ID,a.cDLCode,a.cSLCode,a.cCarCode  " +
                                    "from EV_CargoSPlan_h a left join EV_CargoSPlan_b b on a.PLhid = b.PLhid " +
                                    "where a.cPLCode = '" + vouchNum + "' ";
            }
            if (vouchNum.Substring(0, 2) == "PL")
            {
                //采购
                selectSql = "select a.cAccCode,a.PLhid,a.cPLCode,a.Factory,a.SendAddress,a.iMaker,a.dNewDate,a.ts,a.iSendPerson,a.dSendDate,b.PLbid,b.pk_invbasdoc,b.fQuantity,a.iAccID,a.cU8ID  " +
                            "from EV_CargoPlan_h a left join EV_CargoPlan_b b on a.PLhid = b.PLhid " +
                            "where a.cPLCode = '" + vouchNum + "' ";
            }

            DataTable dt = syDbop.GetTable(selectSql);
            if (dt.Rows.Count > 0)
            {
                datainfos.accCode = accCode;//公司账套号
                datainfos.subAccCode = dt.Rows[0]["cAccCode"].ToString();//子公司账套号
                datainfos.accID = Convert.ToInt32(dt.Rows[0]["iAccID"].ToString());//子公司账套号主键
                datainfos.plCode = dt.Rows[0]["cPLCode"].ToString();
                datainfos.factoryId = Convert.ToInt32(dt.Rows[0]["Factory"].ToString());
                //datainfos.adressId = Convert.ToInt32(dt.Rows[0]["SendAddress"].ToString());
                //datainfos.sendTime = dt.Rows[0]["SendTime"].ToString();
                datainfos.maker = Convert.ToInt32(dt.Rows[0]["iMaker"].ToString());
                datainfos.newDate = dt.Rows[0]["dNewDate"].ToString();
                datainfos.ts = dt.Rows[0]["ts"].ToString();
                datainfos.plHid = Convert.ToInt32(dt.Rows[0]["PLhid"].ToString());
                datainfos.cU8Code = dt.Rows[0]["cU8ID"].ToString();
                if (vouchNum.Substring(0, 2) == "SL")
                {
                    datainfos.dlCode = dt.Rows[0]["cDLCode"].ToString();
                    datainfos.slCode = dt.Rows[0]["cSLCode"].ToString();
                    datainfos.carcode = dt.Rows[0]["cCarCode"].ToString();
                }
                datainfos.sendPer = dt.Rows[0]["iSendPerson"].ToString();
                datainfos.sendTm = dt.Rows[0]["dSendDate"].ToString();
               
                var dataInfo = new JObject {
                        {"PLhid", datainfos.plHid },
                        {"cAccCode",accCode},
                        {"cAccID",datainfos.accID},
                        {"cSubAccCode",datainfos.subAccCode},
                        {"cPLCode",datainfos.plCode},
                        {"Factory", datainfos.factoryId},
                        {"SendAddress",datainfos.adressId},
                        {"iMaker",datainfos.maker},
                        {"dNewData",datainfos.newDate},
                        {"ts",datainfos.ts},
                        {"cU8Code",datainfos.cU8Code},
                        {"cDLCode",datainfos.dlCode},
                        {"cSLCode",datainfos.slCode},
                        {"senPer",datainfos.sendPer},
                        {"sendTm",datainfos.sendTm},
                        {"carCode",datainfos.carcode},
                };
                //子表 数组
                for (int i =0;i<dt.Rows.Count;i++)
                {
                    datainfos.invBasdoc = Convert.ToInt32(dt.Rows[0]["pk_invbasdoc"].ToString());
                    datainfos.fQuantity = dt.Rows[0]["fQuantity"].ToString();
                    datainfos.pkBid = Convert.ToInt32(dt.Rows[0]["PLbid"].ToString());
                    datainfos.plHid = Convert.ToInt32(dt.Rows[0]["PLhid"].ToString());

                    var dataInfoB = new JObject
                    {
                        {"PLhid", datainfos.plHid },
                        {"PLbid", datainfos.pkBid },
                        {"pk_invbasdoc",datainfos.invBasdoc},
                        {"fQuantity",datainfos.fQuantity},
                    };
                    arrayEV.Add(dataInfoB);
                }
                requestObj.Add("dataInfo", dataInfo);
                requestObj.Add("dataInfoB", arrayEV);
            
                string request = JsonConvert.SerializeObject(requestObj);

                string response = webSer.revShipList(request);
                //获取json字符串中的数据
                JObject returnModel = (JObject)JsonConvert.DeserializeObject(response);
                string returnMessage = returnModel["head"]["message"].ToString();
                string returnSuccess = returnModel["head"]["success"].ToString();
                if (returnMessage == "操作成功")
                {
                    result = "True";
                }
                else
                {
                    result = "false";
                }
            }
            return result;
        }

        [WebMethod(Description ="上传货运单到云工物流")]
        public string sendHydInfo56(string vouchCode, string vouchNum)
        {
            DBoperationcs dbop = new DBoperationcs();
            string update_sql = "";
            string result = "";
            string returnInfor = "";
            //string url = "http://testapi.56yongche.com/tmsapi/json/newJMDZgoodsSend.jsp";//测试
            string url = "http://api.56yongche.com/tmsapi/json/newJMDZgoodsSend.jsp";//正式
            JObject requestObj = new JObject();
            loginInfoModel logininfos = new loginInfoModel();
            dataInfo56Model datainfos56 = new dataInfo56Model();

            //查询货运单信息
            DataTable dt_hydan = selectPurInfo(vouchCode, vouchNum);
            if (dt_hydan.Rows.Count > 0)
            {
                datainfos56.sender = dt_hydan.Rows[0]["发货公司"].ToString();
                datainfos56.goodTypeDes = dt_hydan.Rows[0]["存货名称"].ToString();
                datainfos56.amount = dt_hydan.Rows[0]["订单总价"].ToString();
                datainfos56.goodsPrice = dt_hydan.Rows[0]["单价"].ToString();
                datainfos56.weight = dt_hydan.Rows[0]["重量"].ToString();
                datainfos56.remark = dt_hydan.Rows[0]["备注"].ToString();
                datainfos56.orderNum = dt_hydan.Rows[0]["订单号"].ToString();
                datainfos56.ret1 = "http://123.7.18.41";
                datainfos56.prodTypeDesc = "";
                datainfos56.qty = dt_hydan.Rows[0]["数量"].ToString();
                datainfos56.senderMobile = dt_hydan.Rows[0]["发货人手机号"].ToString();
                datainfos56.logisticsMark = dt_hydan.Rows[0]["单据类型"].ToString();
                datainfos56.ret2 = "";
                datainfos56.ret3 = "";
                datainfos56.ret4 = "";
                datainfos56.ret5 = "";
                datainfos56.ret6 = "";
                datainfos56.ret7 = "";
                datainfos56.ret8 = "";
                datainfos56.ret9 = "";
                datainfos56.ret10 = "";

                //查询56平台账套信息
                string selectSql = "select accountName,accountPass from CM_WLAccount where vendorName = '"+ datainfos56.sender + "'";
                DataTable dt = dbop.GetTable(selectSql);
                if (dt.Rows.Count > 0)
                {
                    logininfos.customerid = dt.Rows[0]["accountName"].ToString();
                    logininfos.password = dt.Rows[0]["accountPass"].ToString();
                    //查询供应商信息
                    // datainfos56.senderID = selectIdCus(datainfos56.sender);
                    //查询存货信息
                    //   datainfos56.invId = selectIdInv(datainfos56.goodTypeDes);
                    var obj = new JObject();
                    var loginInfo = new JObject { { "customerId", logininfos.customerid }, { "password", logininfos.password } };
                    var dataInfo = new JObject {
                        {"sender",datainfos56.sender},
                        {"goodTypeDesc",datainfos56.goodTypeDes},
                        {"amount",datainfos56.amount},
                        {"goodsPrice",datainfos56.goodsPrice},
                        {"weight",datainfos56.weight},
                        {"remark",datainfos56.remark},
                        {"orderNum",datainfos56.orderNum},
                        {"ret1",datainfos56.ret1},
                        {"prodTypeDesc",datainfos56.prodTypeDesc},
                        {"qty",datainfos56.qty},
                        {"senderMobile",datainfos56.senderMobile},
                        {"logisticsMark",datainfos56.logisticsMark},
                        {"ret2",datainfos56.ret2},
                        {"ret3",datainfos56.ret3},
                        {"ret4",datainfos56.ret4},
                        {"ret5",datainfos56.ret5},
                        {"ret6",datainfos56.ret6},
                        {"ret7",datainfos56.ret7},
                        {"ret8",datainfos56.ret8},
                        {"ret9",datainfos56.ret9},
                        {"ret10",datainfos56.ret10}
                    };
                    JArray array = new JArray();
                    array.Add(dataInfo);
                    requestObj.Add("loginInfo", loginInfo);
                    //obj.Add("dataInfo", datainfo);
                    requestObj.Add("dataInfo", array);

                    string request = JsonConvert.SerializeObject(requestObj);
                    string returnInfo = PostWL(url, request);
                    //格式化平台返回的json
                    //returnInfo = ConvertJsonString(returnInfo);
                    //json字符串转实体对象
                    JObject returnModel = (JObject)JsonConvert.DeserializeObject(returnInfo);
                    //解析json字符串
                    returnInfor = returnModel["returnInfor"].ToString();
                    string returnFlag = returnModel["returnFlag"].ToString();

                    if (returnFlag == "1")
                    {
                        if (datainfos56.logisticsMark == "1")
                        {
                            datainfos56.logisticsMark = "C";
                        }
                        if (datainfos56.logisticsMark == "5")
                        {
                            datainfos56.logisticsMark = "X";
                        }
                        if (datainfos56.logisticsMark == "C")
                        {
                            update_sql = "update EV_CargoPlan_h set iCancleFlag = 1 where cPLCode = '" + datainfos56.orderNum + "'";
                        }
                        if (datainfos56.logisticsMark == "X")
                        {
                            update_sql = "update EV_CargoSPlan_h set iCancleFlag = 1 where cSLCode = '" + datainfos56.orderNum + "'";
                        }
                        if (dbop.getsqlcom(update_sql) == 1)
                        {
                            result = "True";
                        }
                        else
                        {
                            result = "False";
                        }
                    }
                    else
                    {
                        result = "False";
                    }
                }
                else
                {
                    result = "False";
                }
            }
            else
            {
                result = "False";
            }
            return result;
        }

        [WebMethod(Description = "接收调度单信息国泰衡云")]
        public string revDispatchInfo(string request)
        {
            string result = "false";
            int pk_store = 0;

            decimal referWeight = 0.000m;//货运计划剩余量
            decimal supWeight = 0.000m;
            DBoperationcs syDbop = new DBoperationcs();
            //json字符串转实体类
            JObject responseModel = (JObject)JsonConvert.DeserializeObject(request);

            string subAccCode = responseModel["dataInfo"]["accCodeSub"].ToString();
            int pk_acc = Convert.ToInt32(responseModel["dataInfo"]["accCode"].ToString());
            int PLhid = Convert.ToInt32(responseModel["dataInfo"]["plHid"].ToString());
            int PLbid = Convert.ToInt32(responseModel["dataInfo"]["plBid"].ToString());
            int C_ClassType = Convert.ToInt32(responseModel["dataInfo"]["classType"].ToString());
            int pk_cubasdoc = Convert.ToInt32(responseModel["dataInfo"]["cusId"].ToString());
            if (responseModel["dataInfo"]["storeId"].ToString() == "")
            {
                pk_store = 0;
            }
            else
            {
                 pk_store = Convert.ToInt32(responseModel["dataInfo"]["storeId"].ToString());
            }
            int pk_address = Convert.ToInt32(responseModel["dataInfo"]["adressId"].ToString());
            int pk_invbasdoc = Convert.ToInt32(responseModel["dataInfo"]["invId"].ToString());
            string fQuanty = responseModel["dataInfo"]["qty"].ToString();//派车单重量
            string cCarNum = responseModel["dataInfo"]["carCode"].ToString();
            string cCarID = responseModel["dataInfo"]["carId"].ToString();
            string cDriver = responseModel["dataInfo"]["driverName"].ToString();
            string cDriverPhone = responseModel["dataInfo"]["driverPhone"].ToString();
            int bRead = Convert.ToInt32(responseModel["dataInfo"]["read"].ToString());
            int bFinish = Convert.ToInt32(responseModel["dataInfo"]["finish"].ToString());
            string C_MeasID = responseModel["dataInfo"]["meaCode"].ToString();
            if (responseModel["dataInfo"]["supWeight"].ToString() != "")
            {
                 supWeight = Convert.ToDecimal(responseModel["dataInfo"]["supWeight"].ToString());//已调用量
            }
            string C_CarGua = responseModel["dataInfo"]["cCarGua"].ToString();
            string creaTm = webTime();
            try
            {
                //判断派车单是否重复插入
                bool resultExit = isExitDispatchInfo(C_MeasID);
                if (resultExit)
                {
                    //存在 执行update
                    //string update_sql = "update CM_MeasTemp set pk_acc = '" + pk_acc + "',PLhid = " + PLhid + ",PLbid = " + PLbid + "," +
                    //              "C_ClassType = '" + C_ClassType + "',pk_cubasdoc = " + pk_cubasdoc + ",pk_store = " + pk_store + ",pk_address = " + pk_address + "," +
                    //              "pk_invbasdoc = " + pk_invbasdoc + ",fQuanty = '" + fQuanty + "',cCarNum = '" + cCarNum + "',cCarID = '" + cCarID + "',cDriver = '" + cDriver + "'," +
                    //              "cDriverPhone = '" + cDriverPhone + "',bRead = '" + bRead + "',bFinish = '" + bFinish + "',C_MeasID = '" + C_MeasID + "',creatTm = '" + creaTm + "',cAccCode = '" + subAccCode + "',cCarGua = '"+ C_CarGua + "' " +
                    //              "where pk_acc = '" + pk_acc + "' and C_MeasID = '" + C_MeasID + "'";

                    string update_sql = "update CM_MeasTemp set bRead = 0,bFinish = 2 where pk_acc = '" + pk_acc + "' and C_MeasID = '" + C_MeasID + "'";
                    string insert_sql = "insert into CM_MeasTemp(pk_acc,PLhid,PLbid,C_ClassType,pk_cubasdoc,pk_store,pk_address,pk_invbasdoc,fQuanty,cCarNum,cCarID,cDriver,cDriverPhone,bRead,bFinish,C_MeasID,creatTm,cAccCode,cCarGua) values " +
                                   "('" + pk_acc + "','" + PLhid + "','" + PLbid + "','" + C_ClassType + "','" + pk_cubasdoc + "','" + pk_store + "','" + pk_address + "','" + pk_invbasdoc + "','" + fQuanty + "','" + cCarNum + "','" + cCarID + "','" + cDriver + "','" + cDriverPhone + "','" + bRead + "','" + bFinish + "','" + C_MeasID + "','" + creaTm + "','" + subAccCode + "','" + C_CarGua + "')";
                    if (syDbop.getsqlcom(update_sql) > 0)
                    {
                        if (syDbop.getsqlcom(insert_sql) > 0)
                        {
                            //WriteID(insert_sql,"更新2后添加数据");
                            result = "true";
                        }
                        //写入日志
                        // string insertLog = "insert into "
                        //更新货运计划子表中的库存量的值
                        //string updateSql = "update EV_CargoSPlan_b set fReferQuanty = '" + supWeight + "', fsurplusWeight = '" + referWeight + "' where PLbid = '" + PLbid + "'";
                        //if (syDbop.getsqlcom(updateSql) == 1)
                        //{
                        //    result = "true";
                        //}
                        //result = "true";
                    }
                }
                else
                {
                    string insert_sql = "insert into CM_MeasTemp(pk_acc,PLhid,PLbid,C_ClassType,pk_cubasdoc,pk_store,pk_address,pk_invbasdoc,fQuanty,cCarNum,cCarID,cDriver,cDriverPhone,bRead,bFinish,C_MeasID,creatTm,cAccCode,cCarGua) values " +
                                   "('" + pk_acc + "','" + PLhid + "','" + PLbid + "','" + C_ClassType + "','" + pk_cubasdoc + "','" + pk_store + "','" + pk_address + "','" + pk_invbasdoc + "','" + fQuanty + "','" + cCarNum + "','" + cCarID + "','" + cDriver + "','" + cDriverPhone + "','" + bRead + "','" + bFinish + "','" + C_MeasID + "','" + creaTm + "','" + subAccCode + "','"+ C_CarGua + "')";
                    if (syDbop.getsqlcom(insert_sql) > 0 )
                    {
                        result = "true";
                        //更新货运计划主表 的 bClose = 3
                        //if (C_ClassType == 1)
                        //{
                        //    //采购
                        //     updateSql = "update EV_CargoPlan_h set bClose = 3 where PLhid = '" + PLhid + "'";

                        //}
                        //else
                        //{
                        //     updateSql = "update EV_CargoSPlan_h set bClose = 3 where PLhid = '" + PLhid + "'";

                        //}
                        //if (syDbop.getsqlcom(updateSql) == 1)
                        //{
                        //    result = "true";
                        //}
                        //result = "true";
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
                WriteYCLog(ex, "接收调度单异常");
                throw ex;
            }       
            return result;
        }

        [WebMethod(Description ="接收调度单(云工物流")]
        public string revDispatchInfo56(string requestJson)
        {
            string response = "";
            string tmNow = webTime();
            DBoperationcs dbop = new DBoperationcs();
            //格式化json
            string request = ConvertJsonString(requestJson);
            //保存到日志表中
            string insert = "insert into CV_ApiLog(cAccessInfo,cAccessTm) values ('" + request + "','" + tmNow + "')";
            dbop.getsqlcom(insert);
            //解析json数据
            JObject objResponse = JObject.Parse(request);
            string weight = objResponse["deliveryInfo"][0]["weight"].ToString();
            string idNum = objResponse["deliveryInfo"][0]["idNum"].ToString();
            string status = objResponse["deliveryInfo"][0]["status"].ToString();
            string orderNum = objResponse["deliveryInfo"][0]["orderNum"].ToString();
            string qty = objResponse["deliveryInfo"][0]["qty"].ToString();
            string companyName = objResponse["deliveryInfo"][0]["companyName"].ToString();
            string amount = objResponse["deliveryInfo"][0]["amount"].ToString();
            string deliverId = objResponse["deliveryInfo"][0]["deliveryId"].ToString();
            string uploadId = objResponse["deliveryInfo"][0]["uploadId"].ToString();
            string driverId = objResponse["deliveryInfo"][0]["driverId"].ToString();
            string ret1 = objResponse["deliveryInfo"][0]["ret1"].ToString();
            string delivertyNum = objResponse[ "deliveryInfo"][0]["deliveryNum"].ToString();
            string createDate = objResponse["deliveryInfo"][0]["createDate"].ToString();
            string driverName = objResponse["deliveryInfo"][0]["driverName"].ToString();
            string carNum = objResponse["deliveryInfo"][0]["carNum"].ToString();
            string platformId = objResponse["deliveryInfo"][0]["platformId"].ToString();

            string selectSql = "";
            string classType = "";
            string insert_sql = "";
            //根据orderNum 查询货运单主表主键 子表主键
            if (orderNum.Substring(0,2)=="PL")
            {
                classType = "1";
                //采购
                 selectSql = "select a.PLhid,b.PLbid,a.cAccCode,a.Factory,b.pk_invbasdoc from EV_CargoPlan_h a " +
                             "left join EV_CargoPlan_b b on a.PLhid = b.PLhid " +
                             "where a.cPLCode = '" + orderNum + "' ";
            }
            else
            {
                classType = "5";
                //销售
                selectSql = "select a.PLhid,b.PLbid,a.Factory,a.cAccCode,b.pk_invbasdoc from EV_CargoSPlan_h a " +
                             "left join EV_CargoSPlan_b b on a.PLhid = b.PLhid " +
                             "where a.cPLCode = '" + orderNum + "' ";
            }
            DataTable dtHY = dbop.GetTable(selectSql);
            if (dtHY.Rows.Count > 0)
            {
                //保存派车单信息到本地
                 insert_sql = "insert into CM_MeasTemp(pk_acc,PLhid,PLbid,C_ClassType,pk_cubasdoc,pk_store,pk_address,pk_invbasdoc,fQuanty,cCarNum,cCarID,cDriver,cDriverPhone,bRead,bFinish,C_MeasID,creatTm,cAccCode) values " +
                                     "(3,'" + dtHY.Rows[0]["PLhid"].ToString() + "','" + dtHY.Rows[0]["PLbid"].ToString() + "','" + classType + "','" + dtHY.Rows[0]["Factory"].ToString() + "',0,0,'" + dtHY.Rows[0]["pk_invbasdoc"].ToString() + "','" + weight + "','" + carNum + "','" + idNum + "','" + driverName + "','',0,'" + status + "','" + deliverId + "','" + createDate + "','"+ dtHY.Rows[0]["cAccCode"].ToString() + "')";

                JObject obj = new JObject();
                if (dbop.getsqlcom(insert_sql) == 1)
                {
                    //上传派车单信息到国泰衡云
                    WebReference.WebService1 hyWeb = new WebReference.WebService1();
                    //组装json数据
                    JObject requestYG = new JObject();
                    var dataInfo = new JObject {
                        {"pkAcc",3},
                        {"PLhid", dtHY.Rows[0]["PLhid"].ToString() },
                        {"PLbid", dtHY.Rows[0]["PLbid"].ToString() },
                        {"classType",classType},
                        {"meaCode",deliverId},
                        {"cusID",dtHY.Rows[0]["Factory"].ToString()},
                        {"pk_invbasdoc",dtHY.Rows[0]["pk_invbasdoc"].ToString()},
                        {"fQua",weight},
                        {"carCode",carNum},
                        {"carID",idNum},//身份证号
                        {"driverName",driverName},
                        {"accCode",dtHY.Rows[0]["cAccCode"].ToString()},
                        {"ts",createDate},
                        {"status",status},
                    };

                    requestYG.Add("dataInfo", dataInfo);
                    string request56 = JsonConvert.SerializeObject(requestYG);

                    string ddd = "0";
                    //string response56 = hyWeb.revDispatchYG(request56);

                    var head = new JObject {
                    {"txnCode", "pushRegistry" },
                    {"code", "R00000" },
                    {"httpEncrypt",true },
                    {"message","操作成功" },
                    {"success",true }
                    };
                    obj.Add("head", head);
                    response = JsonConvert.SerializeObject(obj);
                }
                else
                {
                    var head = new JObject {
                    {"txnCode", "pushRegistry"},
                    {"code", "R00000"},
                    {"httpEncrypt",false},
                    {"message","操作失败"},
                    {"success",false}
                    };
                    obj.Add("head", head);
                    response = JsonConvert.SerializeObject(obj);
                }
            }

            //保存调度单信息到金马数据库 EV_Dispatch 表中
            //string insert_dispatch = "insert into EV_Dispatch(orderNum,deliveryId,deliveryNum,weight,qty,driverName,companyName,amount,createDate,driverId,idNum,carNum,status,platformId) " +
            //                        "values('" + orderNum + "','" + deliverId + "','" + delivertyNum + "','" + weight + "','" + qty + "','" + driverName + "','" + companyName + "','" + amount + "','" + createDate + "','" + driverId + "','" + idNum + "','" + carNum + "','" + status + "','" + platformId + "')";
           
            return response;
        }

        [WebMethod(Description = "接收云端下发的原厂磅单图片信息")]
        public bool revDispatchImg(string request)
        {
            bool result = false;
            DBoperationcs syDbop = new DBoperationcs();
            //json字符串转实体类
            JObject responseModel = (JObject)JsonConvert.DeserializeObject(request);
            string imgs = responseModel["dataInfo"]["imgg"].ToString();//图片
            int accID = Convert.ToInt32(responseModel["dataInfo"]["accID"].ToString());
            string meaCode = responseModel["dataInfo"]["meaCode"].ToString();
            //base64转二进制
            byte[] btnew = Convert.FromBase64String(imgs);
            string str = "update CM_MeasTemp set C_Img  = @file where  pk_acc = "+accID+" and C_MeasID = '"+meaCode+"'";
            //string source = "server=192.168.218.55;database=GTCM;uid=sa;pwd=hnjg";
            string source = "server=192.168.7.50;database=GTCM;uid=sa;pwd=Ibm123";
            SqlConnection con = new SqlConnection(source);
            con.Open();
            SqlCommand mycomm = new SqlCommand(str, con);
            mycomm.Parameters.Add("@file", SqlDbType.Binary, btnew.Length);
            mycomm.Parameters["@file"].Value = btnew;
            int n =  mycomm.ExecuteNonQuery();
            con.Close();
            if (n == 1 )
            {
                result = true;
            }
            return result;
        }

        [WebMethod(Description ="接收云端下发的原厂磅单")]
        public bool revDispatchOldWeight(string request)
        {
            bool result = false;
            decimal oldGross = 0.000m;
            decimal oldTare = 0.000m;
            decimal oldRefer = 0.000m;
            string getCode = "";
            DBoperationcs syDbop = new DBoperationcs();
            //json字符串转实体类
            JObject responseModel = (JObject)JsonConvert.DeserializeObject(request);

            int autoId = Convert.ToInt32(responseModel["dataInfo"]["autoid"].ToString());

            if (responseModel["dataInfo"]["grossOld"].ToString() != "" && responseModel["dataInfo"]["grossOld"].ToString() != null)
            {
                oldGross = Convert.ToDecimal(responseModel["dataInfo"]["grossOld"].ToString());//原厂毛重
            }
            if (responseModel["dataInfo"]["tareOld"].ToString() != "" && responseModel["dataInfo"]["tareOld"].ToString() != null)
            {
                oldTare = Convert.ToDecimal(responseModel["dataInfo"]["tareOld"].ToString());//原厂皮重
            }
            if (responseModel["dataInfo"]["referOld"].ToString() != "" && responseModel["dataInfo"]["referOld"].ToString() != null)
            {
                oldRefer = Convert.ToDecimal(responseModel["dataInfo"]["referOld"].ToString());//原厂净重
            }
            int accID = Convert.ToInt32(responseModel["dataInfo"]["accID"].ToString());
            string meaCode = responseModel["dataInfo"]["meaCode"].ToString();

            //base64转二进制
            string str = "update CM_MeasTemp set  N_FacGross = (case when '" + oldGross + "'='0.000' then null else '" + oldGross + "' end)"+
                ",N_FacTare = (case when '" + oldTare + "'='0.000' then null else '" + oldTare + "' end),"+
                "N_FacSuttle=(case when '" + oldRefer + "'='0.000' then null else '" + oldRefer + "' end) "+
                " where pk_acc = " + accID+" and C_MeasID = '"+ meaCode + "'";
            if (syDbop.getsqlcom(str) > 0)
            {
                result = true;
            }
            return result;
        }
        [WebMethod(Description = "接收云端下发的取样号")]
        public bool revSampCode(string request)
        {
            bool result = false;
            string getCode = "";
            DBoperationcs syDbop = new DBoperationcs();
            //json字符串转实体类
            JObject responseModel = (JObject)JsonConvert.DeserializeObject(request);

            int autoId = Convert.ToInt32(responseModel["dataInfo"]["autoid"].ToString());

            //取样号
            if (responseModel["dataInfo"]["Z_cGetCode"].ToString() != "" && responseModel["dataInfo"]["Z_cGetCode"].ToString() != null)
            {
                getCode = responseModel["dataInfo"]["Z_cGetCode"].ToString();
            }

            int accID = Convert.ToInt32(responseModel["dataInfo"]["accID"].ToString());
            string meaCode = responseModel["dataInfo"]["meaCode"].ToString();
            
            //base64转二进制
            string str = "update CM_MeasTemp set "+
                "Z_cGetCode = (case when '" + getCode + "'='' then null else '" + getCode + "' end) where pk_acc = " + accID + " and C_MeasID = '" + meaCode + "'";
            if (syDbop.getsqlcom(str) > 0)
            {
                result = true;
            }
            return result;

        }

        #region 判断是否取样
        private int boolGetUse(string meaCode)
        {
            int getUse = 0;
            DBoperationcs syDbop = new DBoperationcs();
            string sqlCm = "select I_FinishFlag,C_CarryNo,C_BL_No,Z_iGetUsed from CM_MeasureInfo  " +
                           "where  C_BL_No = '" + meaCode + "' ";

            DataTable dt = syDbop.GetTable(sqlCm);
            if (dt.Rows.Count > 0)
            {
                getUse = Convert.ToInt32(dt.Rows[0]["Z_iGetUsed"].ToString());
            }
            return getUse;
        }
        #endregion
        #region 判断是否计量
        private int boolCm(string meaCode)
        {
            int finishFlag = 0;
            DBoperationcs syDbop = new DBoperationcs();
            string sqlCm = "select I_FinishFlag,C_CarryNo,C_BL_No,Z_iGetUsed from CM_MeasureInfo  " +
                            "where  C_Status = 1  and C_BL_No = '" + meaCode + "' and  I_FinishFlag = 3";

            DataTable dt = syDbop.GetTable(sqlCm);
            if (dt.Rows.Count > 0)
            {
                int isZF = Convert.ToInt32(dt.Rows[0]["I_FinishFlag"].ToString());
                if (isZF == 3)
                {
                    //已作废 不允许恢复
                    finishFlag = 1;
                }
            }
            return finishFlag;
        }
        #endregion

        [WebMethod(Description = "撤销派车单")]
        public bool realisePcdLoad(string meaCode,int accID )
        {
            bool result = false;
            DBoperationcs syDbop = new DBoperationcs();
            int finishFlag = boolCm(meaCode);
            int getUse = boolGetUse(meaCode);
            if (finishFlag == 0 && getUse == 0)
            {
                //未计量、未取样允许撤单
                string updateSql = "update CM_MeasTemp set bFinish = 2 where pk_acc = " + accID + " and C_MeasID = '" + meaCode + "'";
                if (syDbop.getsqlcom(updateSql) > 0)
                {
                    result = true;
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        [WebMethod(Description = "恢复派车单")]
        public bool renewPcdLoad(string meaCode, int accID)
        {
            bool result = false;
            DBoperationcs syDbop = new DBoperationcs();
            //如果 计量数据已作废  不允许恢复
            int finishFlag = boolCm(meaCode);
            if (finishFlag == 0 )
            {
                //未计量、未取样允许撤单
                string updateSql = "update CM_MeasTemp set bFinish = 0 where pk_acc = " + accID + " and C_MeasID = '" + meaCode + "'";
                if (syDbop.getsqlcom(updateSql) > 0)
                {
                    result = true;
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        [WebMethod(Description = "上传基础档案信息")]
        public DataSet  sendBasicInfo(string tableName)
        {
            string select_sql = "";
            //string constr = "server=192.168.218.55;database=GTCM;uid=sa;pwd=hnjg";
            string constr = "server=192.168.7.50;database=GTCM;uid=sa;pwd=Ibm123";
            SqlConnection sqlconn = new SqlConnection(constr);
            sqlconn.Open();
            if (tableName == "CB_cubasdoc")
            {
                 select_sql = "select pk_cubasdoc,pk_Acc,accCode,pk_type,cCustCode,cCustName,cCustShortName,cMemo,dr,ts " +
                              "from CB_cubasdoc ";
            }
            if (tableName == "EI_InvbasDoc")
            {
                 select_sql = "select pk_invbasdoc,pk_acc,cAccCode,cInvCode,cInvShortName,cInvName,pk_invclass,cInvSpec,cInvType,pk_measdoc,fMWeight,dr,ts,pk_warehouse,pk_invwaredoc,fTax " +
                              "from EI_InvbasDoc";
            }
            if (tableName == "EI_invStyle")
            {
                select_sql = "select invId,pk_acc,cAccCode,invStyleCode,invStyle,I_Ctreater,D_Createtime,I_Moditer,D_Modittime,I_Stopper,D_Stoptime,dr,ts " +
                             "from EI_invStyle";
            }
            if (tableName == "CB_OtherCar")
            {
                select_sql = "select I_CarID,cAccId,C_CarDes,cDriverCode,C_Driver,cPassWord,cIDcard,I_Iphone,dr,ts " +
                             "from CB_OtherCar";
            }

            SqlDataAdapter da = new SqlDataAdapter(select_sql, sqlconn);
            DataSet ds = new DataSet();
            da.Fill(ds, "tb");
            sqlconn.Close();
            return ds;
        }

        [WebMethod(Description = "查询发、发货单位")]
        public List<string> SelectSendFac(int type)
        {
            DBoperationcs dbop = new DBoperationcs();
            List<string> SendfacCode_list = new List<string>();
            SendfacCode_list.Add("--请选择--");
            try
            {
                string sql = "select Convert(varchar,pk_cubasdoc) +','+ cCustCode +','+ cCustName as 发货商 from CB_cubasdoc " +
                             "where pk_type = '" + type + "'";

                DataTable dt_nepan_fac = dbop.GetTable(sql);

                if (dt_nepan_fac.Rows.Count > 0)
                {
                    for (int i = 0; i < dt_nepan_fac.Rows.Count; i++)
                    {
                        SendfacCode_list.Add(dt_nepan_fac.Rows[i]["发货商"].ToString());
                    }
                }
                else
                {
                    SendfacCode_list.Add("");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return SendfacCode_list;
        }

        [WebMethod(Description = "查询内盘存货")]
        public List<string> SelectMaties()
        {
            DBoperationcs dbop = new DBoperationcs();
            List<string> mates_list = new List<string>();
            mates_list.Add("--请选择--");
            try
            {
                string str_sql = "select Convert(varchar,pk_invbasdoc) +','+ cInvCode + ','+ cInvName as 存货信息 from EI_InvbasDoc " +
                                 "where pk_invclass = 2";
                DataTable dt_mates = dbop.GetTable(str_sql);
                if (dt_mates.Rows.Count > 0)
                {
                    for (int i = 0; i < dt_mates.Rows.Count; i++)
                    {
                        mates_list.Add(dt_mates.Rows[i]["存货信息"].ToString());
                    }
                }
                else
                {
                    mates_list.Add("");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return mates_list;
        }

        [WebMethod(Description = "更新内盘计量信息")]
        public int insert_cminfo(string str)
        {
            DBoperationcs dbop = new DBoperationcs();
            int result = 0;

            // string update_sql = "update CM_MeasureInfo set "
            result = dbop.getsqlcom(str);
            return result;

        }

        [WebMethod(Description ="判断内盘是否添加数据")]
        public bool isInsert(string carCode)
        {
            bool result = true;
            DBoperationcs dbop = new DBoperationcs();
            string selectSql = "select cCarNum from CM_NPUpInfo where I_FinishFlag == 0 and cCarNum = '"+carCode+"'";
            DataTable dt = dbop.GetTable(selectSql);
            if (dt.Rows.Count > 0)
            {
                result = false;
            }
            return result;
        }
        [WebMethod(Description = "判断是否定期皮")]
        public string isDqp(string carNum)
        {
            DBoperationcs dbop = new DBoperationcs();
            string tareInfo = "";
           // string sql = "select tareWeight,tareTime from CM_OtherTareweight where cCarCode ='" + carNum + "'";
            string sql = "select N_RTare from CM_ReglarTare where C_RCarNo ='" + carNum + "'";
            DataTable dt = dbop.GetTable(sql);
            if (dt.Rows.Count > 0)
            {
                tareInfo = dt.Rows[0]["N_RTare"].ToString();
            }
            return tareInfo;
        }

        #region 发送post请求
        private static string PostWL(string Url, string Data)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            //请求内容为json格式
            request.ContentType = "application/json;charset=UTF-8";
            byte[] bytes = Encoding.UTF8.GetBytes(Data);
            //请求内容为form表单
            // request.ContentType = "application/x-www-form-urlencoded";//‘
            request.ContentLength = bytes.Length;
            //字符流 发送请求数据
            Stream myResponseStream = request.GetRequestStream();
            myResponseStream.Write(bytes, 0, bytes.Length);
            //接收返回数据
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader myStreamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();

            myStreamReader.Close();
            myResponseStream.Close();

            if (response != null)
            {
                response.Close();
            }
            if (request != null)
            {
                request.Abort();
            }
            return retString;
        }
        #endregion

        #region 在金马大宗数据库中查询货运单信息
        private static DataTable selectPurInfo(string vouchCode, string orderNum)
        {
            //string orderNums = "PL201903000001";
            DBoperationcs dbop = new DBoperationcs();
            string select_sql = "";
            if (vouchCode == "PL")
            {
                select_sql = "select a.cPLCode 订单号,b.cWeight 重量,b.cQuantity 数量,b.cprivce 单价,b.cSumprice 订单总价," +
                               "f.cInvName 存货名称, c.cCustName 发货公司, a.csenderMobile 发货人手机号,a.cClassType 单据类型,a.cMemo 备注 from EV_CargoPlan_h a " +
                               "left join EV_CargoPlan_b b on a.PLhid = b.PLhid " +
                               "left join CB_cubasdoc c on a.Factory = c.pk_cubasdoc " +
                               "left join ES_User e on a.iMaker = e.pk_user " +
                               "left join EI_InvbasDoc f on b.pk_invbasdoc = f.pk_invbasdoc " +
                               "where a.cPLCode = '" + orderNum + "'";
            }
            if (vouchCode == "SL")
            {
                select_sql = "select a.cSLCode 订单号,b.cWeight 重量,b.fQuantity 数量,b.cprivce 单价,b.cSumprice 订单总价," +
                               "f.cInvName 存货名称, c.cVenName 发货公司, a.csenderMobile 发货人手机号,a.cClassType 单据类型,a.cMemo 备注 from EV_CargoSPlan_h a " +
                               "left join EV_CargoSPlan_b b on a.SLhid = b.SLhid " +
                               "left join EI_Vendor c on a.Factory = c.AutoID " +
                               "left join ES_User e on a.iMaker = e.pk_user " +
                               "left join EI_InvbasDoc f on b.pk_invbasdoc = f.pk_invbasdoc " +
                               "where a.cSLCode = '" + orderNum + "'";
            }

            try
            {
                DataTable dt_cp = dbop.GetTable(select_sql);

                return dt_cp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        public string DataTableToJson(DataTable table)
        {
            var JsonString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JsonString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JsonString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JsonString.Append("}");
                    }
                    else
                    {
                        JsonString.Append("},");
                    }
                }
                JsonString.Append("]");
            }
            return JsonString.ToString();
        }

        #region 判断派车单是否存在
        private bool isExitDispatchInfo(string meaCode)
        {
            bool result = false;
            DBoperationcs syDbop = new DBoperationcs();
            string selectSql = "select C_MeasID from CM_MeasTemp  where  C_MeasID = '" + meaCode + "' and bFinish != 2";
            DataTable dt = syDbop.GetTable(selectSql);
            if (dt.Rows.Count > 0)
            {
                result = true;
            }
            return result;
        }
        #endregion

        #region 记录程序执行日志
        public static void WriteID(string strID, string strType)
        {
            string strPath = Environment.CurrentDirectory + "\\" + strType + ".txt";
            StreamWriter sw = new StreamWriter(strPath, true);
            string strtime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            sw.WriteLine(strType + "|" + strID + "|" + "执行时间" + "|" + strtime);
            sw.WriteLine();
            sw.Close();
        }
        #endregion

        #region 记录异常日志
        public void WriteYCLog(Exception ex, string strType)
        {
            //如果日志文件为空，则默认在Debug目录下新建 YYYY-mm-dd_Log.log文件
            string strPath = AppDomain.CurrentDomain.BaseDirectory + "\\" + strType + ".txt";

            //把异常信息输出到文件
            StreamWriter sw = new StreamWriter(strPath, true);
            sw.WriteLine("当前时间：" + DateTime.Now.ToString());
            sw.WriteLine("异常信息：" + ex.Message);
            sw.WriteLine("异常对象：" + ex.Source);
            sw.WriteLine("调用堆栈：\n" + ex.StackTrace.Trim());
            sw.WriteLine("触发方法：" + ex.TargetSite);
            sw.WriteLine();
            sw.Close();
        }
        #endregion

        #region 查询供应商主键
        private int selectIdCus(string cusName)
        {
            DBoperationcs dbop = new DBoperationcs();
            int cusID = 0;
            string selectCus = "select pk_cubasdoc from CB_cubasdoc where cCustCode ='" + cusName + "' and accCode = '001' and  pk_type = 2";//pk_type
            DataTable dtCus = dbop.GetTable(selectCus);
            if (dtCus.Rows.Count > 0)
            {
                cusID = Convert.ToInt32(dtCus.Rows[0]["pk_cubasdoc"].ToString());
            }
            return cusID;
        }
        #endregion

        #region 查询货物主键
        private int selectIdInv(string invName)
        {
            DBoperationcs dbop = new DBoperationcs();
            int invID = 0;
            string selectInv = "select pk_invbasdoc from EI_InvbasDoc where cInvCode = '" + invName + "' and cAccCode = '001' ";
            DataTable dtInv = dbop.GetTable(selectInv);
            if (dtInv.Rows.Count > 0)
            {
                invID = Convert.ToInt32(dtInv.Rows[0]["pk_invbasdoc"].ToString());
            }
            return invID;
        }
        #endregion
    }
}
