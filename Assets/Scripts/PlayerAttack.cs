using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Transform attackPosition;
    public LayerMask enemy;
    public float attackRange;

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
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, enemy);
        for (var i=0; i<enemies.Length;i++) 
        {
            var a = enemies[i].GameObject();
            Destroy(a);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition.position, attackRange);
    }
}
