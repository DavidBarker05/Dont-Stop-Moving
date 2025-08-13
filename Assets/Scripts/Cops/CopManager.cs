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
    [Tooltip("How long until the cops are despawned after being spawned")]
    [Min(0)]
    float despawnDelay;
    [SerializeField]
    [Tooltip("How long until the current attacking car begins to engage the player (drive forward towards the player to attack)")]
    [Min(0)]
    float engageDelay;
    [SerializeField]
    [Tooltip("How long the current attacking car prepares before attacking the player")]
    [Min(0)]
    float prepareDelay;
    [Tooltip("How long until the current attacking car starts disengaging from the player (drive away from the player after attacking)")]
    [Min(0)]
    float disengageDelay;

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
        StartCoroutine(ChangeAgentState(delay: engageDelay, offset: Vector3.zero, newCopState: CopAgent.CopState.Engaging)); // Do an actual delay and offset at some point
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
        float delay = attackingAgent.CurrentCopState switch {
            CopAgent.CopState.Engaging => prepareDelay,
            CopAgent.CopState.Attacking => disengageDelay,
            CopAgent.CopState.Disengaging => engageDelay,
            _ => 0f // No delay for attacking, also fallback value (fallback should in theory never happen)
        };
        Vector3 offset = attackingAgent.CurrentCopState switch {
            CopAgent.CopState.Engaging => Vector3.zero, // Do an actual offset at some point
            CopAgent.CopState.Preparing => Vector3.zero, // Do an actual offset at some point
            CopAgent.CopState.Attacking => Vector3.zero, // Do an actual offset at some point
            CopAgent.CopState.Disengaging => Vector3.zero, // Do an actual offset at some point
            _ => Vector3.zero // Fallback value, should in theory never be needed
        };
        CopAgent.CopState newCopState = attackingAgent.CurrentCopState switch {
            CopAgent.CopState.Engaging => CopAgent.CopState.Preparing,
            CopAgent.CopState.Preparing => CopAgent.CopState.Attacking,
            CopAgent.CopState.Attacking => CopAgent.CopState.Disengaging,
            CopAgent.CopState.Disengaging => CopAgent.CopState.Engaging,
            _ => CopAgent.CopState.Driving // Fallback value, should in theory never be needed
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
