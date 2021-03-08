using System;

namespace AuthServiceKeycloak
{
  public class KeycloakResponse
    {     
        public string Message { get; set; }
        public string Error { get; set; }
        public string Description { get; set; }
    }
}