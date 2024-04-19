using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


// need to attach into light2d
public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private LayerMask enemyLayer;
    [SerializeField]
    private Transform attackPosition;
    [SerializeField]
    private float attackLightTime;
    [SerializeField]
    private float attackRangeX;
    [SerializeField]
    private float attackRangeY;
    [SerializeField]
    private AudioSource batDeadSound;

    private bool attackLightActive;

    // Update is called once per frame
    void Update()
    {
        if (attackLightActive)
        {
            Attack();
        }
    }
    public void StartAttack()
    {
        attackLightActive = true;
        gameObject.SetActive(true);
        Invoke("TurnOffAttackLight", attackLightTime);
    }
    private void TurnOffAttackLight()
    {
        gameObject.SetActive(false);
        attackLightActive = false;
    }
    private void Attack()
    {
        Collider2D[] enemies = Physics2D.OverlapCapsuleAll(attackPosition.position, new Vector2(attackRangeX, attackRangeY), new CapsuleDirection2D(),0, enemyLayer);
        for (var i=0; i<enemies.Length;i++) 
        {
            var target = enemies[i].GameObject();
            if (target.CompareTag("Enemy"))
            {
                batDeadSound.Play();
                Destroy(target);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPosition.position, new Vector3(attackRangeX, attackRangeY, 0));
    }
}
