using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace ReadGen
{
    public class RESTBase
    {
        protected string UserName;
        protected string Password;
        protected string ServerURL;
        protected ICredentials creds;

        protected const PutRequestCompression UsePutCompression = PutRequestCompression.none;
        protected enum PutRequestCompression
        {
            none,
            gzip,
            deflate
        }
        public RESTBase(string user, string pass, string url)
        {
            UserName = user;
            Password = pass;
            ServerURL = url;
            creds = CreateCredentials();
        }
        private ICredentials CreateCredentials()
        {
            // Credential object to be passed along with the resource request
            return new NetworkCredential(UserName, Password);
        }
    }
}
