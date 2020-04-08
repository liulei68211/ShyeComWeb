using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
namespace GTHYWebservice
{
    public class DBoperationcs
    {
        private SqlConnection mycon = null;
        private SqlCommand mycom = null;

        public SqlConnection GetCon()
        {
           // string constr = "server=192.168.218.55;database=GTCM;uid=sa;pwd=hnjg";
            string constr = "server=192.168.7.50;database=GTCM;uid=sa;pwd=Ibm123";
            mycon = new SqlConnection(constr);
            return mycon;
        }
        /// <summary>
        /// 打开数据库
        /// </summary>
        /// <returns></returns>
        public int  getcon()
        {
            int result_open = 0;
           string constr = "server=192.168.7.50;database=GTCM;uid=sa;pwd=Ibm123";
            //string constr = "server=192.168.218.55;database=GTCM;uid=sa;pwd=hnjg";

            mycon = new SqlConnection(constr);
            mycon.Open();
            if (mycon.State == ConnectionState.Open)
            {
                result_open = 1;
            }
            return result_open;
        }
        /// <summary>
        /// 关闭数据库
        /// </summary>
        public void closecon()
        {
            if (mycon.State == ConnectionState.Open)
            {
                mycon.Dispose();
                mycon.Close();
            }
        }
        /// <summary>
        /// 增删改操作数据库
        /// </summary>
        /// <param name="SQLstr"></param>
        /// <returns></returns>
        public int getsqlcom(string SQLstr)
        {
            int result_com = 0;
            try
            {
                if (getcon() == 1)
                {
                    mycom = new SqlCommand(SQLstr, mycon);
                    result_com = mycom.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                closecon();
            }           
            return result_com;
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="SQLstr"></param>
        /// <returns></returns>
        public SqlDataReader getrd(string SQLstr)
        {
            SqlDataReader mydr = null;
            if (getcon() == 1)
            {
                mycom = new SqlCommand(SQLstr, mycon);
                mydr = mycom.ExecuteReader();
            }
            return mydr;
        }

        /// <summary>
        /// 生成数据表
        /// </summary>
        /// <param name="sqlstr"></param>
        /// <returns></returns>
        public DataTable GetTable(string sqlstr)//生成数据表
        {
            DataTable mydt = null;
            if (getcon() == 1)
            {
                SqlDataAdapter myda = new SqlDataAdapter(sqlstr, mycon);
                // DataSet myds = new DataSet();
                mydt = new DataTable("default");
                myda.Fill(mydt);
                closecon();
            }
            return mydt;
        }


        public DataSet GetDs(string sqlstr,string tableName)//生成数据表
        {
            DataSet ds = null;
            if (getcon() == 1)
            {
                SqlDataAdapter da = new SqlDataAdapter(sqlstr, mycon);
                ds = new DataSet();
                da.Fill(ds, "tb");
            }

            return ds;

        }
    }
}