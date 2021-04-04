using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;

namespace DynamoLocalDemo.Models
{
    [DynamoDBTable("authors")]
    public class Author
    {
        [DynamoDBHashKey]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Book> Books { get; set; }
    }
}
