using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Base;

namespace Model
{
    public class AccountPurpose : BaseModel
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
        /// f_name
        /// </summary>
        public virtual string f_name
        {
            get;
            set;
        }

        /// <summary>
        /// f_type
        /// </summary>
        public virtual int? f_type
        {
            get;
            set;
        }

        /// <summary>
        /// f_descript
        /// </summary>
        public virtual string f_descript
        {
            get;
            set;
        }

        /// <summary>
        /// f_user_id 用户ID
        /// </summary>
        public virtual Guid f_user_id
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
                .Append("\"f_name\":\"").Append(Uri.EscapeDataString(this.f_name.ToString())).Append("\",")
                .Append("\"f_type\":").Append(this.f_type == null ? "null" : ("\"" + Uri.EscapeDataString(this.f_type.ToString()) + "\"")).Append(",")
                .Append("\"f_descript\":").Append(this.f_descript == null ? "null" : ("\"" + Uri.EscapeDataString(this.f_descript.ToString()) + "\"")).Append("")
                .Append(isClose ? "}" : "")
             .ToString();
        }

        #endregion
    }
}