using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace url_verification_handshake.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotifierController : ControllerBase
    {
        // POST: Notifier
        [HttpPost]
        public ActionResult Post(JToken token)
        {
            var type = token["type"].Value<string>();
            var challenge = token["challenge"].Value<string>();
            if (type != "url_verification")
            {
                return NotFound();
            }
            return Ok(challenge);
        }
    }
}