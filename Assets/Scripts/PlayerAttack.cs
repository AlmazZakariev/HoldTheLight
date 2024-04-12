using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Transform attackPosition;
    public LayerMask enemy;
    public float attackRange;

    public float attackRangeX;
    public float attackRangeY;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Attack()
    {
        
        
        
        //Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, enemy);
        Collider2D[] enemies = Physics2D.OverlapCapsuleAll(attackPosition.position, new Vector2(attackRangeX, attackRangeY), new CapsuleDirection2D(), enemy);
        for (var i=0; i<enemies.Length;i++) 
        {
            var target = enemies[i].GameObject();
            if (target.CompareTag("Enemy"))
            {
                Destroy(target);
            }
            ;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition.position, attackRange);
        Gizmos.DrawWireCube(attackPosition.position, new Vector3(attackRangeX, attackRangeY, 0));
    }
}
