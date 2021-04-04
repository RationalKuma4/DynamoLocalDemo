using System;

namespace DynamoLocalDemo.Models
{
    public class Book
    {
        public Guid Id { get; set; }
        public string Isbn { get; set; }
        public string Title { get; set; }
    }
}
