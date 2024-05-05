using UnityEngine;
using UnityEngine.AI;

namespace CyberpunkJam.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshMovement : MonoBehaviour
    {
        private NavMeshAgent agent;

        public float RemainingDistance => agent.remainingDistance;

        private void Awake()
        {
            Setup();
        }

        public void SetPath(NavMeshPath path)
        {
            agent.isStopped = false;

            agent.SetPath(path);
        }

        public void Stop()
        {
            agent.isStopped = true;
        }

        public NavMeshPath GetPathToTarget(Transform target)
        {
            agent.isStopped = false;

            if (agent.enabled == false)
            {
                return null;
            }

            var navMeshPath = new NavMeshPath();

            agent.CalculatePath(target.position, navMeshPath);

            agent.isStopped = true;

            return navMeshPath;
        }

        private void Setup()
        {
            agent = GetComponent<NavMeshAgent>();
            
            agent.isStopped = false;
        }
    }
}