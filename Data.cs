using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace job_allocator
{
    class Agent
    {
        public string Name { get; set; }
        public int MaxUsers { get; set; }
        public int MaxJobs { get; set; }
        public List<Job> RunningJobs { get; set; }

        public int AvailableUsers {
            get
            {
                var sum = RunningJobs.Sum(j => j.Users);

                return MaxUsers - sum;
            }
        }

        public int AvailableJobs {
            get
            {
                return MaxJobs - RunningJobs.Count;
            }
        }

        public bool AddJob(int users, int id, string prefix)
        {
            if (users > AvailableUsers)
                return false;

            if (AvailableJobs < 1)
                return false;

            RunningJobs.Add(new Job
            {
                Id = id,
                Name = $"{prefix}{id}",
                Users = users
            });

            return true;
        }
    }

    class Config
    {
        public List<Agent> Agents { get; set; }
    }

    class Job
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int Users { get; set; }
    }
}
