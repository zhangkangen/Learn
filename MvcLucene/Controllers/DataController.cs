using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MvcLucene.Controllers
{
    public class DataController : ApiController
    {
        // GET api/data
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/data/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/data
        public void Post([FromBody]string value)
        {
        }

        // PUT api/data/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/data/5
        public void Delete(int id)
        {
        }
    }
}
