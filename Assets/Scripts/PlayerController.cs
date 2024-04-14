using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
//TODO: ���. ���� �������� � ����� ������, ������ �� ���������
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerRb;

    public float currentSpeed;
    public float maxSpeed = 10.0f;
    public float forcingDownMaxSpeed = 20;

    public float movingSpeed = 10.0f;
    private float horizontalInput;
    public float xRange = 10.0f;
    private bool facingLeft = true;

    public float jumpForce = 10.0f;
    public float jumpCost;

    public float forceDownSpeed = 3f;
    public float forcingTime = 2;

    public float attackLightTime;
    public GameObject attackLight;

    public Animator animator;
    private LastYPos lastYPos = new LastYPos();
    private JumpStats jumpStats;

    private bool gameOver = false;
    // ���� �������� ������ � �������� �� ���������.
    // ��������� �� ����, ���� �� ���������. � ������ � ���� ��������� �������� ����
    // ����� ������ �� ��������� ���������� ������� �� ������.
    private bool falling = true;

    // gameManager ��� ���������� ����������� ����� �� ������� ���������. 
    private GameManager gameManager;
    // ��� ������ ����� �� ����� ������
    private PlayerAttack playerAttackScript;

    //��� ������
    public AudioSource jumpSound;
    // Start is called before the first frame update


    public void onAttacked()
    {
        gameManager.GameOver();
    }

    void Start()
    {
    
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerRb = GetComponent<Rigidbody2D>();
        jumpStats = new JumpStats(playerRb);

        playerAttackScript = GameObject.Find("Player").GetComponent<PlayerAttack>();
        //messageManager = GameObject.Find("Message").GetComponent<MessageManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (gameOver)
        {
            return;
        }
        var currentMaxSpeed = SetCurrentMaxSpeed();
        currentSpeed = playerRb.velocity.y;
        // ��������� ������� ����     
        if (!jumpStats.IsJumping()&&falling)
        {
            if (playerRb.velocity.y<-maxSpeed)
            {
                jumpStats.SetZeroVelocity(-currentMaxSpeed);
            }
            
            //transform.Translate(Vector3.down * Time.deltaTime * speed);
            //playerRb.AddForce(Vector3.down * speed, ForceMode2D.Force);
        }
        // ��������� ������, ��� ������ ����������� �� FixedUpdate
        if (!jumpStats.IsNeedMakeJump(false) && Input.GetKeyDown(KeyCode.Space)&& !jumpStats.IsJumping())
        {
            jumpStats.SetNeedMakeJump();
        }
       
        // ������ �����
        if (transform.position.x < -xRange)
        {
            transform.position = new Vector3(-xRange, transform.position.y, transform.position.z);
        }
        // ������ ������
        if (transform.position.x > xRange)
        {
            transform.position = new Vector3(xRange, transform.position.y, transform.position.z);
        }
        // ����������� � �������
        horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * Time.deltaTime * horizontalInput * movingSpeed);

        //��������� ��������� ����
        if (!jumpStats.IsNeedFroceDown(false) && Input.GetKeyDown(KeyCode.S) && !jumpStats.IsForcingDown())
        {
            jumpStats.ForcingDown = true;
            Invoke("StopForcingDown", forcingTime);
        }

        animator.SetBool("OnGround", Mathf.Abs(currentSpeed)<0.01);
        animator.SetFloat("HorizontalMove", Mathf.Abs(horizontalInput));
        if(horizontalInput> 0&& facingLeft)
        {
            Flip();
        }
        if (horizontalInput < 0 && !facingLeft)
        {
            Flip();
        }
        // ���������� ������������ ������
        if (Input.GetKeyDown(KeyCode.F) && Time.timeScale != 1)
        {
            MessageManager.disableMessageEvent?.Invoke();

        }
    }
    private void Flip()
    {
        facingLeft = !facingLeft;

        Vector2 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    private float SetCurrentMaxSpeed()
    {
        if (jumpStats.IsForcingDown())
        {
            return forcingDownMaxSpeed;
        }
        return maxSpeed;
    }

    private void FixedUpdate()
    {
        if (gameOver)
        {
            return;
        }
        // ��������� ���������� �� ������� ������, ����� ���������� �������.
        if (jumpStats.IsJumping())
        {
            JumpingEnded();
        }
        // ���������� ������
        if (jumpStats.IsNeedMakeJump(true)&&!jumpStats.IsJumping())
        {
            MakeOneJump(jumpForce);
        }
        if (jumpStats.IsForcingDown())
        {
            playerRb.AddForce(Vector2.down * forceDownSpeed, ForceMode2D.Force);
        }
        
        // ���������� ���������� Y ��� ����������� �����������
        lastYPos.SetNext(transform.position.y);
    }
    private void StopForcingDown()
    {
        jumpStats.ForcingDown = false;
    }
    private void MakeFroceDown(float forceSpeed)
    {
        playerRb.AddForce(Vector2.down * forceSpeed, ForceMode2D.Impulse);
        //playerRb.AddForce(Vector3.down * forceSpeed, ForceMode2D.Force);
    }
    private void MakeOneJump(float speed)
    {
        if (gameManager.AddLight(-jumpCost))
        {
            jumpSound.Play();

            jumpStats.TurnOn();
            playerRb.AddForce(Vector3.up * speed, ForceMode2D.Impulse);
            

            // �����
            playerAttackScript.attackLightActive = true;
            attackLight.SetActive(true);
            Invoke("TurnOffAttackLight", attackLightTime);
        }      
    }
    private void TurnOffAttackLight()
    {
        attackLight.SetActive(false);
        playerAttackScript.attackLightActive = false;
    }
    private void JumpingEnded()
    {
        if (!lastYPos.Direction2(transform.position.y))
        {
           // playerRb.gravityScale = 0;
            jumpStats.TurnOff();
        }
    }
    public void GameOver()
    {
        gameOver = true;
        //speed = 0;
        movingSpeed = 0;
        jumpForce = 0;
        jumpStats.SetZeroVelocity();
        playerRb.gravityScale = 0;
        animator.gameObject.SetActive(false);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ��� �������� ��������� �� ���������
        if (collision.gameObject.CompareTag("Platform"))
        {
            falling = false;
        }
 
        // ��� �������� ����� � ������
        if (collision.gameObject.CompareTag("Enemy"))
        {
            
            gameManager.GameOver();
        }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        // ��� �������� ������ �� ���������
        if (collision.gameObject.CompareTag("Platform"))
        {
            falling = true;
            
        }

        
    }
   
}
// ����� ��� ����������� ����������� ����� ������
// ���� ������� ������, �� ����� Direction2 �������� ����
class LastYPos
{
    float PrevY { get; set; }
    float PrevPrevY { get; set; }
    // ��� ������ update ������ ����������� ���������� Y �� �������, � � ������� ���������� �������. 
    public void SetNext(float next)
    {
        PrevPrevY = PrevY;
        PrevY = next;
    }
    //returns true when jumping, false when falling
    public bool Direction()
    {
        return PrevY > PrevPrevY;
    }
    public bool Direction2(float currY)
    {
        return currY > PrevPrevY;
    }
}
// ����� ������ ��� ����������, ��������� � �������
class JumpStats
{
    Rigidbody2D PlayerRb { get; set; }
    // ��������� ������ � ������ ������
    bool Jumping { get; set; }
    // ��� ��������� ������
    bool NeedMakeJump { get; set; }
    public bool ForcingDown { get; set; } = false;
    bool NeedForceDown { get; set; }
    public JumpStats(Rigidbody2D playerRb)
    {
        PlayerRb = playerRb;
        Jumping = false;
        NeedMakeJump = false;
    }
    // �������� ��� ������ ������.
    // ��������� ���������� � ��������� ������
    public void TurnOn()
    {
        //PlayerRb.gravityScale = gravityMod;
        SetZeroVelocity();
        Jumping = true;      
    }
    // �������� ��� ���������� ������
    public void TurnOff()
    {
        //PlayerRb.gravityScale = 0;
        Jumping = false;
        //SetZeroVelocity();
    }
    public void SetZeroVelocity(float speed = 0)
    {
        var velocity = PlayerRb.velocity;
        velocity.y = speed;
        PlayerRb.velocity = velocity;
    }
    // ���������� ��������� ������
    public bool IsJumping()
    {
        return Jumping;
    }
    // ���������� ���� ��������� ������
    public bool IsNeedMakeJump(bool reset)
    {
        // reset - ������� �������
        // ���� ���������� reset = false, �� �� ������ ���������� ����
        // ���� ���������� reset = true,  �� ���������� ��������� NeedMakeJump
        // ���� NeedMakeJump = true, �� ������ �� false
        if (reset)
        {   if (NeedMakeJump)
            {
                NeedMakeJump = false;
                return true;
            }
            return false;
        }
        else
        {
            return NeedMakeJump;
        }

    }

    public void SetNeedMakeJump()
    {
        NeedMakeJump = true;
    }

    public bool IsNeedFroceDown(bool reset)
    {
        if (reset)
        {
            if (NeedForceDown)
            {
                NeedForceDown = false;
                return true;
            }
            return false;
        }
        else
        {
            return NeedForceDown;
        }
    }
    public bool IsForcingDown()
    {
        return ForcingDown;
    }
  
}