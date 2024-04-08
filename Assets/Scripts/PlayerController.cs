using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerRb;
    public float speed = 10.0f;
    public float jumpForce = 10.0f;
    public float gravityModifier = 1.0f;

    public float horizontalInput;
    public float movingSpeed = 10.0f;
    public float xRange = 10.0f;

    private LastYPos lastYPos = new LastYPos();
    private JumpStats jumpStats;

    // Start is called before the first frame update
    void Start()
    {
        
        playerRb = GetComponent<Rigidbody2D>();
        jumpStats = new JumpStats(playerRb);
    }

    // Update is called once per frame
    void Update()
    {
        // Свободное падения вниз     
        if (!jumpStats.IsJumping())
        {
            transform.Translate(Vector3.down * Time.deltaTime * speed);
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

    }
    private void FixedUpdate()
    {
        // Проверяем закончился ли импульс прыжка, чтобы продолжить падение.
        if (jumpStats.IsJumping())
        {
            JumpingEnded();
        }
        // Исполнение прыжка
        if (jumpStats.IsNeedMakeJump(true))
        {
            MakeOneJump(jumpForce);
        }
        // Запоминаем координату Y для определения направления
        lastYPos.SetNext(transform.position.y);
    }
    private void MakeOneJump(float speed)
    {
        playerRb.AddForce(Vector3.up * speed, ForceMode2D.Impulse);
        jumpStats.TurnOn(gravityModifier);
    }
    private void JumpingEnded()
    {
        if (!lastYPos.Direction2(transform.position.y))
        {
            playerRb.gravityScale = 0;
            jumpStats.TurnOff();
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
    public JumpStats(Rigidbody2D playerRb)
    {
        PlayerRb = playerRb;
        Jumping = false;
        NeedMakeJump = false;
    }
    // Вызываем при начале прыжка.
    // Включется гравитация и состояние прыжка
    public void TurnOn(float gravityMod)
    {
        PlayerRb.gravityScale = gravityMod;
        Jumping= true;      
    }
    // Вызываем при завершении прыжка
    public void TurnOff()
    {
        PlayerRb.gravityScale = 0;
        Jumping = false;
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
}