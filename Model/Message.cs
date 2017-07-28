using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Base;

namespace Model
{
    public class Message : BaseModel
    {
        #region Model

        /// <summary>
        /// f_message_id
        /// </summary>
        public virtual Guid f_message_id
        {
            get;
            set;
        }

        /// <summary>
        /// f_writer_id
        /// </summary>
        public virtual Guid f_writer_id
        {
            get;
            set;
        }

        /// <summary>
        /// f_writer_name
        /// </summary>
        public virtual string f_writer_name
        {
            get;
            set;
        }

        /// <summary>
        /// f_common_date
        /// </summary>
        public virtual DateTime? f_common_date
        {
            get;
            set;
        }

        /// <summary>
        /// f_parent_message_id
        /// </summary>
        public virtual Guid f_parent_message_id
        {
            get;
            set;
        }

        /// <summary>
        /// f_message_type
        /// </summary>
        public virtual int? f_message_type
        {
            get;
            set;
        }

        /// <summary>
        /// f_message_exist
        /// </summary>
        public virtual int f_message_exist
        {
            get;
            set;
        }

        /// <summary>
        /// f_content
        /// </summary>
        public virtual string f_content
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
                .Append("\"f_message_id\":\"").Append(Uri.EscapeDataString(this.f_message_id.ToString())).Append("\",")
                .Append("\"f_writer_id\":").Append(this.f_writer_id == null ? "null" : ("\"" + Uri.EscapeDataString(this.f_writer_id.ToString()) + "\"")).Append(",")
                .Append("\"f_writer_name\":").Append(this.f_writer_name == null ? "null" : ("\"" + Uri.EscapeDataString(this.f_writer_name.ToString()) + "\"")).Append(",")
                .Append("\"f_common_date\":").Append(this.f_common_date == null ? "null" : ("\"" + Uri.EscapeDataString(Convert.ToDateTime(this.f_common_date).ToString("yyyy-MM-dd HH:mm:ss")) + "\"")).Append(",")
                .Append("\"f_parent_message_id\":").Append(this.f_parent_message_id == null ? "null" : ("\"" + Uri.EscapeDataString(this.f_parent_message_id.ToString()) + "\"")).Append(",")
                .Append("\"f_message_type\":").Append(this.f_message_type == null ? "null" : ("\"" + Uri.EscapeDataString(this.f_message_type.ToString()) + "\"")).Append(",")
                .Append("\"f_message_exist\":\"").Append(Uri.EscapeDataString(this.f_message_exist.ToString())).Append("\",")
                .Append("\"f_content\":").Append(this.f_content == null ? "null" : ("\"" + Uri.EscapeDataString(this.f_content.ToString()) + "\"")).Append("")
                .Append(isClose ? "}" : "")
             .ToString();
        }

        #endregion
    }
}