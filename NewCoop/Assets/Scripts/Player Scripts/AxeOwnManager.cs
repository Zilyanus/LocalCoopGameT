using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeOwnManager : MonoBehaviour
{

    /// <summary>
    /// Bu Kod dizimi sadece atýlan baltaya özgüdür. 
    /// Eðer oyuncu ile iliþkilendirilmesi gerekiyorsa CombatManager scripti kullanýlmalýdýr.
    /// </summary>

    [Space(10)]
    [Header("-----Axe Direction Settings-----")]
    [Space(20)]
    [BackgroundColor(1, 0, 0, 1)] [Range(0.005f, 100)] [SerializeField] float directionForce;
    [BackgroundColor(1, 0, 0, 1)] [Range(0.005f, 100)] [SerializeField] float directionForceCuter;
    [BackgroundColor(1, 0, 0, 1)] [Range(0.005f, 100)] [SerializeField] float AxeForce;
    [BackgroundColor(1, 0, 0, 1)] [Range(0.005f, 100)] [SerializeField] float AxeForceCuter;
    [BackgroundColor(1, 0, 0, 1)] [Range(0.005f, 10)] [SerializeField] float AxeGravityCounter;


    [Space(10)]
    [Header("-----Axe Raycast Settings-----")]
    [Space(20)]
    [BackgroundColor(0, 1, 0, 1)] [SerializeField] LayerMask _maskForGround;
    [BackgroundColor(0, 1, 0, 1)] [SerializeField] LayerMask _maskForHead;
    [BackgroundColor(0, 1, 0, 1)] [SerializeField] LayerMask _maskForPlayer;
    [BackgroundColor(0, 1, 0, 1)] [SerializeField] LayerMask _maskForTest;
    [BackgroundColor(0, 1, 0, 1)] [Range(0.001f, 10)] [SerializeField] float axeHeadRange;
    [BackgroundColor(0, 1, 0, 1)] [Range(0.001f, 10)] [SerializeField] float axeBoolHeadRange;
    [BackgroundColor(0, 1, 0, 1)] [Range(0.001f, 10)] [SerializeField] float axeGroundRange;
    [BackgroundColor(0, 1, 0, 1)] [Range(0.001f, 10)] [SerializeField] float axePlayerRange;
    [BackgroundColor(0, 1, 0, 1)] [Range(0.001f, 10)] [SerializeField] float axeTestRange;
    private bool _IsTouchingToGround;
    private bool _IsTouchToHead;
    private bool _IsTouchToPlayer;
    private bool _Test;
    private float AxeForceCounter;

    [Space(10)]
    [Header("-----Axe Physics-----")]
    [Space(20)]
    [BackgroundColor(0, 1, 0, 1)] [SerializeField] Rigidbody2D rbAxe;


    [Space(10)]
    [Header("-----Components-----")]
    [Space(20)]
    [HideInInspector] public MovementBehaviour Inputs;
    [HideInInspector] public Collider2D _WhichHead;
    [HideInInspector] public Collider2D _WhichPlayer;
    private Collider2D _ThisHead;
    private Collider2D _ThisPlayer;



    [Header("-----Debug-----")]
    [SerializeField] bool forGround;
    [SerializeField] bool forPlayer;
    [SerializeField] bool forTest;
    [SerializeField] bool forHead;

    private void Start()
    {
        AxeForceCounter = AxeForce - 50;
        
        rbAxe.gravityScale = 0;
    }

    #region Add Force To Angle and Rotate
    private void AddForceToAxe()
    {
        if (_IsTouchingToGround) { rbAxe.velocity = Vector2.zero; return; }
        else
        {
            if (Inputs.r < 0)
            {
                transform.Rotate(new Vector3(0, 0, -Inputs.r * directionForce) * Time.deltaTime * 10);
                Debug.Log("RL yönünde force var");
            }
            if (Inputs.r > 0)
            {
                transform.Rotate(new Vector3(0, 0, -Inputs.r * directionForce) * Time.deltaTime * 10);
                Debug.Log("RR yönünde force var");
            }
        }
    }
    #endregion

    #region Draw Gizmos
    private void OnDrawGizmos()
    {
        
        if (forPlayer)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, axePlayerRange);
        }
        if (forHead)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, axeBoolHeadRange);
        }
        if (forHead)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, axeHeadRange);
        }
        if (forGround)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, axeGroundRange);
        }
    }
    #endregion


    void Update()
    {
        AxePhysics();
        GetTakeBackTheAxe();
        rbAxe.velocity = new Vector2(Mathf.Clamp(rbAxe.velocity.x, -20, 20), Mathf.Clamp(rbAxe.velocity.y, -20, 20));
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
        if (!_IsTouchingToGround) return;

        _IsTouchToPlayer = Physics2D.OverlapCircle(transform.position, axePlayerRange, _maskForPlayer);
        if (_IsTouchToPlayer)
        {
            Collider2D _WhichPlayer = Physics2D.OverlapCircle(transform.position, axePlayerRange, _maskForPlayer);
            _ThisPlayer = _WhichPlayer;
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
        else if (_ThisPlayer == null) return;

        CombatManager playerComponent = _ThisPlayer.GetComponent<CombatManager>();
        if (playerComponent.AxeCount < 2)
        {
            playerComponent.HasAxe = true;
            playerComponent.AxeCount++;
            Destroy(gameObject);
        }
    }
    #endregion

    #region Axe Physics
    private void AxePhysics()
    {
        if (_IsTouchingToGround) AxeOnGround();
        else AxeOnFlying();
    }
    #endregion

    #region Axe On Fly
    private void AxeOnFlying()
    {
        AddForceToAxe();
        rbAxe.velocity += new Vector2(transform.up.x, transform.up.y) * Time.deltaTime * AxeForce * 10;
        this.Wait(AxeGravityCounter, () => rbAxe.gravityScale = 0.7f);
        directionForce = (directionForce >= 0 && !_IsTouchingToGround) ? directionForce - Time.deltaTime * directionForceCuter : directionForce;
        AxeForce = (AxeForce >= 0 && !_IsTouchingToGround) ? AxeForce - Time.deltaTime * AxeForceCounter : AxeForce;
        if (_IsTouchToHead) Destroy(gameObject);
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
