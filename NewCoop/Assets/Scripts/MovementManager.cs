using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : Librariy
{
    [Header("-----Move Options-----")]
    [SerializeField] float MainSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float decceleration;
    [SerializeField] float velPower;

    [Header("-----Jump Options-----")]
    [SerializeField] float fallingGravityScale;
    [SerializeField] float gravityScale;
    [SerializeField] float jumpAmount;
    [SerializeField] float CoyotoTime = 0.2f;
    [SerializeField] float CoyotoTimeCounter;
    [SerializeField] float JumpBuffer = 0.2f;
    [SerializeField] float JumpBufferCounter;
    [SerializeField] float jumpCutMultiplayer;
    [SerializeField] Vector2 checkPosSize;
    [SerializeField] Transform checkPos;
    [SerializeField] LayerMask layerMask;
    private bool IsGrouned;
    private bool IsJumped;


    [Header("----Componenets-----")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private MovementBehaviour movementBehaviour;

    private void Update()
    {
        if (JumpBufferCounter > 0 && CoyotoTimeCounter > 0)
        {
            IsJumped = true;
        }

        if (movementBehaviour.Jump == 0)
        {
            JumpBufferCounter = 0f;
        }

        #region Coyota
        if (IsGrouned)
        {
            CoyotoTimeCounter = CoyotoTime;
        }
        else
        {
            CoyotoTimeCounter -= Time.deltaTime;
        }
        #endregion

        #region Buffering
        if (movementBehaviour.Jump == 1)
        {
            JumpBufferCounter = JumpBuffer;
        }
        else
        {
            JumpBufferCounter -= Time.deltaTime;
        }
        #endregion
    }

    void FixedUpdate()
    {
        #region Run
        //Direction
        float moveInput = movementBehaviour.x;
        //Calculat�ng direction we want to move in and our desired velocity
        float targetSpeed = moveInput * MainSpeed;
        //Calculeting difference between current velocity and desired velocity
        float speedDif = targetSpeed - rb.velocity.x;
        //This part choosing is need acceleration or deccelaration
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        //Calculeting movement value
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        //Moving
        rb.velocity = new Vector2(moveInput * MainSpeed, rb.velocity.y);
        //Moving with forces
        rb.AddForce(moveInput * Vector2.right);
        #endregion

        #region Jump
        IsGrouned = Physics2D.OverlapBox(checkPos.position, checkPosSize, 0, layerMask);
        if (IsJumped)
        {
            AddJumpForce();
            IsJumped = false;
        }
        OnJump();
        #endregion
    }


    private void AddJumpForce()
    {
        rb.velocity += Vector2.up * jumpAmount;
    }

    private void OnJump()
    {
        if (rb.velocity.y > 5)
        {
            rb.gravityScale = gravityScale * fallingGravityScale;
        }
        if(rb.velocity.y == 0)
        {
            rb.gravityScale = gravityScale;
        }
    }
}