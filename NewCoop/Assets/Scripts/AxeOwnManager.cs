using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeOwnManager : MonoBehaviour
{
    [Space(10)]
    [Header("-----Axe Direction Settings-----")]
    [Space(20)]
    [BackgroundColor(1, 0, 0, 1)] [Range(1, 300)] [SerializeField] float directionForce;
    [BackgroundColor(1, 0, 0, 1)] [Range(1, 100)] [SerializeField] float directionForceCuter;
    [BackgroundColor(1, 0, 0, 1)] [Range(1, 300)] [SerializeField] float AxeForce;
    [BackgroundColor(1, 0, 0, 1)] [Range(1, 100)] [SerializeField] float AxeForceCuter;
    [BackgroundColor(1, 0, 0, 1)] [Range(0.005f, 10)] [SerializeField] float AxeGravityCounter;

    [Space(10)]
    [Header("-----Axe Settings-----")]
    [Space(20)]
    [BackgroundColor(0, 1, 0, 1)] [SerializeField] LayerMask _maskForGround;
    [BackgroundColor(0, 1, 0, 1)] [SerializeField] LayerMask _maskForHead;
    [BackgroundColor(0, 1, 0, 1)] [SerializeField] LayerMask _maskForPlayer;
    [BackgroundColor(0, 1, 0, 1)] [SerializeField] Rigidbody2D rbAxe;
    [BackgroundColor(0, 1, 0, 1)] [Range(0.001f, 10)] [SerializeField] float axeHeadRange;
    [BackgroundColor(0, 1, 0, 1)] [Range(0.001f, 10)] [SerializeField] float axeBoolHeadRange;
    [BackgroundColor(0, 1, 0, 1)] [Range(0.001f, 10)] [SerializeField] float axeGroundRange;
    [BackgroundColor(0, 1, 0, 1)] [Range(0.001f, 10)] [SerializeField] float axePlayerRange;
    private bool _IsTouchingToGround;
    private bool _IsTouchToHead;
    private bool _IsTouchToPlayer;
    private float AxeForceCounter;
    


    [Space(10)]
    [Header("-----Components-----")]
    [Space(20)]
    [HideInInspector] public MovementBehaviour Inputs;
    [HideInInspector] public Collider2D _WhichHead;
    private Collider2D _ThisHead;
    private Collider2D _ThisPlayer;

    private void Start()
    {
        AxeForceCounter = AxeForce - 50;
    }

    #region Add Force To Angle and Rotate
    private void AddForceToAxe()
    {
        if (_IsTouchingToGround) return;
        else
        {
            if (Inputs.r < 0)
            {
                transform.Rotate(new Vector3(0, 0, -Inputs.r * directionForce * Time.fixedDeltaTime));
                Debug.Log("RL y�n�nde force var");
            }
            if (Inputs.r > 0)
            {
                transform.Rotate(new Vector3(0, 0, -Inputs.r * directionForce * Time.fixedDeltaTime));
                Debug.Log("RR y�n�nde force var");
            }
        }
    }
    #endregion


    #region Draw Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, axePlayerRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, axeBoolHeadRange);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, axeHeadRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, axeGroundRange);

    }
    #endregion

    void Update()
    {
        AxePhysics();
        AddForceToAxe();
        GetTakeBackTheAxe();
    }

    
    private void FixedUpdate()
    {
        GroundDedection();
        HeadDedection();
        PlayerDedection();
    }

    #region Head Dedection
    private void HeadDedection()
    {
        Collider2D ThisHead = Physics2D.OverlapCircle(transform.position, axeBoolHeadRange, _maskForHead);
        if (ThisHead != _WhichHead)
        {
            _IsTouchToHead = Physics2D.OverlapCircle(transform.position, axeHeadRange, _maskForHead);
            _ThisHead = ThisHead;
        }
    }
    #endregion

    #region Player Dedection
    private void PlayerDedection()
    {
        if (_IsTouchingToGround)
        {
            _IsTouchToPlayer = Physics2D.OverlapCircle(transform.position, axePlayerRange, _maskForPlayer);
            if (_IsTouchToPlayer)
            {
                Collider2D _WhichPlayer = Physics2D.OverlapCircle(transform.position, axePlayerRange, _maskForPlayer);
                _ThisPlayer = _WhichPlayer;
            }
        }
    }
    #endregion

    #region Ground Dedection
    private void GroundDedection()
    {
        _IsTouchingToGround = Physics2D.OverlapCircle(transform.position, axeGroundRange, _maskForGround);
    }
    #endregion

    #region Get Take Axe
    private void GetTakeBackTheAxe()
    {
        if (!_IsTouchingToGround) return;
        if (_ThisPlayer == null) return;
        else
        {
            CombatManager playerComponent = _ThisPlayer.GetComponent<CombatManager>();
            if (playerComponent.AxeCount < 2)
            {
                playerComponent.HasAxe = true;
                playerComponent.AxeCount++;
                Destroy(gameObject);
            }
        }

    }
    #endregion

    #region Axe Physics
    private void AxePhysics()
    {
        if (!_IsTouchingToGround)
        {
            AxeOnFlying();
        }
        else
        {
            AxeOnGround();
        }
    }
    #endregion

    #region Axe On Fly
    private void AxeOnFlying()
    {
        rbAxe.velocity = new Vector2(Mathf.Clamp(rbAxe.velocity.x, -20, 20), Mathf.Clamp(rbAxe.velocity.y, -20, 20));
        rbAxe.velocity += new Vector2(transform.up.x, transform.up.y) * Time.fixedDeltaTime * AxeForce;
        if (AxeForce >= 0) AxeForce -= Time.deltaTime * AxeForceCuter;
        this.Wait(AxeGravityCounter, () => rbAxe.gravityScale = 0.7f);
        if (directionForce >= 0) directionForce -= Time.deltaTime * directionForceCuter;
        if (_IsTouchToHead) Inputs.gameObject.GetComponent<CombatManager>().Kill(_ThisHead.gameObject.transform.parent.gameObject.transform.parent.gameObject);
    }
    #endregion

    #region Axe On Ground
    private void AxeOnGround()
    {
        rbAxe.velocity = Vector2.zero;
        rbAxe.gravityScale = 0;
    }
    #endregion
}
