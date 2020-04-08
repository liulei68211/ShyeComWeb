using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
namespace GTHYWebservice
{
    public class CMoperation
    {
        private DBoperationcs dbop;
        #region 生成计量单号方法
        public string GetMeasureID()
        {
            dbop = new DBoperationcs();
            string strErr = string.Empty;
            string strMeasureID = string.Empty;
            string strQText = string.Empty;
            string strUText = string.Empty;
            string ZD = string.Empty;
            string I_SiteID = string.Empty;

            //获得当前时间
            string strText = "Select Convert(varchar,getdate(),120)";
            DataTable dt = dbop.GetTable(strText);
            string objDate = dt.Rows[0][0].ToString();

            //string IP = dbop.LocalIP;

            //string sqlZD = "select C_SiteNo,I_SiteID from CM_MeaSiteInfo where C_SiteIP='" + IP + "'";
            //DataTable dtZD = dbop.GetTable(sqlZD);
            //ZD = dtZD.Rows[0]["C_SiteNo"].ToString();
            //I_SiteID = dtZD.Rows[0]["I_SiteID"].ToString();
            if (objDate != null)
            {
                List<string> lstSQL = new List<string>();
                strQText = "Select I_MeasureDocID from CM_MeasureID where C_MeaDate='" + objDate.ToString().Substring(0, 10) + "'";
                dt = dbop.GetTable(strQText);

                if (dt.Rows.Count < 1)
                    strMeasureID = ZD + DateTime.Parse(objDate.ToString()).ToString("yyMMdd") + "0001";  //计量单号
                else
                {
                    string objMeasureID = dt.Rows[0]["I_MeasureDocID"].ToString();
                    strMeasureID = ZD + DateTime.Parse(objDate.ToString()).ToString("yyMMdd") + objMeasureID.ToString().ToString().PadLeft(4, '0');  //计量单号
                }
                if (dt.Rows.Count < 1)
                {
                    string strDText = "Delete from CM_MeasureID";
                    dbop.getsqlcom(strDText);

                    strUText = "Insert into CM_MeasureID(I_MeasureDocID,C_MeaDate,C_User,C_TimeStamp) Values("
                             + "2"
                             + ",'" + objDate.ToString().Substring(0, 10) + "'"
                             + ",'自动'"
                             + ",'" + objDate.ToString() + "')";
                    dbop.getsqlcom(strUText);
                }
                else
                {
                    strUText = "Update CM_MeasureID set I_MeasureDocID=I_MeasureDocID+1,C_User='自动',C_TimeStamp='" + objDate.ToString() + "'";
                    dbop.getsqlcom(strUText);
                }
            }
            return strMeasureID;
        }
        #endregion

        #region 生成排队号
        //生成排队号
        public  string GetMeasureOrder()
        {
            dbop =new DBoperationcs();
            string strErr = string.Empty;
            string strOrderID = string.Empty;
            string strQText = string.Empty;
            string strUText = string.Empty;

            string I_SiteID = string.Empty;

            //获得当前时间
            string strText = "Select Convert(varchar,getdate(),120)";
            DataTable dt = dbop.GetTable(strText);
            string objDate = dt.Rows[0][0].ToString();

            //string IP = dbop.LocalIP;

            //string sqlZD = "select C_SiteNo,I_SiteID from CM_MeaSiteInfo where C_SiteIP='" + IP + "'";
            //DataTable dtZD = dbop.GetTable(sqlZD);
            //ZD = dtZD.Rows[0]["C_SiteNo"].ToString();
            //I_SiteID = dtZD.Rows[0]["I_SiteID"].ToString();
            if (objDate != null)
            {
                List<string> lstSQL = new List<string>();
                strQText = "Select I_OrderID from CM_OrderID where C_OrderDate like '%" + objDate.ToString().Substring(0, 10) + "%'";
                dt = dbop.GetTable(strQText);

                if (dt.Rows.Count < 1)
                    strOrderID = DateTime.Parse(objDate.ToString()).ToString("yyyyMMdd") + "001";  //计量单号
                else
                {
                    string objMeasureID = dt.Rows[0]["I_OrderID"].ToString();
                    strOrderID = DateTime.Parse(objDate.ToString()).ToString("yyyyMMdd") + objMeasureID.ToString().ToString().PadLeft(3, '0');  //计量单号
                }
                if (dt.Rows.Count < 1)
                {
                    string strDText = "Delete from CM_OrderID";
                    dbop.getsqlcom(strDText);

                    strUText = "Insert into CM_OrderID(I_OrderID,C_OrderDate,C_User,C_TimeStamp) Values("
                             + "2"
                             + ",'" + objDate.ToString().Substring(0, 10) + "'"
                             + ",'自动'"
                             + ",'" + objDate.ToString() + "')";
                    dbop.getsqlcom(strUText);
                }
                else
                {
                    strUText = "Update CM_OrderID set I_OrderID=I_OrderID+1,C_User='自动',C_TimeStamp='" + objDate.ToString() + "'";
                    dbop.getsqlcom(strUText);
                }
            }
            return strOrderID;
        }

        #endregion 

        //组装json数据
        public string DataTableToJson(DataTable table)
        {
            var JsonString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JsonString.Append("[");
                for (int i= 0;i<table.Rows.Count;i++)
                {
                    JsonString.Append("[");
                    for (int j=0;j<table.Columns.Count;j++)
                    {
                        if (j<table.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }else if (j == table.Columns.Count - 1)
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

        /// <summary>
        /// DataTableToXML 将DataTable转为XML
        /// </summary>
        public  string ConvertDataTableToXML(DataTable dt)
        {
            return ConvertDataTableToXML(dt, string.Empty);
        }
        public  string ConvertDataTableToXML(DataTable dt, string aaa)
        {
            StringWriter sw = null;
            try
            {
                if (dt.TableName == string.Empty)
                    dt.TableName = "table1";
                sw = new StringWriter();
                dt.WriteXml(sw, XmlWriteMode.WriteSchema);
                return sw.ToString();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }
        }
    }

}