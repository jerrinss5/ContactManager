using ContactManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ContactManager.Controllers
{
    [Authorize]
    public class ContactAPIController : ApiController
    {
        public List<Contact> Get()
        {
            return new ContactDB();
        }
    }
}
