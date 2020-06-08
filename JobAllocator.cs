using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace job_allocator
{
    class JobAllocator
    {
        public Config AllocateJobs(Config config, out string message)
        {
            if (config.TotalUsersForTests == 0)
            {
                message = "";
                return config;
            }

            if (config.AvailableUsers < config.TotalUsersForTests)
            {
                message = "Not enough available users";
                return null;
            }

            if (config.AvailableJobs < config.Tests.Count)
            {
                message = "Not enough available jobs";
                return null;
            }

            var clonedConfig = config.Clone();
            
            return TryAllocate(clonedConfig, out message) ? clonedConfig : null;
        }

        List<Agent> GetAvailableAgents(Config config)
        {
            return config.Agents
                .Where(a => a.AvailableUsers > 0 && a.AvailableJobs > 0).
                OrderByDescending(a => a.AvailableJobs).ThenBy(a => a.AvailableUsers)
                .ToList();
        }

        bool IsEmpty(int[] a)
        {
            return a.All(v => v == 0);
        }

        bool TryAllocate(Config config, out string message)
        {
            var tests = config.Tests.Select(t => t).ToArray();

            var index = 0;
            
            foreach(var n in tests)
            {
                var availableAgents = GetAvailableAgents(config);
                
                if (!TryAllocateTest(availableAgents, n, index, out message))
                    return false;

                index++;
            }

            message = "";
            return true;
        }

        bool TryAllocateTest(List<Agent> agents, int n, int index, out string message)
        {
            message = $"Cannot allocate job T{index} with {n} users";

            var availableAgents = agents;
            var unallocatedAmount = n;

            while(unallocatedAmount > 0 && availableAgents.Count > 0)
            {
                var k = unallocatedAmount / availableAgents.Count;
                if (k == 0)
                    k = 1;

                var min_available_users = agents.Min(a => a.AvailableUsers);
                k = k > min_available_users ? min_available_users : k;

                foreach(var agent in availableAgents)
                {
                    if (unallocatedAmount == 0)
                        break;

                    if(agent.AddUsersToJob(k, index, "T"))
                    {
                        unallocatedAmount -= k;
                    }
                }

                availableAgents = availableAgents.Where(a => a.AvailableUsers > 0).ToList();
            }

            return unallocatedAmount == 0;
        }
    }
}
