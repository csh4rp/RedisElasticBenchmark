using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedisElasticBenchmark
{
    public interface ITestService
    {
        Task AddAsync(Model model);
        Task<List<Model>> GetByDatesAsync(DateTime fromDate, DateTime toDate);
        Task<Model> GetByIdAsync(int id);
        Task<Model> GetByGuidAsync(Guid id);
    }
}