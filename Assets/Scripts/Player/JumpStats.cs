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
    // Класс хранит все переменные, связанные с вертикальным перемещением player
    public PlayerState PlayerState { get; set; }
    // Ключ активации прыжка
    public bool NeedMakeJump { get; set; }
    // Ключ активации ускорения вниз
    public bool NeedForceDown { get; set; }

    public JumpStats(Rigidbody2D playerRb)
    {
        PlayerState= PlayerState.Falling;
    }
    
    // Возвращаем ключ активации прыжка
    public bool IsNeedMakeJumpAndJumpIfNeed()
    {
        // Делаем прыжок, если это возможно
        if (NeedMakeJump)
        {
            NeedMakeJump = false;
            return true;
        }
        return false;
    }
    // Возвращаем ключ активации ускорения вниз
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
