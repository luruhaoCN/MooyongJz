using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MooyongCommon;
using MooyongCommon.MyDB;
using MooyongEntity.SysEntity;
using MooyongJz.ApiBase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace MooyongJz.Controllers.SysController
{
    [ApiController]
    [Route("[controller]")]
    [ServiceFilter(typeof(GlobalExceptionFilter))]
    [ApiExplorerSettings(GroupName = "base")]
    public class UserController : ApiControllerBase
    {
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="user">查询参数</param>
        /// <returns></returns>
        [Route("GetList")]
        [HttpPost]
        [Authorize(Roles = "testUser")]
        public ApiResult GetList([FromBody]User user)
        {
            using DapperHelper helper = new DapperHelper();
            List<User> result = helper.GetAll<User>().AsList();
            ApiResult ret = new ApiResult().SetSuccessResult();
            ret.ResultObject = result;
            return ret;
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="user">查询参数</param>
        /// <returns></returns>
        [Route("Add")]
        [HttpPost]
        public ApiResult Add([FromBody] User user)
        {
            return new ApiResult().SetSuccessResult();
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="user">查询参数</param>
        /// <returns></returns>
        [Route("Update")]
        [HttpPost]
        public ApiResult Update([FromBody] User user)
        {
            return new ApiResult();
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="user">查询参数</param>
        /// <returns></returns>
        [Route("Delete")]
        [HttpPost]
        public ApiResult Delete([FromBody] User user)
        {
            return new ApiResult().SetFailedResult("-1","操作删除失败！");
        }

        /// <summary>
        /// 登录API （账号登陆）
        /// </summary>
        /// <param name="user_code">登录账号，可以是手机或者用户名、邮箱</param>
        /// <param name="user_password">加密后的密码，这里避免明文，客户端加密后传到API端</param>
        /// <param name="deviceType">客户端的设备类型</param>
        /// <param name="clientId">客户端识别号, 一般在APP上会有一个客户端识别号</param>
        /// <returns></returns>
        [Route("Login")]
        [HttpPost]
        public ApiResult Login(string user_code, string user_password, int deviceType = 0, string clientId = "")
        {
            if (string.IsNullOrEmpty(user_code))
                throw new ArgumentException("用户名不能为空。", "user_code");

            if (string.IsNullOrEmpty(user_password))
                throw new ArgumentException("密码不能为空.", "user_password");

            using DapperHelper helper = new DapperHelper();
            User nowUser = helper.QueryFirst<User>(@"select * from t_sys_user where user_code = '"+ user_code + "' or user_name = '"+user_code+"'");
            if (nowUser == null)
                throw new ArgumentException("用户不存在或密码错误！");

            #region 验证密码
            if (!string.Equals(nowUser.User_password, user_password))
            {
                throw new ArgumentException("用户不存在或密码错误！");
            }
            #endregion

            if (nowUser.Is_enable != "1")
                throw new ArgumentException("用户已失效！");
            string passkey = "";
            ApiResult ret = new ApiResult().SetSuccessResult();
            if (nowUser != null)
            {
                passkey = ApiTools.StringToMD5Hash(nowUser.User_id + nowUser.User_code + DateTime.UtcNow + Guid.NewGuid());
                //生成Token
                var token = ApiAuthentication.GenUserToken(nowUser.User_name,"testUser");
                var refreshToken = "123456123456123456";
                nowUser.User_password = "";
                ret.ResultObject = new {Token = token, RefreshToken = refreshToken, SessionKey = passkey, LogonUser = nowUser };
            }
            else
            {
                ret = new ApiResult().SetFailedResult("-1","获取用户数据失败！");
            }
            return ret;
        }
    }
}
