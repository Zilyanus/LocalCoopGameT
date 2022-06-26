using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : Librariy
{
    [Space(10)]
    [Header("-----Physical Settings-----")]
    //R-G-B-A
    [BackgroundColor(1f, 0f, 0f, 1f)] [Range(-50f, 50f)] [SerializeField] float maxFallGravity;
    [BackgroundColor(1f, 0f, 0f, 1f)] [Range(-50f, 50f)] [SerializeField] float minFallGravity;
    [BackgroundColor(1f, 0f, 0f, 1f)] [Range(0.005f, 100f)] public float ClampGravityMin;
    [BackgroundColor(1f, 0f, 0f, 1f)] [Range(0.005f, 100f)] public float ClampGravityMax;
    [BackgroundColor(1f, 0f, 0f, 1f)] public bool IsCanMove = true;
    [BackgroundColor(1f, 0f, 0f, 1f)] [SerializeField] Transform _graph;


    [BackgroundColor(0f, 1f, 0f, 1f)] [Range(1f,100f)] public float MainSpeed;
    [BackgroundColor(0f, 1f, 0f, 1f)] [Range(1f, 50f)] public float acceleration;
    [BackgroundColor(0f, 1f, 0f, 1f)] [Range(1f, 50f)] public float decceleration;
    [BackgroundColor(0f, 1f, 0f, 1f)] [Range(0.005f, 1f)] public float velPower;
    [BackgroundColor(0f, 1f, 0f, 1f)] [Range(1f, 10f)] public float fallingGravityScale;
    [BackgroundColor(0f, 1f, 0f, 1f)] [Range(1f, 10f)] public float gravityScale;

    [Space(10)]
    [Header("-----Move Options With one axe-----")]
    [Space(30)]
    [BackgroundColor(0f, 1f, 0f, 1f)] [Range(1f, 100f)] public float AxeMainSpeed_1;
    [BackgroundColor(0f, 1f, 0f, 1f)] [Range(1f, 50f)] public float AxeAcceleration_1;
    [BackgroundColor(0f, 1f, 0f, 1f)] [Range(1f, 50f)] public float AxeDecceleration_1;

    [Space(10)]
    [Header("-----Move Options With two axe-----")]
    [Space(30)]
    [BackgroundColor(0f, 1f, 0f, 1f)] [Range(1f, 100f)] public float AxeMainSpeed_2;
    [BackgroundColor(0f, 1f, 0f, 1f)] [Range(1f, 50f)] public float AxeAcceleration_2;
    [BackgroundColor(0f, 1f, 0f, 1f)] [Range(1f, 50f)] public float AxeDecceleration_2;


    [Space(10)]
    [Header("-----Jump Settings Without axe-----")]
    [Space(30)]
    [BackgroundColor(0f, 1f, 0f, 1f)] [Range(1f, 100f)] public float jumpAmount;
    [BackgroundColor(0f, 1f, 0f, 1f)] [Range(1f, 10f)] public float JumpCutTimer;
    [BackgroundColor(0f, 1f, 0f, 1f)] [Range(0.005f, 1f)] public float jumpCutMultiplier;
    [BackgroundColor(0f, 1f, 0f, 1f)] [Range(0.005f, 1f)] public float jumpTime;
    [HideInInspector] public float jumpTimeCounter;
    [HideInInspector] public Vector2 checkPosSize;
    [HideInInspector] public bool isGrounded = true;
    [HideInInspector] public bool isJumped;
    [BackgroundColor(0f, 1f, 0f, 1f)] public int totalJump;
    [BackgroundColor(0f, 1f, 0f, 1f)] public int jumpCounter;
    [BackgroundColor(0f, 1f, 0f, 1f)] public Transform checkPos;
    [BackgroundColor(0f, 1f, 0f, 1f)] public LayerMask layerMask;


    [Space(10)]
    [Header("-----Jump Settings With One Axe-----")]
    [Space(30)]
    [BackgroundColor(0f, 1f, 1f, 1f)] [Range(1f, 100f)] public float AxejumpAmount_1;

    [Space(10)]
    [Header("-----Jump Settings With Two Axe-----")]
    [Space(30)]
    [BackgroundColor(0f, 1f, 1f, 1f)] [Range(1f, 100f)] public float AxejumpAmount_2;


    private float _jumpAmount, _MainSpeed, _Accelerration, _Deacceleration;


    [Space(10)]
    [Header("-----Coyote Time Settings-----")]
    [Space(30)]

    [BackgroundColor(0f, 1f, 1f, 1f)] [Range(0.005f, 1f)] public float CoyotoTime;
    [BackgroundColor(0f, 1f, 1f, 1f)] [Range(1f, 100f)] public float CoyoteAmount;
    [HideInInspector] public bool coyoteJump;
    [HideInInspector] public float coyoteTimeCounter;

    [Space(10)]
    [Header("-----Buffering Settings-----")]
    [Space(30)]
    [BackgroundColor(0f, 1f, 1f, 1f)] [Range(0.005f, 1f)] public float BufferTime;
    [BackgroundColor(0f, 1f, 1f, 1f)] [Range(0.005f, 100f)] public float BufferAmount;
    [HideInInspector] public bool buffering = false;

    [Space(10)]
    [Header("-----Dash Settings-----")]
    [Space(20)]
    [BackgroundColor(0f, 1f, 1f, 1f)] [Range(0.005f, 100f)] public float DashDistance;
    [HideInInspector] public bool IsCanDash;
    [HideInInspector] public bool isDashing;
    [HideInInspector] public float JumpCutTimeCounter;
    public int dashCount;

    [Space(10)]
    [Header("-----Double Jump Settings-----")]
    [Space(20)]
    [BackgroundColor(0f, 1f, 1f, 1f)] [Range(1f, 100f)] public float doubleJumpAmount;

    [Space(10)]
    [Header("----Componenets-----")]
    [Space(20)]
    [BackgroundColor(1f, 0f, 1f, 1f)] [SerializeField] public Rigidbody2D rb;
    [SerializeField] private BoxCollider2D boxC2D;
    [SerializeField] private MovementBehaviour Input;
    [SerializeField] private CombatManager combatManager;
    private void Start()
    {
        JumpCutTimeCounter = JumpCutTimer;
    }

    private void Update()
    {
        CalculatingStatues();
    }

    void FixedUpdate()
    {
        #region Run
        if (IsCanMove)
        {
            if (!isDashing) Runing();
        }
        #endregion

        #region Physical Settings
        if (!isDashing) rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, minFallGravity, maxFallGravity));
        
        #endregion
    }

    #region Direction Of Player
    private void DirectionOfPlayer()
    {
        if (rb.velocity.x > 0.01f)
        {
            _graph.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (rb.velocity.x < -0.01f)
        {
            _graph.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

    }
    #endregion

    #region Run Settings
    private void Runing()
    {
        //Direction
        float moveInput = Input.x;
        //Calculating direction we want to move in and our desired velocity
        float targetSpeed = moveInput * _MainSpeed;
        //Calculeting difference between current velocity and desired velocity
        float speedDif = targetSpeed - rb.velocity.x;
        //This part choosing is need acceleration or deccelaration
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _Accelerration : _Deacceleration;
        //Calculeting movement value
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        //Moving with forces
        rb.AddForce(movement * Vector2.right);
        DirectionOfPlayer();
    }
    #endregion

    #region Axe Settings
    public void CalculatingStatues()
    {
        if (combatManager.HasAxe)
        {
            if (combatManager.AxeCount == 1)
            {
                _MainSpeed = AxeMainSpeed_1;
                _Accelerration = AxeAcceleration_1;
                _Deacceleration = AxeDecceleration_1;
                _jumpAmount = AxejumpAmount_1;
                totalJump = 1;
            }
            else if (combatManager.AxeCount == 2)
            {
                _MainSpeed = AxeMainSpeed_2;
                _Accelerration = AxeAcceleration_2;
                _Deacceleration = AxeDecceleration_2;
                _jumpAmount = AxejumpAmount_2;
                totalJump = 1;
            }
        }
        else if (!combatManager.HasAxe)
        {
            _MainSpeed = MainSpeed;
            _Accelerration = acceleration;
            _Deacceleration = decceleration;
            _jumpAmount = jumpAmount;
            totalJump = 2;
        }
    }
    #endregion

    #region Adding Normal Jump Force
    public void AddJumpForce()
    {
        rb.velocity = Vector2.zero;
        _AddVelocity(rb, _jumpAmount);
    }
    #endregion

    #region Adding Double Jump Force
    public void AddDoubleJumpForce()
    {
        rb.velocity = Vector2.zero;
        _AddVelocity(rb, doubleJumpAmount);
    }
    #endregion

    #region Adding Coyote Jump Force
    public void AddCoyoteTimeForce()
    {
        rb.velocity = Vector2.zero;
        _AddVelocity(rb, CoyoteAmount);
    }
    #endregion

    #region Add Jump Buffer Force
    public void AddBufferJumpForce()
    {
        rb.velocity = Vector2.zero;
        _AddVelocity(rb, BufferAmount);
    }
    #endregion

    #region On Jumping
    public void OnJump()
    {
        if (rb.velocity.y > 3)
        {
            rb.gravityScale = gravityScale * fallingGravityScale;
        }
        if (rb.velocity.y == 0)
        {
            rb.gravityScale = gravityScale;
        }
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = gravityScale * fallingGravityScale;
        }
    }
    #endregion    
    
    #region Jump Cut
    public void JumpCut()
    {
        if (rb.velocity.y > 0)
        {
            rb.AddForce(Vector2.down * rb.velocity.y * (1 - JumpCutTimeCounter), ForceMode2D.Impulse);
        }
    }
    #endregion
}
