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
    float spawnTime;
    [SerializeField]
    [Tooltip("How long cops will pursue the player for")]
    [Min(0)]
    float pursuitTime;

    System.Collections.Generic.List<CopAgent> agents = new System.Collections.Generic.List<CopAgent>();

    CopAgent attackingAgent;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
    }

    void Update()
    {
    }

    IEnumerator SpawnCops()
    {
        yield return new WaitForSeconds(spawnTime);
        if (agents.Count > 0) DespawnCops();
        for (int i = 0; i < Random.Range(minCops, maxCops); i++) agents.Add(Instantiate(copPrefab));
        DespawnCops();
    }

    IEnumerator DespawnCops()
    {
        yield return new WaitForSeconds(pursuitTime);
        if (agents.Count <= 0) yield break;
        foreach (CopAgent agent in agents) Destroy(agent.gameObject);
        agents.Clear();
        SpawnCops();
    }

    public void MoveAgentToNextState()
    {
        switch (attackingAgent.CurrentCopState)
        {
            case (CopAgent.CopState.Disengaging):
                attackingAgent.Offset = Vector3.zero; // Do an actual offset at some point
                attackingAgent.CurrentCopState = CopAgent.CopState.Driving;
                attackingAgent = agents[Random.Range(0, agents.Count)];
                attackingAgent.Offset = Vector3.zero; // Do an actual offset at some point
                attackingAgent.CurrentCopState = CopAgent.CopState.Engaging;
                break;
        }
    }
}
