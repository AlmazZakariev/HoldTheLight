using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpStats 
{
    // Класс хранит все переменные, связанные с вертикальным перемещением player
    Rigidbody2D PlayerRb { get; set; }
    // Состояние прыжка на данный момент
    public bool Jumping { get; set; }
    // Ключ активации прыжка
    public bool NeedMakeJump { get; set; }
    // Состояние ускорения вниз на данный момент
    public bool ForcingDown { get; set; } = false;
    // Ключ активации ускорения вниз
    public bool NeedForceDown { get; set; }

    public JumpStats(Rigidbody2D playerRb)
    {
        PlayerRb = playerRb;
        Jumping = false;
        NeedMakeJump = false;
    }
  
    // Вызываем при начале прыжка.
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
