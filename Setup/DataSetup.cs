using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using DynamoLocalDemo.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DynamoLocalDemo.Setup
{
    public class DataSetup
    {
        private readonly IAmazonDynamoDB _amazonDynamoDb;
        private readonly IDynamoSettings _dynamoSettings;

        public DataSetup(IOptions<IDynamoSettings> dynamoSettings, IAmazonDynamoDB amazonDynamoDb)
        {
            _amazonDynamoDb = amazonDynamoDb;
            _dynamoSettings = dynamoSettings.Value;
        }

        public async Task CreateTable()
        {
            var request = new ListTablesRequest { Limit = 10 };
            var response = await _amazonDynamoDb.ListTablesAsync(request);
            var results = response.TableNames;

            if (!results.Any(_ => _.Equals($"{_dynamoSettings.TablePrefix}{_dynamoSettings.AuthorsTable}", StringComparison.OrdinalIgnoreCase)))
            {
                var authorsTableRequest = new CreateTableRequest
                {
                    TableName = $"{_dynamoSettings.TablePrefix}{_dynamoSettings.AuthorsTable}",
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new()
                        {
                            AttributeName = "Id",
                            AttributeType = ScalarAttributeType.S
                        }
                    },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new()
                        {
                            AttributeName = "Id",
                            KeyType = KeyType.HASH
                        }
                    },
                    BillingMode = BillingMode.PAY_PER_REQUEST
                };

                await _amazonDynamoDb.CreateTableAsync(authorsTableRequest);
                await WaitUntilTableActive(authorsTableRequest.TableName);
            }
        }

        private async Task WaitUntilTableActive(string tableName)
        {
            string status = null;
            do
            {
                Thread.Sleep(1000);
                try
                {
                    status = await GetTableStatus(tableName);
                }
                catch (ResourceNotFoundException) { }
            } while (status != "ACTIVE");
        }

        private async Task<string> GetTableStatus(string tableName) =>
            (await _amazonDynamoDb.DescribeTableAsync(
                new DescribeTableRequest
                {
                    TableName = tableName
                })).Table.TableStatus;

        public async Task DeleteTable() =>
            await _amazonDynamoDb.DeleteTableAsync($"{_dynamoSettings.TablePrefix}{_dynamoSettings.AuthorsTable}");
    }
}
