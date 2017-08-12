using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Base;

namespace Model
{
    public class RunningAccount : BaseModel
    {
        #region Model

        /// <summary>
        /// f_id
        /// </summary>
        public virtual Guid f_id
        {
            get;
            set;
        }

        /// <summary>
        /// f_user_id
        /// </summary>
        public virtual Guid f_user_id
        {
            get;
            set;
        }

        /// <summary>
        /// f_time
        /// </summary>
        public virtual DateTime f_time
        {
            get;
            set;
        }

        /// <summary>
        /// f_year
        /// </summary>
        public virtual int f_year
        {
            get;
            set;
        }

        /// <summary>
        /// f_month
        /// </summary>
        public virtual int f_month
        {
            get;
            set;
        }

        /// <summary>
        /// f_day
        /// </summary>
        public virtual int f_day
        {
            get;
            set;
        }

        /// <summary>
        /// f_money
        /// </summary>
        public virtual decimal f_money
        {
            get;
            set;
        }

        /// <summary>
        /// f_purpose_id
        /// </summary>
        public virtual Guid f_purpose_id
        {
            get;
            set;
        }

        /// <summary>
        /// f_remark
        /// </summary>
        public virtual string f_remark
        {
            get;
            set;
        }

        /// <summary>
        /// f_address
        /// </summary>
        public virtual string f_address
        {
            get;
            set;
        }

        /// <summary>
        /// f_exist
        /// </summary>
        public virtual int f_exist
        {
            get;
            set;
        }


        #endregion

        #region ToJsonString

        /// <summary>
        /// 生成json结构
        /// </summary>
        /// <param name="isClose"></param>
        /// <returns></returns>
        public override string ToJsonString(bool isClose)
        {
            return new System.Text.StringBuilder(string.Empty)
             .Append(isClose ? "{" : "")
                .Append("\"f_id\":\"").Append(Uri.EscapeDataString(this.f_id.ToString())).Append("\",")
                .Append("\"f_user_id\":\"").Append(Uri.EscapeDataString(this.f_user_id.ToString())).Append("\",")
                .Append("\"f_time\":\"").Append(Uri.EscapeDataString(this.f_time.ToString("yyyy-MM-dd HH:mm:ss"))).Append("\",")
                .Append("\"f_year\":\"").Append(Uri.EscapeDataString(this.f_year.ToString())).Append("\",")
                .Append("\"f_month\":\"").Append(Uri.EscapeDataString(this.f_month.ToString())).Append("\",")
                .Append("\"f_day\":\"").Append(Uri.EscapeDataString(this.f_day.ToString())).Append("\",")
                .Append("\"f_money\":\"").Append(Uri.EscapeDataString(this.f_money.ToString())).Append("\",")
                .Append("\"f_purpose_id\":\"").Append(Uri.EscapeDataString(this.f_purpose_id.ToString())).Append("\",")
                .Append("\"f_remark\":").Append(this.f_remark == null ? "null" : ("\"" + Uri.EscapeDataString(this.f_remark.ToString()) + "\"")).Append(",")
                .Append("\"f_address\":").Append(this.f_address == null ? "null" : ("\"" + Uri.EscapeDataString(this.f_address.ToString()) + "\"")).Append(",")
                .Append("\"f_exist\":\"").Append(Uri.EscapeDataString(this.f_exist.ToString())).Append("\"")
                .Append(isClose ? "}" : "")
             .ToString();
        }

        #endregion
    }
}