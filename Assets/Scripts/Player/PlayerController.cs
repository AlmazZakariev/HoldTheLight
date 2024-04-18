using UnityEngine;
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
    public float forceDownCoolDown;
    private bool forcingDownAvalable = true;

    public float attackLightTime;
    public GameObject attackLight;

    public Animator animator;
    private LastYPos lastYPos = new LastYPos();
    private JumpStats jumpStats;

    private bool gameOver = false;

    // gameManager для управления количеством света от подбора батарейки. 
    private GameManager gameManager;
    // для вызова атаки во время прыжка
    private PlayerAttack playerAttackScript;

    //для звуков
    public AudioSource jumpSound;
    public AudioSource pickUpSound;
    
    private CameraFollow cameraFollowScript;


    public void onAttacked()
    {
        gameManager.GameOver();
    }
    private void Awake()
    {
        Time.timeScale = 1.0f;
    }
    // Start is called before the first frame update
    void Start()
    {
    
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerRb = GetComponent<Rigidbody2D>();
        jumpStats =  new JumpStats(playerRb);

        playerAttackScript = GameObject.Find("Player").GetComponent<PlayerAttack>();
        //messageManager = GameObject.Find("Message").GetComponent<MessageManager>();
        cameraFollowScript = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
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
        //if (jumpStats.PlayerState == PlayerState.Falling)
        {
            if (playerRb.velocity.y<-maxSpeed)
            {
                SetYVelocity(-currentMaxSpeed);
            }
        }
        // Активация прыжка, сам прыжок исполняется из FixedUpdate
        if (Input.GetKeyDown(KeyCode.Space) && !jumpStats.NeedMakeJump && jumpStats.PlayerState != PlayerState.Jumping && cameraFollowScript.VerticalScene)
        {
            jumpStats.NeedMakeJump = true; 
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
        var playerVelocity = playerRb.velocity;
        playerVelocity.x = horizontalInput * movingSpeed;
        playerRb.velocity = playerVelocity;

        //Активация ускорения вниз
        if (!jumpStats.IsNeedFroceDownAndForceDownIfNeed() && Input.GetKeyDown(KeyCode.S) && jumpStats.PlayerState != PlayerState.ForcingDown && cameraFollowScript.VerticalScene&&forcingDownAvalable)
        {
            MakeFroceDown();
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
        if (jumpStats.PlayerState == PlayerState.ForcingDown)
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
        if (jumpStats.PlayerState == PlayerState.Jumping)
        {
            JumpingEnded();
        }
        // Исполнение прыжка
        if (jumpStats.IsNeedMakeJumpAndJumpIfNeed()&& jumpStats.PlayerState != PlayerState.Jumping)
        {
            MakeOneJump(jumpForce);
        }
        // форс фниз
        if (jumpStats.PlayerState == PlayerState.ForcingDown)
        {
            playerRb.AddForce(Vector2.down * forceDownSpeed, ForceMode2D.Force);

        }
        // Запоминаем координату Y для определения направления
        lastYPos.SetNext(transform.position.y);
    }
    private void MakeForceAble()
    {
        forcingDownAvalable = true;
    }
    private void StopForcingDown()
    {
        jumpStats.PlayerState = PlayerState.Falling;
    }
    private void MakeFroceDown()
    {
        jumpStats.PlayerState = PlayerState.ForcingDown;
        Invoke("StopForcingDown", forcingTime);
        forcingDownAvalable = false;
        Invoke("MakeForceAble", forceDownCoolDown);
    }
    private void MakeOneJump(float speed)
    {
        if (gameManager.AddLight(-jumpCost))
        {
            jumpSound.Play();

            SetYVelocity();
            jumpStats.PlayerState = PlayerState.Jumping;
            playerRb.AddForce(Vector3.up * speed, ForceMode2D.Impulse);

            // атака
            MakeAttack();
        }      
    }
    private void MakeAttack()
    {
        playerAttackScript.attackLightActive = true;
        attackLight.SetActive(true);
        Invoke("TurnOffAttackLight", attackLightTime);
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
            jumpStats.PlayerState = PlayerState.Falling;
        }
    }
    public void SetYVelocity(float speed = 0)
    {
        var velocity = playerRb.velocity;
        velocity.y = speed;
        playerRb.velocity = velocity;
    }
    public void GameOver()
    {
        gameOver = true;
        animator.gameObject.SetActive(false);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Для проверки попадания на платформу
        if (collision.gameObject.CompareTag("Platform"))
        {
            jumpStats.PlayerState = PlayerState.Graunded;
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
            jumpStats.PlayerState = PlayerState.Falling;
            
        } 
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Для проверки подбора батарейки
        if (collider.gameObject.CompareTag("Battery"))
        {
            pickUpSound.Play();
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