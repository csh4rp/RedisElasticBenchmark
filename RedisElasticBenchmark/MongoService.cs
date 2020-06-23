using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Nest;

namespace RedisElasticBenchmark
{
    public class MongoService : ITestService
    {
        private MongoClient _client;
        private readonly IMongoCollection<Model> _collection;
        private readonly List<Model> _list = new List<Model>();
        
        public MongoService()
        {
            _client = new MongoClient("mongodb://root:example@localhost:27017?maxPoolSize=3000");
            _collection = _client.GetDatabase("model").GetCollection<Model>("model");
        }
        
        public async Task AddAsync(Model model)
        {
            _list.Add(model);
            if (_list.Count == 10000)
            {
                await _collection.InsertManyAsync(_list);
                _list.Clear();
            }
        }

        public async Task<List<Model>> GetByDatesAsync(DateTime fromDate, DateTime toDate)
        {
            using (var result = await _collection.FindAsync(f => f.Date >= fromDate && f.Date <= toDate))
            {
                return result.ToList();
            }
        }

        public async Task<Model> GetByIdAsync(int id)
        {
            var filter = Builders<Model>.Filter.Eq("_id", id);
            using (var result = await _collection.FindAsync(filter))
            {
                return result.ToList().First();
            }
        }

        public async Task<Model> GetByGuidAsync(Guid id)
        {
            using (var result = await _collection.FindAsync(f => f.Guid == id))
            {
                return result.ToList().First();
            }
        }
    }
}