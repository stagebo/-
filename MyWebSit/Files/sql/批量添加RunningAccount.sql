﻿
--批量添加前一百天记录
use blogsystem
go
declare @time datetime
declare @i int
set @i =1
while(@i <100)
begin

set @id=newid();
set @time = getDate()
set @time = DATEADD(day,-@i,getdate())

INSERT [dbo].[t_running_account] ([f_id], [f_user_id], [f_time], [f_year], [f_month], [f_day], [f_money],
 [f_type], [f_purpose_id], [f_remark], [f_address], [f_exist]) 
VALUES (@id, N'3162231b-42f9-4bf3-902f-386b1c8f598c', 
@time, Datename(year,@time), Datename(month,@time), Datename(day,@time),
 1, 1, N'72b95c9a-9502-46e0-bcff-88f02711a987', N'fagongzi', N'', 1)
 set @i = @i+1
end