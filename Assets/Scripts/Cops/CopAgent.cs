using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CopAgent : MonoBehaviour
{
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
                if (value == CopState.Attacking) target = Player.transform.position + Player.Velocity * predictionTime;
            }
        }
    }

    /// <summary>
    /// The offset of the agent relative to the player when it isn't attacking
    /// </summary>
    public Vector3 Offset { get; set; }

    public CarController Player { get; set; }

    Vector3 target;

    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }

    void Update()
    {
        if (agent.isOnOffMeshLink)
        {
            Vector3 endPos = agent.currentOffMeshLinkData.endPos + Vector3.up * agent.baseOffset;
            agent.transform.position = Vector3.MoveTowards(agent.transform.position, endPos, agent.speed * Time.deltaTime);
            if (agent.transform.position == endPos) agent.CompleteOffMeshLink();
        }
        if (CurrentCopState != CopState.Attacking) target = Player.transform.position + Offset;
        if (agent.destination != target) agent.SetDestination(target);
        if (CurrentCopState != CopState.Driving && Vector3.Distance(transform.position, target) <= completionDistance) CopManager.Instance.MoveAgentToNextState();
    }
}
