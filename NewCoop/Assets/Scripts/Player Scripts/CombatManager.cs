using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class CombatManager : MonoBehaviour
{
    [Space(10)]
    [Header("-----Object's Settings-----")]

    [Space(25)]
    [BackgroundColor(0, 1, 0, 1)] [SerializeField] GameObject AxePrefab;
    [BackgroundColor(0, 1, 0, 1)] [SerializeField] SpriteRenderer AxeSprite;
    [BackgroundColor(0, 1, 0, 1)] [SerializeField] Transform AxeSpawner;
    [BackgroundColor(0, 1, 0, 1)] [SerializeField] GameObject[] Arrows;
    [BackgroundColor(0, 1, 0, 1)] [SerializeField] LayerMask AxeMask;
    [BackgroundColor(0, 1, 0, 1)] [SerializeField] Transform MeleeCombatArea;
    [BackgroundColor(0, 1, 0, 1)] [SerializeField] LayerMask MeeleCombatLayerMask;
    [BackgroundColor(0, 1, 0, 1)] [SerializeField] Collider2D HeadCollider;
    [BackgroundColor(0, 1, 0, 1)] [SerializeField] Collider2D PlayerCollider;

    [Space(10)]
    [Header("-----Statues-----")]
    [Space(25)]
    [BackgroundColor(0, 0, 1, 1)] public bool HasAxe = true;
    [BackgroundColor(0, 0, 1, 1)] public int AxeCount;

    [Space(10)]
    [Header("----Components-----")]
    [Space(25)]
    [BackgroundColor(0, 1, 1, 1)] [SerializeField] MovementBehaviour Inputs;
    [BackgroundColor(0, 1, 1, 1)] [SerializeField] MovementManager movementManager;
    GameManager gameManager;

    private bool IsPressing;
    private float Axedirection = 0;
    private GameObject myAxe;
    private bool IsAttackOn;
    private bool IsMeeleCombat;

    public string ControlShame;
    public InputDevice ControlDevice;

    private void Start()
    {
       // gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        Shooting();
    }

    #region Shooting
    private void Shooting()
    {
        if (HasAxe)
        {
            IfHasAxe();
            MeeleCombat();
        }
    }
    #endregion

    #region Meele Combat Settings
    private void MeeleCombat()
    {
        if (Inputs.MeleeDown > 0)
        {
            Collider2D IsTouchToPlayer = Physics2D.OverlapBox(MeleeCombatArea.position, MeleeCombatArea.localScale, default, MeeleCombatLayerMask);

            if (IsTouchToPlayer != null && IsTouchToPlayer)
            {
                Kill(IsTouchToPlayer.gameObject);
            }
        }
        else
        {
            IsMeeleCombat = false;
        }
    }

    #endregion

    #region Mouse Calculate

    #region Mouse Down For Aim
    private void InputMouseButtonDown()
    {
        IsPressing = true;
        movementManager.IsCanMove = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Axedirection = ShootingDirection();
        IsAttackOn = true;
    }
    #endregion

    #region Mouse Up For Aim
    private void InputMouseButtonUp()
    {
        if (IsPressing)
        {
            //Fýrlatma ayarlarý burasý
            AxeSprite.enabled = false;

            CreateAe();
            SupportArrows();
            calculating();
        }
    }

    #endregion

    #endregion

    #region Axe Calculate

    #region Axe Calculating
    private void IfHasAxe()
    {
        if (Inputs.Attack == 1)
        {
            //eðim alma ayarlarý burasý
            InputMouseButtonDown();
        }
        if (IsAttackOn)
        {
            if (Inputs.Attack == 0)
            {
                InputMouseButtonUp();
            }
        }

    }
    #endregion

    #region Throw Axe Settings
    private void CreateAe()
    {
        myAxe = Instantiate(AxePrefab, AxeSpawner.position, Quaternion.identity);
        myAxe.GetComponent<AxeOwnManager>().Inputs = Inputs;
        myAxe.GetComponent<AxeOwnManager>()._WhichHead = HeadCollider;
        myAxe.GetComponent<AxeOwnManager>()._WhichPlayer = PlayerCollider;
        Rigidbody2D AxeRb = myAxe.GetComponent<Rigidbody2D>();
        myAxe.GetComponent<Transform>().Rotate(0, 0, Axedirection);
    }

    private void SupportArrows()
    {
        for (int i = 0; i < Arrows.Length; i++)
        {
            Arrows[i].SetActive(false);
        }
    }

    private void calculating()
    {
        AxeCount--;
        if (AxeCount == 0)
        {
            HasAxe = false;
        }
        IsAttackOn = false;
        this.Wait(0.2f, () => movementManager.IsCanMove = true);
    }

    #endregion

    #endregion

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

    public void Kill(GameObject Player)
    {
        Player.SetActive(false);
        
        if(Player != gameObject)
        {
            gameManager.ChangeScore(int.Parse(gameObject.name[1].ToString()), 1);
        }
        else
        {
            gameManager.ChangeScore(int.Parse(gameObject.name[1].ToString()), -1);
        }
    }

    public void ResetPlayer()
    {
        HasAxe = true;
        GetComponent<PlayerInput>().SwitchCurrentControlScheme(ControlShame, InputSystem.devices[0]);
        AxeCount = 1;
    }
}
