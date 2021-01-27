using System;
using System.Collections.Generic;

namespace net.atos.daf.ct2.identitysession.entity
{
    public class AccountToken
    {
        
        public string UserName { get; set; }  
        public string AccessToken { get; set; }
        public int ExpireIn { get; set; }  
        public string RefreshToken { get; set; }  
        public int RefreshExpireIn { get; set; }
        public int UserId { get; set; }  
        public int AccountId { get; set; }  
        public string TokenType { get; set; }    
        public string IdpType { get; set; }
        public string SessionState { get; set; }
        public long CreatedAt { get; set; }
        public string Scope { get; set; }
        public string Error { get; set; }
        public string TokenId{get; set;}
	
    }
}