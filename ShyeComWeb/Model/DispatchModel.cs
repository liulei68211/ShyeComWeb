using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShyeComWeb.Model
{
    /// <summary>
    /// 调度单实体类
    /// </summary>
    public class DispatchModel
    {
        public int autoId;
        public string accCode;
        public int plHid;
        public int plBid;
        public string cmCode;
        public string clssType;
        public int customId;
        public int invId;
        public int adressId;
        public int storeId;
        public string quality;
        public string carCode;
        public string driverName;
        public string driverPhone;
        public string carId;//身份证号
        public string creatTm;//派单时间
        public int read;
        public int finish;
    }
}