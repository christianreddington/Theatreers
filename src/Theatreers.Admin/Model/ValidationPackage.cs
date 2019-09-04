using System;
using System.Collections.Generic;
using System.Text;

namespace Theatreers.Admin.Model
{
    public class ValidationPackage
    {
        public bool ValidToken { get; set; }
        public string PrincipalName { get; set; }
        public string Scope { get; set; }
        public string AppID { get; set; }
        public long IssuedAt { get; set; }
        public long ExpiresAt { get; set; }
        public string Token { get; set; }
        public string Permissions { get; set; }
        public string LastName { get; internal set; }
        public string FirstName { get; internal set; }
        public ValidationPackage()
        {
            ValidToken = false;
        }
    }
}
