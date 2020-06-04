using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Net.Http.Headers;

namespace job_allocator
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ReadConfig(args);
            
            PrintConfig(config);
        }

        static void PrintConfig(Config config)
        {
            foreach(var a in config.Agents)
            {
                var jobs = string.Join(" ", a.RunningJobs.Select(j => j.Name + '(' + j.Users.ToString() + ')').ToArray());
                Console.WriteLine($"{a.Name, 3} {a.MaxUsers, 4} {a.MaxJobs, 2} {jobs}");
            }
        }

        static Config ReadConfig(string[] args)
        {
            int agents = 20;
            int jobs = 10;

            var readFromFile = false;
            var customAgentNumber = false;
            var fileName = string.Empty;
            int pos = 0;

            while(pos < args.Length)
            {
                if (args[pos].ToLower() == "-f")
                {
                    pos++;

                    if (pos >= args.Length)
                        CloseAppWithError("Missing argument for option -f!", -1);

                    fileName = args[pos];
                    readFromFile = true;
                } else if(args[pos].ToLower() == "-a")
                {
                    pos++;

                    if (pos >= args.Length)
                        CloseAppWithError("Missing argument for option -a!", -1);

                    if (Regex.Match(args[pos], @"^\d+$").Success)
                    {
                        agents = Convert.ToInt32(args[pos]);
                        customAgentNumber = true;
                    } else
                        CloseAppWithError("Argument for option -a should be numeric!", -1);
                }

                pos++;
            }

            if (readFromFile && customAgentNumber)
                CloseAppWithError("You cannot use options -f and -a together!", -1);

            if (!readFromFile)
                return GenerateNewConfig(agents, jobs);

            return ReadConfigFromFile(fileName);
        }

        private static void CloseAppWithError(string message, int code)
        {
            Console.WriteLine(message);
            
            Environment.Exit(code);
        }

        private static Config ReadConfigFromFile(string fileName)
        {
            string[] lines = null;

            try
            {
                lines = File.ReadAllLines(fileName);
            } catch(Exception e)
            {
                CloseAppWithError(string.Format("Error: {0}", e.Message), -2);
            }

            var config = new Config
            {
                Agents = new List<Agent>()
            };

            var counter = 0;
            var jobCounter = 0;

            foreach(var line in lines)
            {
                var match = Regex.Match(line, @"^\s*(\d+):(\d+)\s*(.*)$", RegexOptions.CultureInvariant | RegexOptions.Singleline);
                if (match.Success)
                {
                    var users = match.Groups[1].Value;
                    var jobs = match.Groups[2].Value;
                    var rest = match.Groups[3].Value;

                    var runningJobs = rest.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    var a = new Agent
                    {
                        MaxUsers = Convert.ToInt32(users),
                        MaxJobs = Convert.ToInt32(jobs),
                        Name = $"A{counter}",
                        RunningJobs = new List<Job>()
                    };

                    if (runningJobs.Length > 0)
                    {
                        foreach(var rj in runningJobs)
                        {
                            if (Regex.Match(rj, @"^\d+$").Success && a.AddJob(Convert.ToInt32(rj), jobCounter, "J"))
                            {
                                jobCounter++;
                            }
                        }
                    }

                    config.Agents.Add(a);
                    counter++;
                }
            }

            return config;
        }

        static Config GenerateNewConfig(int agents, int jobs)
        {
            var config = new Config
            {
                Agents = new List<Agent>()
            };

            var jobCounter = 0;
            var agentCapacity = new int[] { 10, 20, 50, 100, 200, 500, 1000, 2000, 5000 };

            var rand = new Random();

            for(var i = 0; i < agents; i++)
            {
                var capacityIndex = rand.Next(0, agentCapacity.Length);

                var agent = new Agent
                {
                    MaxUsers = agentCapacity[capacityIndex],
                    MaxJobs = rand.Next(jobs / 2, jobs + 1),
                    Name = $"A{i}",
                    RunningJobs = new List<Job>()
                };

                var addJobs = rand.Next(0, 2);

                if(addJobs == 1 && agent.AddJob(rand.Next(1, agent.MaxUsers + 1), jobCounter, "J"))
                {
                    jobCounter++;
                }

                config.Agents.Add(agent);
            }

            return config;
        }
    }
}
