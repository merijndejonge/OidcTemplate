using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenSoftware.OidcTemplate.Domain.Authentication;

namespace OpenSoftware.OidcTemplate.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = DomainPolicies.NormalUser)]
    public class AnotherController : Controller
    {
        [Authorize(Policy = DomainPolicies.NormalUser)]
        [HttpGet("[action]")]
        public void TestNswag([FromBody] WouldLikeThisOnTheClient easy)
        {

        }

        public class WouldLikeThisOnTheClient
        {
            public string Test { get; set; }

            public string Another { get; set; }
        }
    }
}
