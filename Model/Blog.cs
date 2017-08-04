using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base;

namespace Model
{
    class Blog:BaseModel
    {

        public override string ToJsonString(bool flag)
        {
            return base.ToJsonString();
        }
    }
}
//USE[BlogSystem]
//GO

//CREATE TABLE[dbo].[t_blog](
//	[f_id]
//    [uniqueidentifier]
//    NOT NULL,

//    [f_title] [nvarchar](256) NOT NULL,

//[f_second_title] [nvarchar](512) NULL,
//	[f_content]
//    [text]
//    NULL,
//	[f_state]
//    [int]
//    NOT NULL,

//    [f_create_time] [datetime]
//    NOT NULL,

//    [f_modify_time] [datetime]
//    NOT NULL,

//    [f_writer_id] [uniqueidentifier]
//    NOT NULL,

//    [f_type] [int]
//    NULL,
//	[f_exist]
//    [int]
//    NOT NULL,
// CONSTRAINT[PK_t_blog] PRIMARY KEY CLUSTERED
//(
//   [f_id] ASC
//)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]
//) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]

//GO
