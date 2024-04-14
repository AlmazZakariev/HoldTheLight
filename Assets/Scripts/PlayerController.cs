using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
//TODO: баг. Если прыгнуть в самом начале, прыжок не кончается
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
    // Ключ добавлен вместе с реакцией на платформу.
    // Переводим на ложь, пока на платформе. И вместе с этим отключаем движенеи вниз
    // После выхода из платформы возвращаем обратно на истину.
    private bool falling = true;

    // gameManager для управления количеством света от подбора батарейки. 
    private GameManager gameManager;
    // для вызова атаки во время прыжка
    private PlayerAttack playerAttackScript;

    //для звуков
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
        // Свободное падения вниз     
        if (!jumpStats.IsJumping()&&falling)
        {
            if (playerRb.velocity.y<-maxSpeed)
            {
                jumpStats.SetZeroVelocity(-currentMaxSpeed);
            }
            
            //transform.Translate(Vector3.down * Time.deltaTime * speed);
            //playerRb.AddForce(Vector3.down * speed, ForceMode2D.Force);
        }
        // Активация прыжка, сам прыжок исполняется из FixedUpdate
        if (!jumpStats.IsNeedMakeJump(false) && Input.GetKeyDown(KeyCode.Space)&& !jumpStats.IsJumping())
        {
            jumpStats.SetNeedMakeJump();
        }
       
        // Барьер слева
        if (transform.position.x < -xRange)
        {
            transform.position = new Vector3(-xRange, transform.position.y, transform.position.z);
        }
        // Барьер справа
        if (transform.position.x > xRange)
        {
            transform.position = new Vector3(xRange, transform.position.y, transform.position.z);
        }
        // Перемещение в стороны
        horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * Time.deltaTime * horizontalInput * movingSpeed);

        //Активация ускорения вниз
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
        // выключение всплывающего окошка
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
        // Проверяем закончился ли импульс прыжка, чтобы продолжить падение.
        if (jumpStats.IsJumping())
        {
            JumpingEnded();
        }
        // Исполнение прыжка
        if (jumpStats.IsNeedMakeJump(true)&&!jumpStats.IsJumping())
        {
            MakeOneJump(jumpForce);
        }
        if (jumpStats.IsForcingDown())
        {
            playerRb.AddForce(Vector2.down * forceDownSpeed, ForceMode2D.Force);
        }
        
        // Запоминаем координату Y для определения направления
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
            

            // атака
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
        // Для проверки попадания на платформу
        if (collision.gameObject.CompareTag("Platform"))
        {
            falling = false;
        }
 
        // Для проверки удара с врагом
        if (collision.gameObject.CompareTag("Enemy"))
        {
            
            gameManager.GameOver();
        }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Для проверки выхода из платформы
        if (collision.gameObject.CompareTag("Platform"))
        {
            falling = true;
            
        }

        
    }
   
}
// Класс для определения направления полёта плеера
// Надо тестить больше, но вроде Direction2 работает норм
class LastYPos
{
    float PrevY { get; set; }
    float PrevPrevY { get; set; }
    // При каждом update меняем позапрошлую координату Y на прошлую, а в прошлую записываем текущую. 
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
// Класс хранит все переменные, связанные с прыжком
class JumpStats
{
    Rigidbody2D PlayerRb { get; set; }
    // Состояние прыжка в данные момент
    bool Jumping { get; set; }
    // Для активации прыжка
    bool NeedMakeJump { get; set; }
    public bool ForcingDown { get; set; } = false;
    bool NeedForceDown { get; set; }
    public JumpStats(Rigidbody2D playerRb)
    {
        PlayerRb = playerRb;
        Jumping = false;
        NeedMakeJump = false;
    }
    // Вызываем при начале прыжка.
    // Включется гравитация и состояние прыжка
    public void TurnOn()
    {
        //PlayerRb.gravityScale = gravityMod;
        SetZeroVelocity();
        Jumping = true;      
    }
    // Вызываем при завершении прыжка
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
    // Возвращаем состояние прыжка
    public bool IsJumping()
    {
        return Jumping;
    }
    // Возвращаем ключ активации прыжка
    public bool IsNeedMakeJump(bool reset)
    {
        // reset - ебейший костыль
        // Если вызывается reset = false, то мы просто возвращаем ключ
        // Если вызывается reset = true,  то возвращаем состояние NeedMakeJump
        // если NeedMakeJump = true, то меняем на false
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