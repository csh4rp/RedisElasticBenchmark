using System;

namespace RedisElasticBenchmark
{
    public class Model
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public Guid Guid { get; set; }
    }
}