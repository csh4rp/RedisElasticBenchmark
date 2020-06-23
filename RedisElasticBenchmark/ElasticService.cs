using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace RedisElasticBenchmark
{
    public class ElasticService : ITestService
    {
        private const string IndexName = "model";
        private readonly IElasticClient _client;

        public ElasticService()
        {
            _client = new ElasticClient(new Uri("http://localhost:10200"));
        }


        public Task AddAsync(Model model)
        {
            var request = new IndexRequest<Model>(model, IndexName, model.Id);

            return _client.IndexAsync(request);
        }

        public async Task<List<Model>> GetByDatesAsync(DateTime fromDate, DateTime toDate)
        {
            var response = await _client.SearchAsync<Model>(s => s.Index(IndexName)
                .Query(q =>
                    q.DateRange(s =>
                        s.Field(f => f.Date)
                            .GreaterThanOrEquals(fromDate)
                            .LessThanOrEquals(toDate))).Take(1000));

            return response.Documents.ToList();
        }

        public async Task<Model> GetByIdAsync(int id)
        {
            var request = new GetRequest(IndexName, id);

            var response = await _client.GetAsync<Model>(request);

            return response.Source;
        }

        public async Task<Model> GetByGuidAsync(Guid id)
        {
            var response = await _client.SearchAsync<Model>(s => s.Index(IndexName)
                .Query(q =>
                    q.Term(t =>
                        t.Field(f =>
                            f.Guid.Suffix("keyword")).Value(id))));

            return response.Documents.First();
        }
    }
}