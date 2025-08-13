using System.Collections;
using UnityEngine;

using SCG = System.Collections.Generic;

public class CopManager : MonoBehaviour
{
    /// <summary>
    /// Singleton
    /// </summary>
    public static CopManager Instance { get; private set; }

    [SerializeField]
    CarController player;
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
    [Tooltip("How long the currently attacking car stays at its driving position")]
    [Min(0)]
    float driveHoldTime;
    [SerializeField]
    [Tooltip("How long the currently attacking car stays at its preparing position")]
    [Min(0)]
    float prepareHoldTime;
    [SerializeField]
    [Tooltip("How long the currently attacking car stays at its attacking position")]
    [Min(0)]
    float attackHoldTime;
    [Header("Offsets")]
    [SerializeField]
    [Tooltip("The offsets from the player that a car can be while driving")]
    SCG::List<Vector3> drivingOffsets = new SCG::List<Vector3>();
    [SerializeField]
    [Tooltip("The offsets from the player that a car can be while preparing to attack")]
    SCG::List<Vector3> preparingOffsets = new SCG::List<Vector3>();

    /// <summary>
    /// Lock guard to stop agent from moving to next state if not allowed
    /// </summary>
    public bool CanAgentMoveToNextState { get; private set; }

    SCG::List<CopAgent> agents = new SCG::List<CopAgent>();

    CopAgent attackingAgent;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        CanAgentMoveToNextState = true;
    }

    void Start() => StartCoroutine(SpawnCops(delay: spawnDelay));

    IEnumerator SpawnCops(float delay)
    {
        if (drivingOffsets.Count <= 0 ||  preparingOffsets.Count <= 0) yield break;
        yield return new WaitForSeconds(delay);
        if (agents.Count > 0) StartCoroutine(DespawnCops(delay: 0f));
        for (int i = 0; i < Random.Range(minCops, maxCops); i++)
        {
            agents.Add(Instantiate(copPrefab, player.transform.position + drivingOffsets[Random.Range(0, drivingOffsets.Count)], Quaternion.identity));
            agents[i].Player = player;
        }
        ChangeAttackingAgent();
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

    IEnumerator ChangeAgentState(float holdTime, Vector3 newOffset, CopAgent.CopState newCopState)
    {
        CanAgentMoveToNextState = false;
        yield return new WaitForSeconds(holdTime);
        CanAgentMoveToNextState = true;
        if (agents.Count <= 0 || attackingAgent == null) yield break;
        attackingAgent.Offset = newOffset;
        attackingAgent.CurrentCopState = newCopState;
        if (newCopState == CopAgent.CopState.Driving) ChangeAttackingAgent();
    }

    void ChangeAttackingAgent()
    {
        if (agents.Count <= 0) return;
        attackingAgent = agents[Random.Range(0, agents.Count)];
        MoveAgentToNextState(caller: this);
    }

    /// <summary>
    /// Change the current state of the attacking agent to the next state in the state machine
    /// </summary>
    /// <param name="caller">The class calling this method</param>
    public void MoveAgentToNextState(Object caller)
    {
        if (caller == null || attackingAgent == null) return; // Don't do any logic if there is no caller or attacking agent
        if (attackingAgent.CurrentCopState == CopAgent.CopState.Driving && caller != this) return; // Only allow the manager to change a driving agent
        if (!CanAgentMoveToNextState) return;
        float holdTime = attackingAgent.CurrentCopState switch {
            CopAgent.CopState.Driving => driveHoldTime,
            CopAgent.CopState.Engaging => 0f, // Instantly switch from engaging to preparing
            CopAgent.CopState.Preparing => prepareHoldTime,
            CopAgent.CopState.Attacking => attackHoldTime,
            CopAgent.CopState.Disengaging => 0f, // Instantly switch from disengaging to driving
            _ => 0f, // Fallback value, should in theory never be needed
        };
        Vector3 newOffset = attackingAgent.CurrentCopState switch {
            CopAgent.CopState.Driving => preparingOffsets.Count > 0 ? preparingOffsets[Random.Range(0, preparingOffsets.Count)] : attackingAgent.Offset, // Move to the prepare position for engaging if possible
            CopAgent.CopState.Engaging => attackingAgent.Offset, // Stay at the same offset for preparing
            CopAgent.CopState.Preparing => Vector3.zero, // Move to where player is for attacking
            CopAgent.CopState.Attacking => drivingOffsets.Count > 0 ? drivingOffsets[Random.Range(0, drivingOffsets.Count)] : attackingAgent.Offset, // Move to the drive position for disengaging if possible
            CopAgent.CopState.Disengaging => attackingAgent.Offset, // Stay at the same offset for driving
            _ => Vector3.zero // Fallback value, should in theory never be needed
        };
        CopAgent.CopState newCopState = attackingAgent.CurrentCopState switch {
            CopAgent.CopState.Driving => CopAgent.CopState.Engaging, // Switch from driving to engaging
            CopAgent.CopState.Engaging => CopAgent.CopState.Preparing, // Switch from engaging to preparing
            CopAgent.CopState.Preparing => CopAgent.CopState.Attacking, // Switch from preparing to attacking
            CopAgent.CopState.Attacking => CopAgent.CopState.Disengaging, // Switch from attacking to disengaging
            CopAgent.CopState.Disengaging => CopAgent.CopState.Driving, // Switch from disengaging to driving
            _ => CopAgent.CopState.Driving // Fallback value, should in theory never be needed
        };
        StartCoroutine(ChangeAgentState(holdTime: holdTime, newOffset: newOffset, newCopState: newCopState));
    }
}
