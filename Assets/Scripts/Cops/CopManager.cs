using System.Collections;
using UnityEngine;

public class CopManager : MonoBehaviour
{
    /// <summary>
    /// Singleton
    /// </summary>
    public static CopManager Instance { get; private set; }

    [Header("Spawning")]
    [SerializeField]
    CopAgent copPrefab;
    [SerializeField]
    [Range(1, 3)]
    int minCops;
    [SerializeField]
    [Range(3, 5)]
    int maxCops;
    [Header("Timing")]
    [SerializeField]
    [Tooltip("How long until the manager spawns cops")]
    [Min(0)]
    float spawnDelay;
    [SerializeField]
    [Tooltip("How long cops until the cops are despawned after being spawned")]
    [Min(0)]
    float despawnDelay;

    System.Collections.Generic.List<CopAgent> agents = new System.Collections.Generic.List<CopAgent>();

    CopAgent attackingAgent;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start() => StartCoroutine(SpawnCops(delay: spawnDelay));

    IEnumerator SpawnCops(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (agents.Count > 0) StartCoroutine(DespawnCops(delay: 0f));
        for (int i = 0; i < Random.Range(minCops, maxCops); i++) agents.Add(Instantiate(copPrefab));
        attackingAgent = agents[Random.Range(0, agents.Count)];
        StartCoroutine(ChangeAgentState(delay: 0f, offset: Vector3.zero, newCopState: CopAgent.CopState.Engaging)); // Do an actual delay and offset at some point
        StartCoroutine(DespawnCops(delay: despawnDelay));
    }

    IEnumerator DespawnCops(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (agents.Count <= 0) yield break;
        foreach (CopAgent agent in agents) Destroy(agent.gameObject);
        agents.Clear();
        SpawnCops(delay: spawnDelay);
    }

    IEnumerator ChangeAgentState(float delay, Vector3 offset, CopAgent.CopState newCopState)
    {
        yield return new WaitForSeconds(delay);
        if (agents.Count <= 0 || attackingAgent == null) yield break;
        attackingAgent.Offset = offset;
        attackingAgent.CurrentCopState = newCopState;
    }

    /// <summary>
    /// Change the current state of the attacking agent to the next state in the state machine
    /// </summary>
    public void MoveAgentToNextState()
    {
        float delay = attackingAgent.CurrentCopState {
            CopAgent.CopState.Engaging => 0f, // Do an actual delay at some point
            CopAgent.CopState.Preparing => 0f, // Do an actual delay at some point
            CopAgent.CopState.Attacking => 0f, // Do an actual delay at some point
            CopAgent.CopState.Disengaging => 0f, // Do an actual delay at some point
            _ => 0f // Fallback value, should in theory never be needed
        };
        Vector3 offset = attackingAgent.CurrentCopState {
            CopAgent.CopState.Engaging => Vector3.zero, // Do an actual offset at some point
            CopAgent.CopState.Preparing => Vector3.zero, // Do an actual offset at some point
            CopAgent.CopState.Attacking => Vector3.zero, // Do an actual offset at some point
            CopAgent.CopState.Disengaging => Vector3.zero, // Do an actual offset at some point
            _ => Vector3.zero // Fallback value, should in theory never be needed
        };
        CopAgent.CopState newCopState = attackingAgent.CurrentCopState {
            CopAgent.CopState.Engaging => CopAgent.CopState.Preparing,
            CopAgent.CopState.Preparing => CopAgent.CopState.Attacking,
            CopAgent.CopState.Attacking => CopAgent.CopState.Disengaging
            CopAgent.CopState.Disengaging => CopAgent.CopState.Engaging
            _ => Vector3.Driving // Fallback value, should in theory never be needed
        };
        if (attackingAgent.CurrentCopState == CopAgent.CopState.Disengaging)
        {
            attackingAgent.Offset = Vector3.zero; // Do an actual offset at some point
            attackingAgent.CurrentCopState = CopAgent.CopState.Driving;
            attackingAgent = agents[Random.Range(0, agents.Count)];
        }
        StartCoroutine(ChangeAgentState(delay: delay, offset: offset, newCopState: newCopState));
    }
}
