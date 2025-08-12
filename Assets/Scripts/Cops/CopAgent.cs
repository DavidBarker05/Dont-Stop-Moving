using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CopAgent : MonoBehaviour
{
    // Here for now until an actual player is added
    struct TempPlayer { public Transform transform; public Vector3 velocity; }

    /// <summary>
    /// The states that a cop agent can be in while chasing the player
    /// </summary>
    public enum CopState
    {
        /// <summary>
        /// Idle — just following behind the player
        /// </summary>
        Driving,
        /// <summary>
        /// Moving up next to the player to prepare for attack
        /// </summary>
        Engaging,
        /// <summary>
        /// Buildup phase, lining up the ram
        /// </summary>
        Preparing,
        /// <summary>
        /// The actual ram attack
        /// </summary>
        Attacking,
        /// <summary>
        /// Backing away after the attack
        /// </summary>
        Disengaging
    }

    [SerializeField]
    [Tooltip("How far ahead the cop car looks to attack the player")]
    float predictionTime;
    [SerializeField]
    [Tooltip("How far the cop car must be to be considered at its target destination")]
    float completionDistance;

    CopState _state;
    /// <summary>
    /// The current state of the cop agent
    /// </summary>
    public CopState CurrentCopState
    {
        get => _state;
        set
        {
            if (_state != value)
            {
                _state = value;
                if (value == CopState.Attacking) target = player.transform.position + player.velocity * predictionTime;
            }
        }
    }

    /// <summary>
    /// The offset of the agent relative to the player when it isn't attacking
    /// </summary>
    public Vector3 Offset { get; set; }

    TempPlayer player;

    Vector3 target;

    NavMeshAgent agent;

    void Awake() => agent = GetComponent<NavMeshAgent>();

    void Update()
    {
        if (CurrentCopState != CopState.Attacking) target = player.transform.position + Offset;
        agent.SetDestination(target);
        if (CurrentCopState != CopState.Driving && Vector3.Distance(transform.position, target) <= completionDistance) CopManager.Instance.MoveAgentToNextState();
    }
}
