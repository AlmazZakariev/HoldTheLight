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
        // ��������� ������� ����     
        if (!jumpStats.IsJumping())
        {
            transform.Translate(Vector3.down * Time.deltaTime * speed);
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

    }
    private void FixedUpdate()
    {
        // ��������� ���������� �� ������� ������, ����� ���������� �������.
        if (jumpStats.IsJumping())
        {
            JumpingEnded();
        }
        // ���������� ������
        if (jumpStats.IsNeedMakeJump(true))
        {
            MakeOneJump(jumpForce);
        }
        // ���������� ���������� Y ��� ����������� �����������
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
    public JumpStats(Rigidbody2D playerRb)
    {
        PlayerRb = playerRb;
        Jumping = false;
        NeedMakeJump = false;
    }
    // �������� ��� ������ ������.
    // ��������� ���������� � ��������� ������
    public void TurnOn(float gravityMod)
    {
        PlayerRb.gravityScale = gravityMod;
        Jumping= true;      
    }
    // �������� ��� ���������� ������
    public void TurnOff()
    {
        PlayerRb.gravityScale = 0;
        Jumping = false;
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
}