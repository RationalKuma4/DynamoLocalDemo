using Amazon.DynamoDBv2;
using DynamoLocalDemo.Settings;
using DynamoLocalDemo.Setup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace DynamoLocalDemo.Controllers
{
    [Route("api/home"),
     ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("init")]
        public async Task<IActionResult> Get([FromServices] IAmazonDynamoDB amazonDynamoDb,
            [FromServices] IOptions<DynamoSettings> dynamoSettings)
        {
            try
            {
                await new DataSetup(dynamoSettings, amazonDynamoDb).CreateTable();
                return Ok();
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }

        [HttpGet("delete")]
        public async Task<IActionResult> Delete([FromServices] IAmazonDynamoDB amazonDynamoDb,
            [FromServices] IOptions<DynamoSettings> dynamoSettings)
        {
            try
            {
                await new DataSetup(dynamoSettings, amazonDynamoDb).DeleteTable();
                return Ok();
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }
    }
}
