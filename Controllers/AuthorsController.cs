using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using DynamoLocalDemo.Models;
using DynamoLocalDemo.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace DynamoLocalDemo.Controllers
{
    [Route("api/authors"),
     ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IDynamoDBContext _context;

        public AuthorsController(IAmazonDynamoDB amazonDynamoDb, IOptions<DynamoSettings> settings)
        {
            // Read dynamo settings from configuration file
            var dynamoSettings = settings.Value;

            // Configuration for dynamo to add a prefix to our table's name
            var cfg = new DynamoDBContextConfig
            {
                TableNamePrefix = dynamoSettings.TablePrefix
            };
            _context = new DynamoDBContext(amazonDynamoDb, cfg);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Author author)
        {
            try
            {
                await _context.SaveAsync(author);
                return Ok(author);
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var author = await _context.LoadAsync<Author>(id);
            return Ok(author);
        }
    }
}
