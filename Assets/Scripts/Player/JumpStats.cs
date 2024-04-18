using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Falling,
    Graunded,
    Jumping,
    ForcingDown
}
public class JumpStats 
{
    // ����� ������ ��� ����������, ��������� � ������������ ������������ player
    public PlayerState PlayerState { get; set; }
    // ���� ��������� ������
    public bool NeedMakeJump { get; set; }
    // ���� ��������� ��������� ����
    public bool NeedForceDown { get; set; }

    public JumpStats(Rigidbody2D playerRb)
    {
        PlayerState= PlayerState.Falling;
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
