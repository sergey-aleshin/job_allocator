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

        public bool AddUsersToJob(int users, int id, string prefix)
        {
            var jobName = $"{prefix}{id}";
            var job = RunningJobs.FirstOrDefault(j => j.Name == jobName);

            if (job == null)
                return AddJob(users, id, prefix);

            if (users > AvailableUsers)
                return false;

            job.Users += users;

            return true;
        }
    }

    class Config
    {
        public List<Agent> Agents { get; set; }

        public int TotalUsers
        {
            get
            {
                return Agents.Sum(a => a.MaxUsers);
            }
        }

        public int AvailableUsers
        {
            get
            {
                return Agents.Sum(a => a.AvailableUsers);
            }
        }

        public int TotalJobs
        {
            get
            {
                return Agents.Sum(a => a.MaxJobs);
            }
        }

        public int AvailableJobs
        {
            get
            {
                return Agents.Sum(a => a.AvailableJobs);
            }
        }

        public List<int> Tests { get; set; }

        public int TotalUsersForTests
        {
            get
            {
                if (Tests == null)
                    return 0;

                return Tests.Sum();
            }
        }

        public int MinJobsPerAgent
        {
            get
            {
                return Agents.Min(a => a.RunningJobs.Count);
            }
        }

        internal Config Clone()
        {
            return new Config
            {
                Tests = this.Tests.Select(t => t).ToList(),
                Agents = this.Agents.Select(a => new Agent
                {
                    MaxJobs = a.MaxJobs,
                    MaxUsers = a.MaxUsers,
                    Name = a.Name,
                    RunningJobs = a.RunningJobs.Select(j => new Job
                    {
                        Id = j.Id,
                        Name = j.Name,
                        Users = j.Users
                    }).ToList()
                }).ToList()
            };
        }
    }

    class Job
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int Users { get; set; }
    }
}
