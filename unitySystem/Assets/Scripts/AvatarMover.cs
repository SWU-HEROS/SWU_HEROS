using UnityEngine;
using UnityEngine.AI;

public class AvatarMover : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3[] path;
    private int currentIndex = 0;

    public void Initialize(Vector3[] targetPath, float moveSpeed)
    {
        if (targetPath == null || targetPath.Length == 0)
        {
            Debug.LogWarning("Target path is null or empty! Movement not possible.");
            return;
        }

        agent = GetComponent<NavMeshAgent>() ?? gameObject.AddComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.autoBraking = false;  // Continuous movement
        agent.radius = 0.17f;
        agent.acceleration = 16f;   // Natural acceleration
        agent.angularSpeed = 360f;  // Natural rotation
        agent.avoidancePriority = Random.Range(10, 50);
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;  // Collision avoidance
        agent.stoppingDistance = 2.5f; // Expand target recognition range


        // Current position adjusted to NavMesh
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
        else
        {
            Debug.LogWarning("Avatar not spawned on NavMesh!");
            return;
        }

        // Target path adjusted to NavMesh
        path = new Vector3[targetPath.Length];
        for (int i = 0; i < targetPath.Length; i++)
        {
            if (NavMesh.SamplePosition(targetPath[i], out NavMeshHit hitPos, 2f, NavMesh.AllAreas))
            {
                path[i] = hitPos.position;
            }
            else
            {
                Debug.LogWarning($"Target position {i} is not on the NavMesh!");
            }
        }

        currentIndex = 0;
        if (path.Length > 0 && path[0] != Vector3.zero)
        {
            agent.SetDestination(path[0]);
        }
    }

    void Update()
    {
        if (path == null || currentIndex >= path.Length || agent == null) return;

        // Set next destination
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            currentIndex++;
            if (currentIndex < path.Length)
            {
                agent.SetDestination(path[currentIndex]);
            }
        }

        // Add rotation logic
        if (agent.hasPath)
        {
            Vector3 direction = agent.steeringTarget - transform.position;
            direction.y = 0;  // Prevent up-and-down rotation

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // Smooth rotation
            }
        }
    }

}
