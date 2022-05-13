using ERPService.BC;
using ERPService.BC.Utility;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Microsoft.Owin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;

namespace ERPService.WebApi
{

    /// <summary>
    /// An example of controller
    /// </summary>
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : AppApiBaseController
    {

        public UserController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {

        }

        [HttpGet]
        [Route("userLoginWindows")]
        [AllowAnonymous]
        public UserInfo userLoginWindows()
        {
            var userLogin = new UserLogin()
            {
                SSOUserName = HttpContext.Current.User.Identity.Name,
                PassWord = null
            };
            UserMasterBC userBC = new UserMasterBC(_logger, _repository);
            return userBC.ValidateUserLogin(userLogin, true);
        }
        [HttpPost]
        [Route("userLogin")]
        [AllowAnonymous]
        public UserInfo userLogin(UserLogin userLogin)
        {
            UserMasterBC userBC = new UserMasterBC(_logger, _repository);
            return userBC.ValidateUserLogin(userLogin);
        }
        [HttpPost]
        [Route("RePassword")]
        [AllowAnonymous]
        public UserSetting RePassword(UserMaster userInfo)
        {
            UserMasterBC userBC = new UserMasterBC(_logger, _repository);
            return userBC.SaveRePwd(userInfo);
        }
        [HttpGet]
        [Route("sendPasswordResetMail/{id}")]
        public AppResponse SendPasswordResetMail(Guid id)
        {
            UserMasterBC userBC = new UserMasterBC(_logger, _repository);
            return userBC.SendPasswordResetMail(id);
        }
        [HttpGet]
        [Route("ResetPassword/{key}")]
        [AllowAnonymous]
        public UserMaster ResetPassword(string Key)
        {
            UserMasterBC userBC = new UserMasterBC(_logger, _repository);
            return userBC.getResetPassword(Key);
        }
        [HttpPost]
        [Route("saveUserInfo")]
        [Authorize]
        public AppResponse SaveUserInfo(UserMaster userInfo)
        {
            UserMasterBC userBC = new UserMasterBC(_logger, _repository);           
            return userBC.SaveUserInfo(userInfo);
        }
        [HttpPost]
        [Route("getUserList")]
        [Authorize]
        public List<UserMaster> GetUserList(UserMaster user)
        {
            UserMasterBC userBC = new UserMasterBC(_logger, _repository);
            return userBC.GetUserList(user);
        }

        [HttpGet]
        [Route("getUserListFilterd/{query}")]
        [Authorize]
        public List<SelectLabel> GetUserList(string query)
        {
            UserMasterBC userBC = new UserMasterBC(_logger, _repository);
            return userBC.GetUserListFiltered(query);
        }

        [HttpGet]
        [Route("getUserMasterById/{userMasterId}")]
        [Authorize]
        public UserMaster GetUserMasterById(Guid userMasterId)
        {
            UserMasterBC userMasterBC = new UserMasterBC(_logger, _repository);
            return userMasterBC.GetUserMasterEFById(userMasterId);
        }
        [HttpGet]
        [Route("getUserAccessInfo")]
        [Authorize]
        public UserInfo GetUserAccessInfo()
        {
            UserMasterBC userMasterBC = new UserMasterBC(_logger, _repository);
            return userMasterBC.GetUserAccessInfo(_userContext.Id);
        }

        [HttpGet]
        [Route("markUserInactive/{userMasterId}")]
        [Authorize]
        public AppResponse MarkUserInactive(Guid userMasterId)
        {
            UserMasterBC userMasterBC = new UserMasterBC(_logger, _repository);
            return userMasterBC.MarkUserInactive(userMasterId);
        }       
    }
}