using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using RedisElasticBenchmark;

namespace RedisElasticBenchmark
{
    class Program
    {
        private static readonly RedisService redisService = new RedisService();
        private static readonly ElasticService elasticService = new ElasticService();
        private static readonly List<Model> objects = new List<Model>();

        private const int id = 5000;
        private static Guid guid = Guid.Parse("ff8cc246-1513-4959-83fb-efdc694ba1ba");
        private static readonly DateTime startDate = DateTime.Now;
        private static readonly DateTime endDate = DateTime.Now.AddDays(7);


        static void Main(string[] args)
        {
            Console.WriteLine("Press i - initialize, r - range test, d - id test, g - guid test, e - exit");
            var key = Console.ReadLine();
            while (key != "e")
            {
                Stopwatch sw;

                var number = 10000;
                var startDate = DateTime.Now.AddDays(-180);

                for (int i = 1; i <= number; i++)
                {
                    objects.Add(new Model
                    {
                        Id = i,
                        Date = startDate.AddMinutes(51.84 * i),
                        Name = $"Sample model name {i}",
                        Guid = Guid.NewGuid()
                    });
                }

                // Warm up
                GetByDatesRedis().Wait();
                GetByDatesElastic().Wait();

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

                GetByDatesRedis().Wait();

                sw.Stop();

                var elapsedRedis = sw.Elapsed;



                Console.WriteLine("Finished");
                Console.WriteLine($"Redis time: {elapsedRedis}");
                Console.WriteLine($"Elastic time: {elapsedElastic}");

                Console.WriteLine("Press key...");
                key = Console.ReadLine();
            }
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

        static async Task GetByDatesRedis()
        {
            for (var i = 0; i < 100; i++)
            {
                var result = await redisService.GetByDatesAsync(startDate, endDate);
            }
        }

        static async Task GetByDatesElastic()
        {
            for (var i = 0; i < 100; i++)
            {
                var result = await elasticService.GetByDatesAsync(startDate, endDate);
            }
        }
        
        static async Task GetByIdRedis()
        {
            for (var i = 0; i < 100; i++)
            {
                var result = await redisService.GetByIdAsync(id);
            }
        }

        static async Task GetByIdElastic()
        {
            for (var i = 0; i < 100; i++)
            {
                var result = await elasticService.GetByIdAsync(id);
            }
        }
        
        static async Task GetByGuidRedis()
        {
            for (var i = 0; i < 100; i++)
            {
                var result = await redisService.GetByGuidAsync(guid);
            }
        }

        static async Task GetByGuidElastic()
        {
            for (var i = 0; i < 100; i++)
            {
                var result = await elasticService.GetByGuidAsync(guid);
            }
        }
    }
}