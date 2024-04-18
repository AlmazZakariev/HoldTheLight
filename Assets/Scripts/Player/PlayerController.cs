using UnityEngine;
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
    private JumpStats jumpStats;


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
        playerAttackScript = GameObject.Find("Player").GetComponent<PlayerAttack>();
        cameraFollowScript = GameObject.Find("Main Camera").GetComponent<CameraFollow>();

        playerRb = GetComponent<Rigidbody2D>();
        jumpStats =  new JumpStats(playerRb);
    }

    // Update is called once per frame
    void Update()
    {
        //контроль максимальной скорости
        var currentMaxSpeed = SetCurrentMaxSpeed();
        currentSpeed = playerRb.velocity.y;     
        if (playerRb.velocity.y<-maxSpeed)
        {
            SetYVelocity(-currentMaxSpeed);
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
        if (playerRb.velocity.y<=0)
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