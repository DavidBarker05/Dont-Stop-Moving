using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
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
    [SerializeField]
    [Min(0f)]
    float maxSpeed;
    [SerializeField]
    [Min(0f)]
    float acceleration;
    [SerializeField]
    [Min(0f)]
    [Tooltip("The amount of speed the player loses when they impact a cop.")]
    float impactSpeedLoss;

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
                if (value == CopState.Attacking)
                {
                    target = Player.transform.position + Player.Velocity * predictionTime;
                    canSlowPlayer = true;
                }
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
    bool canSlowPlayer;
    bool failedChangeAfterAttack;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.speed = maxSpeed;
        agent.acceleration = acceleration;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().freezeRotation = true;
        GetComponent<Rigidbody>().useGravity = false;
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
        if (CurrentCopState != CopState.Driving && CopManager.Instance.CanAgentMoveToNextState)
        {
            if (Vector3.Distance(transform.position, target) <= completionDistance || failedChangeAfterAttack) CopManager.Instance.MoveAgentToNextState(caller: this);
            if (failedChangeAfterAttack) failedChangeAfterAttack = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player")) return; // TODO: Make sure player has player tag
        if (!canSlowPlayer) return;
        canSlowPlayer = false; // Only slow the player once
        Player.LoseSpeed(impactSpeedLoss);
        if (CurrentCopState == CopState.Attacking && CopManager.Instance.CanAgentMoveToNextState) CopManager.Instance.MoveAgentToNextState(caller: this);
        else if (!CopManager.Instance.CanAgentMoveToNextState) failedChangeAfterAttack = true;
    }
}
