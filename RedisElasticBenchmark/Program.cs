using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using RedisElasticBenchmark;
using System.Linq;

namespace RedisElasticBenchmark
{
    class Program
    {
        private static readonly RedisService redisService = new RedisService();
        private static readonly ElasticService elasticService = new ElasticService();
        private static readonly MongoService mongoService = new MongoService();
        
        private static readonly List<Model> objects = new List<Model>();

        private const int Id = 5000;
        private const int Number = 10000;
        private const int NumberOfRuns = 1000;
        private static Guid Guid = Guid.Parse("6ae30b1e-a4b9-4afb-a4c4-4e4b64536325"); // Needs to be retrieved first
        private static readonly DateTime startDate = DateTime.Now.Date;
        private static readonly DateTime endDate = DateTime.Now.Date.AddDays(7);
        private static Dictionary<string, List<long>> Times = new Dictionary<string, List<long>>();



        static void Main(string[] args)
        {
            Console.WriteLine("Press i - initialize, r - range test, d - id test, g - guid test, e - exit");
            var key = Console.ReadLine();

            Times["Redis"] = new List<long>();
            Times["Elastic"] = new List<long>();
            Times["Mongo"] = new List<long>();

                // Warm up
                GetByDatesRedis().Wait();
                GetByDatesElastic().Wait();
                GetByDatesMongo().Wait();

            while (key != "e")
            {
                Stopwatch sw;
                
                var startDate = DateTime.Now.AddDays(-180);

                if (key == "i")
                {
                    for (int i = 1; i <= Number; i++)
                    {
                        objects.Add(new Model
                        {
                            Id = i,
                            Date = startDate.AddMinutes(51.84 * i),
                            Name = $"Sample model name {i}",
                            Guid = Guid.NewGuid()
                        });
                    }
                }



                sw = Stopwatch.StartNew();

                switch (key)
                {
                    case "i":
                        RunElastic().Wait();
                        break;
                    case "r":
                        GetByDatesElastic().Wait();
                        break;
                    case "d":
                        GetByIdElastic().Wait();
                        break;
                    case "g":
                        GetByGuidElastic().Wait();
                        break;
                    default:
                        break;
                }

                sw.Stop();

                var elapsedElastic = sw.Elapsed;

                Times["Elastic"].Add(elapsedElastic.Milliseconds);
                
                sw = Stopwatch.StartNew();

                switch (key)
                {
                    case "i":
                        RunRedis().Wait();
                        break;
                    case "r":
                        GetByDatesRedis().Wait();
                        break;
                    case "d":
                        GetByIdRedis().Wait();
                        break;
                    case "g":
                        GetByGuidRedis().Wait();
                        break;
                    default:
                        break;
                }
                
                sw.Stop();

                var elapsedRedis = sw.Elapsed;

                Times["Redis"].Add(elapsedRedis.Milliseconds);
                
                sw = Stopwatch.StartNew();

                switch (key)
                {
                    case "i":
                        RunMonngo().Wait();
                        break;
                    case "r":
                        GetByDatesMongo().Wait();
                        break;
                    case "d":
                        GetByIdMongo().Wait();
                        break;
                    case "g":
                        GetByGuidMongo().Wait();
                        break;
                    default:
                        break;
                }
                
                sw.Stop();

                var elapsedMongo = sw.Elapsed;

                Times["Mongo"].Add(elapsedMongo.Milliseconds);
                
                Console.WriteLine("Finished");
                Console.WriteLine($"Redis time: {elapsedRedis.Milliseconds}");
                Console.WriteLine($"Elastic time: {elapsedElastic.Milliseconds}");
                Console.WriteLine($"Mongo time: {elapsedMongo.Milliseconds}");

                Console.WriteLine("Press i - initialize, r - range test, d - id test, g - guid test, e - exit");
                Console.WriteLine("Press key...");
                key = Console.ReadLine();
            }

                Console.WriteLine($"AVG Elastic: {Times["Elastic"].Sum() / Times["Elastic"].Count()}");
                Console.WriteLine($"AVG Redis: {Times["Redis"].Sum() / Times["Redis"].Count()}");
                Console.WriteLine($"AVG Mongo: {Times["Mongo"].Sum() / Times["Mongo"].Count()}");
        }

