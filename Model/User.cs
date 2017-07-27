using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Base;

namespace Model
{
    public class User : BaseModel
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
        /// f_uid
        /// </summary>
        public virtual string f_uid
        {
            get;
            set;
        }

        /// <summary>
        /// f_pwd
        /// </summary>
        public virtual string f_pwd
        {
            get;
            set;
        }

        /// <summary>
        /// f_reg_date
        /// </summary>
        public virtual DateTime f_reg_date
        {
            get;
            set;
        }

        /// <summary>
        /// f_email
        /// </summary>
        public virtual string f_email
        {
            get;
            set;
        }

        /// <summary>
        /// f_phone
        /// </summary>
        public virtual string f_phone
        {
            get;
            set;
        }

        /// <summary>
        /// f_age
        /// </summary>
        public virtual int? f_age
        {
            get;
            set;
        }

        /// <summary>
        /// f_gender
        /// </summary>
        public virtual int? f_gender
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
                .Append("\"f_uid\":\"").Append(Uri.EscapeDataString(this.f_uid.ToString())).Append("\",")
                .Append("\"f_pwd\":\"").Append(Uri.EscapeDataString(this.f_pwd.ToString())).Append("\",")
                .Append("\"f_reg_date\":\"").Append(Uri.EscapeDataString(this.f_reg_date.ToString("yyyy-MM-dd HH:mm:ss"))).Append("\",")
                .Append("\"f_email\":").Append(this.f_email == null ? "null" : ("\"" + Uri.EscapeDataString(this.f_email.ToString()) + "\"")).Append(",")
                .Append("\"f_phone\":").Append(this.f_phone == null ? "null" : ("\"" + Uri.EscapeDataString(this.f_phone.ToString()) + "\"")).Append(",")
                .Append("\"f_age\":").Append(this.f_age == null ? "null" : ("\"" + Uri.EscapeDataString(this.f_age.ToString()) + "\"")).Append(",")
                .Append("\"f_gender\":").Append(this.f_gender == null ? "null" : ("\"" + Uri.EscapeDataString(this.f_gender.ToString()) + "\"")).Append(",")
                .Append("\"f_address\":").Append(this.f_address == null ? "null" : ("\"" + Uri.EscapeDataString(this.f_address.ToString()) + "\"")).Append(",")
                .Append("\"f_exist\":\"").Append(Uri.EscapeDataString(this.f_exist.ToString())).Append("\"")
                .Append(isClose ? "}" : "")
             .ToString();
        }

        #endregion
    }
}