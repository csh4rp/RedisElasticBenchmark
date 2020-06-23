using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisElasticBenchmark
{
    public class RedisService : ITestService
    {
        private const string DataKeyName = nameof(DataKeyName);
        private const string DateLookupKeyName = nameof(DateLookupKeyName);
        private const string GuidLookupKeyName = nameof(GuidLookupKeyName);
        
        private readonly IDatabase _database;
        

        public RedisService()
        {
            _database = ConnectionMultiplexer.Connect("127.0.0.1:6389").GetDatabase(0);
        }
        
        public async Task AddAsync(Model model)
        {
            var lookupKey = $"{model.Date.Ticks}:{model.Id}";

            await _database.HashSetAsync(DataKeyName, model.Id, JsonSerializer.Serialize(model));
            await _database.SortedSetAddAsync(DateLookupKeyName, lookupKey, 0);
            await _database.HashSetAsync(GuidLookupKeyName, model.Guid.ToString("N"), model.Id);
        }

        public async Task<List<Model>> GetByDatesAsync(DateTime fromDate, DateTime toDate)
        {
            var startRange = $"{fromDate.Ticks}:";
            var endRange = $"{toDate.Ticks}:";

            var result = await _database.SortedSetRangeByValueAsync(DateLookupKeyName, startRange, endRange, Exclude.None);

            var ids = result.Select(r => (RedisValue) r.ToString().Split(':')[1])
                .ToArray();

            var data = await _database.HashGetAsync(DataKeyName, ids);
            return data.Select(d => JsonSerializer.Deserialize<Model>(d)).ToList();
        }

        public async Task<Model> GetByIdAsync(int id)
        {
            var data = await _database.HashGetAsync(DataKeyName, id);
            return JsonSerializer.Deserialize<Model>(data);
        }

        public async Task<Model> GetByGuidAsync(Guid id)
        {
            var objId = await _database.HashGetAsync(GuidLookupKeyName, id.ToString("N"));
            
            var data = await _database.HashGetAsync(DataKeyName, objId);
            return JsonSerializer.Deserialize<Model>(data);
        }
    }
}