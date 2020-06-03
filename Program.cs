using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace job_allocator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(string.Join(" ", args));

            var config = ReadConfig(args);
            PrintConfig(config);
        }

        static void PrintConfig(Config config)
        {
            foreach(var a in config.Agents)
            {
                Console.WriteLine($"{a.Name, 3} {a.Users, 4} {a.Jobs, 2}");
            }
        }

        static Config ReadConfig(string[] args)
        {
            int agents = 20;
            int users = 1000;
            int jobs = 10;

            return GenerateNewConfig(agents, users, jobs);
        }

        static Config GenerateNewConfig(int agents, int users, int jobs)
        {
            var config = new Config
            {
                Agents = new List<Agent>()
            };

            var rand = new Random();

            for(var i = 0; i < agents; i++)
            {
                var agent = new Agent
                {
                    Users = rand.Next(users / 2, users + 1),
                    Jobs = rand.Next(jobs / 2, jobs + 1),
                    Name = $"A{i}"
                };

                config.Agents.Add(agent);
            }

            return config;
        }
    }

    class Agent
    {
        public string Name { get; set; }
        public int Users { get; set; }
        public int Jobs { get; set; }
    }

    class Config
    {
        public List<Agent> Agents { get; set; }
    }
}
