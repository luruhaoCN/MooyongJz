using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooyongEntity.SysEntity
{
    public class User
    {
        public string User_id { get; set; }
        public string User_code { get; set; }
        public string User_name { get; set; }
        public string User_password { get; set; }
        public string User_caption { get; set; }
        public string? Card_type { get; set; }
        public string? User_card { get; set; }
        public string User_email { get; set; }
        public string? Remark { get; set; }
        public string Is_enable { get; set; }
        public string Creator { get; set; }
        public DateTime Created_date { get; set; }
        public string? Modifier { get; set; }
        public DateTime? Last_updated_date { get; set; }
        public string Update_control_id { get; set; }
        public string? Org_user { get; set; }
        public DateTime? Org_date { get; set; }
    }
    public class PersonMapper : ClassMapper<User>
    {
        public PersonMapper()
        {
            Table("t_sys_user");
            AutoMap();
        }
    }
}
