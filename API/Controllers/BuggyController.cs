using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController: BaseApiController
    {
        [HttpGet("unauthorized")]
        public IActionResult GetUnauthorzied()
        {
            return Unauthorized();
        }

        [HttpGet("badrequest")]
        public IActionResult GetBadrequest()
        {
            return BadRequest("This is not a good request");
        }

        [HttpGet("notfound")]
        public IActionResult GetNotfound()
        {
            return NotFound();
        }

        [HttpGet("internalerror")]
        public IActionResult GetInternalError()
        {
            throw new Exception("This is a test exception");
        }

        [HttpPost("validationerror")]
        public IActionResult GetValidationError(CreateProductDto product)
        {
            return Ok();
        }
    }
}