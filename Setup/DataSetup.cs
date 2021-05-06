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
            // Dynamo service
            _amazonDynamoDb = amazonDynamoDb;
            // Dynamo settings from our configuration file
            _dynamoSettings = dynamoSettings.Value;
        }

        public async Task CreateTable()
        {
            // Request to know what tables are in our container
            var request = new ListTablesRequest { Limit = 10 };
            var response = await _amazonDynamoDb.ListTablesAsync(request);

            // We save the existing tables names
            var results = response.TableNames;

            // We check if our table of interest is in the existing tables
            // if not we proceed to create the table
            if (!results.Any(_ => _.Equals($"{_dynamoSettings.TablePrefix}{_dynamoSettings.AuthorsTable}", StringComparison.OrdinalIgnoreCase)))
            {
                // Preparation for a table request creation
                // We will keep the request simple so we will just work with the following properties
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

                // We send the request to de Dynamo client 
                await _amazonDynamoDb.CreateTableAsync(authorsTableRequest);
                // Sometime after creating the table we would like to seed some data, but 
                // the process might not be that fast, so we wait until the status is ACTIVE
                await WaitUntilTableActive(authorsTableRequest.TableName);
            }
        }

        // Method to wait for the status
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

        // Method to request for the table status
        private async Task<string> GetTableStatus(string tableName) =>
            (await _amazonDynamoDb.DescribeTableAsync(
                new DescribeTableRequest
                {
                    TableName = tableName
                })).Table.TableStatus;

        // Method to delete the table
        public async Task DeleteTable() =>
            await _amazonDynamoDb.DeleteTableAsync($"{_dynamoSettings.TablePrefix}{_dynamoSettings.AuthorsTable}");
    }
}
