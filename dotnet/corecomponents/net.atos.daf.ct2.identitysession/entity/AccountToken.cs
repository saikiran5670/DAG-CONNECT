using System;
using System.Collections.Generic;

namespace net.atos.daf.ct2.identitysession.entity
{
    public class AccountToken
    {
        public int Id { get; set; }  
        public string User_Name { get; set; }  
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }  
        public string Refresh_Token { get; set; }  
        public int Refresh_Expire_In { get; set; }  
        public int Account_Id { get; set; }  
        public string TokenType { get; set; }      
        public int Session_Id { get; set; }    
        public string Idp_Type { get; set; }
        public string SessionState { get; set; }
        public int Created_At { get; set; }
        public string Scope { get; set; }
        public string Error { get; set; }
	
    }
}