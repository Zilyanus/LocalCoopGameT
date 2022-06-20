using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CombatManager : MonoBehaviour
{
    [Space(10)]
    [Header("-----Object's Settings-----")]
    [BackgroundColor(0, 1, 0, 1)]
    [Space(25)]
    [SerializeField] GameObject AxePrefab;
    [SerializeField] SpriteRenderer AxeSprite;
    [SerializeField] Transform AxeSpawner;
    [SerializeField] GameObject[] Arrows;
    [SerializeField] LayerMask AxeMask;
    [SerializeField] Transform MeleeCombatArea;
    [SerializeField] LayerMask MeeleCombatLayerMask;

    [Space(10)]
    [Header("-----Statues-----")]
    [Space(25)]
    [BackgroundColor(0,0,1,1)]
    public bool HasAxe = true;
    public int AxeCount;


    [Space(10)]
    [Header("----Components-----")]
    [Space(25)]
    [BackgroundColor(0, 1, 1, 1)]
    [SerializeField] MovementBehaviour Inputs;
    [SerializeField] MovementManager movementManager;

    private bool IsPressing;
    private float Axedirection = 0;
    private GameObject myAxe;
    private bool IsAttackOn;
    private bool IsMeeleCombat;

    private void Update()
    {
        //Debug.Log("Axe count: " + AxeCount);

        #region Shooting
        if (HasAxe)
        {
            if (Inputs.Attack == 1)
            {
                //e�im alma ayarlar� buras�
                IsPressing = true;
                movementManager.IsCanMove = false;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                Axedirection = ShootingDirection();
                
                IsAttackOn = true;
            }
            if (IsAttackOn)
            {
                if (Inputs.Attack == 0)
                {
                    if (IsPressing)
                    {
                        //F�rlatma ayarlar� buras�
                        AxeSprite.enabled = false;
                        myAxe = Instantiate(AxePrefab, transform.position, Quaternion.identity);
                        myAxe.GetComponent<AxeOwnManager>().Inputs = Inputs;
                        Rigidbody2D AxeRb = myAxe.GetComponent<Rigidbody2D>();
                        myAxe.GetComponent<Transform>().Rotate(0, 0, Axedirection);
                        for (int i = 0; i < Arrows.Length; i++)
                        {
                            Arrows[i].SetActive(false);
                        }
                        AxeCount--;
                        if (AxeCount == 0)
                        {
                            HasAxe = false;
                        }
                        IsAttackOn = false;
                        this.Wait(0.2f, () => movementManager.IsCanMove = true);
                    }
                }
            }
            
        }
        #endregion

        #region Melee Combot
        if (HasAxe)
        {
            if (Inputs.MeleeDown > 0)
            {
                IsMeeleCombat = true;
            }
            else
            {
                IsMeeleCombat = false;
            }
        }
        #endregion
    }

    private void FixedUpdate()
    {
        if (IsMeeleCombat)
        {
            Collider2D IsTouchToPlayer = Physics2D.OverlapBox(MeleeCombatArea.position, MeleeCombatArea.localScale, default, MeeleCombatLayerMask);

            if (IsTouchToPlayer != null && IsTouchToPlayer)
            {
                Destroy(IsTouchToPlayer.gameObject);
            }
        }
    }

    #region Shooting direction settings
    private int ShootDirectionSettings()
    {
        if (Inputs.x >= -0.1f && Inputs.x <= 0.1f && Inputs.y > 0.1f)
        {
            return 1;
        }
        if (Inputs.x < -0.1f && Inputs.y > 0.3f)
        {
            return 2;
        }
        if (Inputs.x < -0.1f && Inputs.y >= -0.1f && Inputs.y <= 0.1f)
        {
            return 3;
        }
        if (Inputs.x < -0.1f && Inputs.y < -0.1f)
        {
            return 4;
        }
        if (Inputs.x >= -0.1f && Inputs.x <= 0.1f && Inputs.y < -0.1f)
        {
            return 5;
        }
        if (Inputs.x > 0.3f && Inputs.y < -0.1f)
        {
            return 6;
        }
        if (Inputs.x > 0.3f && Inputs.y >= -0.1f && Inputs.y <= 0.1f)
        {
            return 7;
        }
        if (Inputs.x > 0.3f && Inputs.y > 0.3f)
        {
            return 8;
        }
        else
        {
            return 0;
        }
    }
    #endregion

    #region Shooting Direction
    private float ShootingDirection()
    {
        Debug.ClearDeveloperConsole();
        float a = 0;
        switch (ShootDirectionSettings())
        {
            case 1:
                ArrowSystem(0);
                a = 0;
                break;
            case 2:
                ArrowSystem(1);
                a = 45;
                break;
            case 3:
                ArrowSystem(2);
                a = 90;
                break;
            case 4:
                ArrowSystem(3);
                a = 135;
                break;
            case 5:
                ArrowSystem(4);
                a = 180;
                break;
            case 6:
                ArrowSystem(5);
                a = 225;
                break;
            case 7:
                ArrowSystem(6);
                a = 270;
                break;
            case 8:
                ArrowSystem(7);
                a = 315;
                break;
            case 0:
                ArrowSystem(9);
                break;
            default:
                break;
        }
        return a;
    }
    #endregion

    #region Arrow System
    private void ArrowSystem(int key)
    {
        if (key == 9)
        {
            for (int i = 7; i >= 0; i--)
            {
                Arrows[i].SetActive(false);
            }
        }
        else if (key == 8)
        {
            Arrows[7].SetActive(true);
            int i = key - 1;
            for (; i >= 0; i--)
            {
                Arrows[i].SetActive(false);
            }
        }
        else
        {
            Arrows[key].SetActive(true);
            int i = key + 1;
            int a = key - 1;
            if (key == 0)
            {
                for (; i < Arrows.Length; i++)
                {
                    Arrows[i].SetActive(false);
                }
            }
            else
            {
                for (; i < Arrows.Length; i++)
                {
                    Arrows[i].SetActive(false);
                }
                for (; a >= 0; a--)
                {
                    Arrows[a].SetActive(false);
                }
            }
            
        }

    }
    #endregion
}