        static async Task RunRedis()
        {
            foreach (var obj in objects)
            {
                await redisService.AddAsync(obj);
            }
        }

        static async Task RunElastic()
        {
            foreach (var obj in objects)
            {
                await elasticService.AddAsync(obj);
            }
        }
        
        static async Task RunMonngo()
        {
            foreach (var obj in objects)
            {
                await mongoService.AddAsync(obj);
            }
        }

        static async Task GetByDatesRedis()
        {
            var tasks = new List<Task<List<Model>>>(NumberOfRuns);
            for (var i = 0; i < NumberOfRuns; i++)
            {
                var result =  redisService.GetByDatesAsync(startDate, endDate);
                tasks.Add(result);
            }

            await Task.WhenAll(tasks);
            if (tasks[0].Result.Count < 190) throw new Exception();
        }

        static async Task GetByDatesElastic()
        {
            var tasks = new List<Task<List<Model>>>(NumberOfRuns);
            for (var i = 0; i < NumberOfRuns; i++)
            {
                var result =  elasticService.GetByDatesAsync(startDate, endDate);
                tasks.Add(result);
            }
            
            await Task.WhenAll(tasks);
            if (tasks[0].Result.Count < 190) throw new Exception();
        }
        
        static async Task GetByDatesMongo()
        {
            var tasks = new List<Task<List<Model>>>(NumberOfRuns);
            for (var i = 0; i < NumberOfRuns; i++)
            {
                var result =  mongoService.GetByDatesAsync(startDate, endDate);
                tasks.Add(result);
            }
            
            await Task.WhenAll(tasks);
            if (tasks[0].Result.Count < 190) throw new Exception();
        }
        
        static async Task GetByIdRedis()
        {
            var tasks = new List<Task<Model>>(NumberOfRuns);
            for (var i = 0; i < NumberOfRuns; i++)
            {
                var result =  redisService.GetByIdAsync(Id);
                tasks.Add(result);
            }

            await Task.WhenAll(tasks);
        }

        static async Task GetByIdElastic()
        {           
            var tasks = new List<Task<Model>>(NumberOfRuns);
            for (var i = 0; i < NumberOfRuns; i++)
            {
                var result = elasticService.GetByIdAsync(Id);         
                tasks.Add(result);
            }

            await Task.WhenAll(tasks);
        }
        
        static async Task GetByIdMongo()
        {           
            var tasks = new List<Task<Model>>(NumberOfRuns);
            for (var i = 0; i < NumberOfRuns; i++)
            {
                var result = mongoService.GetByIdAsync(Id);         
                tasks.Add(result);
            }

            await Task.WhenAll(tasks);
        }
        
        static async Task GetByGuidRedis()
        {
            var tasks = new List<Task<Model>>(NumberOfRuns);
            for (var i = 0; i < NumberOfRuns; i++)
            {
                var result =  redisService.GetByGuidAsync(Guid);
                tasks.Add(result);
            }
            
            await Task.WhenAll(tasks);
        }

        static async Task GetByGuidElastic()
        {
            var tasks = new List<Task<Model>>(NumberOfRuns);
            for (var i = 0; i < NumberOfRuns; i++)
            {
                var result =  elasticService.GetByGuidAsync(Guid);
                tasks.Add(result);
            }
            
            await Task.WhenAll(tasks);
        }
        
        static async Task GetByGuidMongo()
        {
            var tasks = new List<Task<Model>>(NumberOfRuns);
            for (var i = 0; i < NumberOfRuns; i++)
            {
                var result =  mongoService.GetByGuidAsync(Guid);
                tasks.Add(result);
            }
            
            await Task.WhenAll(tasks);
        }
    }
}