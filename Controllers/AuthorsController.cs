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

        public AuthorsController(IAmazonDynamoDB amazonDynamoDb, IOptions<IDynamoSettings> settings)
        {
            var dynamoSettings = settings.Value;
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
            var auhtor = await _context.LoadAsync<Author>(id);
            return Ok(auhtor);
        }
    }
}
