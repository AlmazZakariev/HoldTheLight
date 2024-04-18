using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpStats 
{
    // ����� ������ ��� ����������, ��������� � ������������ ������������ player
    Rigidbody2D PlayerRb { get; set; }
    // ��������� ������ �� ������ ������
    public bool Jumping { get; set; }
    // ���� ��������� ������
    public bool NeedMakeJump { get; set; }
    // ��������� ��������� ���� �� ������ ������
    public bool ForcingDown { get; set; } = false;
    // ���� ��������� ��������� ����
    public bool NeedForceDown { get; set; }

    public JumpStats(Rigidbody2D playerRb)
    {
        PlayerRb = playerRb;
        Jumping = false;
        NeedMakeJump = false;
    }
  
    // �������� ��� ������ ������.
    public void TurnOn()
    {
        SetYVelocity();
        Jumping = true;
    }
    public void SetYVelocity(float speed = 0)
    {
        var velocity = PlayerRb.velocity;
        velocity.y = speed;
        PlayerRb.velocity = velocity;
    }
    // ���������� ���� ��������� ������
    public bool IsNeedMakeJumpAndJumpIfNeed()
    {
        // ������ ������, ���� ��� ��������
        if (NeedMakeJump)
        {
            NeedMakeJump = false;
            return true;
        }
        return false;
    }
    // ���������� ���� ��������� ��������� ����
    public bool IsNeedFroceDownAndForceDownIfNeed()
    {
        if (NeedForceDown)
        {
            NeedForceDown = false;
            return true;
        }
        return false;
    }
}
