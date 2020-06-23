using System;
using MongoDB.Bson.Serialization.Attributes;

namespace RedisElasticBenchmark
{
    public class Model
    {
        [BsonId]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public Guid Guid { get; set; }
    }
}