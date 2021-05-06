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
        private readonly IAmazonDynamoDB _amazonDynamoDb;
        private readonly IOptions<DynamoSettings> _dynamoSettings;

        public HomeController(IAmazonDynamoDB amazonDynamoDb, IOptions<DynamoSettings> dynamoSettings)
        {
            _amazonDynamoDb = amazonDynamoDb;
            _dynamoSettings = dynamoSettings;
        }

        // Action to initialize our tables on dynamo
        [HttpGet("init")]
        public async Task<IActionResult> Get()
        {
            try
            {
                // We create the DataSetup object and call the create method
                await new DataSetup(_dynamoSettings, _amazonDynamoDb).CreateTable();
                return Ok();
            }
            catch (Exception e)
            {
                // If something fails we want to know what happened
                return Problem(e.Message);
            }
        }

        // Action to delete our tables on dynamo
        [HttpGet("delete")]
        public async Task<IActionResult> Delete()
        {
            try
            {
                await new DataSetup(_dynamoSettings, _amazonDynamoDb).DeleteTable();
                return Ok();
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }
    }
}
