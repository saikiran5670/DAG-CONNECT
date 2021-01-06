using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DAF.IdentityLogic;
using DAF.Entity;


namespace DAF.IdentityService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagement userManagement;
        private readonly ILogger<UserManagementController> logger;
        public UserManagementController(IUserManagement _userManagement,ILogger<UserManagementController> _logger)
        {
            userManagement=_userManagement;
            logger=_logger;

        }

        /// <summary>
        ///  This action method will be used to insert user onto Identity Server
        /// </summary>
        /// <param name="user"> User model that will having information of user creation</param>
        /// <returns>Httpstatuscode along with success or failed message if any</returns>
        [HttpPost]        
        [Route("Create")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            string result = string.Empty;
            try 
            {
                logger.LogInformation("Create user function is called ");
                if(string.IsNullOrEmpty(user.UserName))
                {
                    return StatusCode(401,"invalid_grant: The username is Empty.");
                }
                else
                {
                    //token 
                    Response response = await userManagement.CreateUser(user);
                    if(response!=null && response.Result==null)
                    {
                        logger.LogInformation("User has been added to Identity Server.");
                        return Ok(result);
                    }
                    else 
                    {
                        logger.LogInformation("User has not been added to Identity Server.");
                        return StatusCode((int) response.StatusCode,response.Result);
                    }
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                var p = ex.Message;
                return StatusCode(500,"Internal Server Error.");
            }            
        }
        /// <summary>
        ///  This action method will be used to update user onto Identity Server
        /// </summary>
        /// <param name="user"> User model that will having information of user creation</param>
        /// <returns>Httpstatuscode along with success or failed message if any</returns>
        [HttpPost]        
        [Route("Update")]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            string result = string.Empty;
            try 
            {
                logger.LogInformation("UpdateUser function is called ");
                if(string.IsNullOrEmpty(user.UserName))
                {
                    return StatusCode(401,"invalid_grant: The username is Empty.");
                }
                else
                {
                    Response response = await userManagement.UpdateUser(user);
                    if(response!=null && response.Result==null)
                    {
                        logger.LogInformation("User has been updated to Identity Server.");
                        return Ok(result);
                    }
                    else 
                    {
                        logger.LogInformation("User has not been updated to Identity Server.");
                        return StatusCode((int) response.StatusCode,response.Result);
                    }
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                var p = ex.Message;
                return StatusCode(500,"Internal Server Error.");
            }            
        }
        /// <summary>
        ///  This action method will be used to delete user onto Identity Server
        /// </summary>
        /// <param name="user"> User model that will having information of user creation</param>
        /// <returns>Httpstatuscode along with success or failed message if any</returns>
        [HttpPost]        
        [Route("Delete")]
        public async Task<IActionResult> DeleteUser([FromBody] User user)
        {
            string result = string.Empty;
            try 
            {
                logger.LogInformation("DeleteUser function is called ");
                if(string.IsNullOrEmpty(user.UserName))
                {
                    return StatusCode(401,"invalid_grant: The username is Empty.");
                }
                else
                {
                    Response response = await userManagement.DeleteUser(user);
                    if(response!=null && response.Result==null)
                    {
                        logger.LogInformation("User has been deleted from Identity Server.");
                        return Ok(result);
                    }
                    else 
                    {
                        logger.LogInformation("User has not been deleted from Identity Server.");
                        return StatusCode((int) response.StatusCode,response.Result);
                    }
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                var p = ex.Message;
                return StatusCode(500,"Internal Server Error.");
            }            
        }
        /// <summary>
        ///  This action method will be used to change user password onto Identity Server
        /// </summary>
        /// <param name="user"> User model that will having information of user creation</param>
        /// <returns>Httpstatuscode along with success or failed message if any</returns>
        [HttpPost]        
        [Route("Changepassword")]
        public async Task<IActionResult> ChangeUserPassword([FromBody] User user)
        {
            string result = string.Empty;
            try 
            {
                logger.LogInformation("Change user password function is called");
                if(string.IsNullOrEmpty(user.UserName))
                {
                    return StatusCode(401,"invalid_grant: The username is Empty.");
                }
                else
                {
                    Response response = await userManagement.ChangeUserPassword(user);
                    if(response!=null && response.Result==null)
                    {
                        logger.LogInformation("User password has been changed into identity Server.");
                        return Ok(result);
                    }
                    else 
                    {
                        logger.LogInformation("User password changed has failed.");
                        return StatusCode((int) response.StatusCode,response.Result);
                    }
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                var p = ex.Message;
                return StatusCode(500,"Internal Server Error.");
            }            
        }
    }
}
