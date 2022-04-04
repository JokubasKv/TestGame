using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_CustomBullet : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask whatIsEnemies;

    [Header("Bullet Settings")]
    [Range(0f,1f)]
    public float bouncinnes;
    public bool useGravity;

    public int explosionDamage;
    public float explosionRange;

    public float maxCollisions;
    public float maxLifetime;
    public bool explodeOnTouch = true;

    private int collisions;
    PhysicMaterial physics_mat;

    [Header("Audio")]
    public AudioSource src;
    public AudioClip explosionEffect;


    private void Start()
    {
        Setup();
    }
    private void Update()
    {
        if (collisions > maxCollisions) Explode();


        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Explode();
    }
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.collider.CompareTag("Bullet")) return;

        collisions++;
        if ((collision.collider.CompareTag("Enemy") || collision.collider.CompareTag("Player")) && explodeOnTouch) 
        { 
            Explode(); 
        } 
    }
    
    private void Explode()
    {
        if (explosion != null) 
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }
        if (explosionEffect != null)
        {
            src.PlayOneShot(explosionEffect);
        }
        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].CompareTag("Enemy")){
                enemies[i].GetComponent<scr_EnemyAi>().TakeDamage(explosionDamage);
            }
            if (enemies[i].CompareTag("Player")){
                if (enemies[i].enabled == true)
                {
                    enemies[i].GetComponent<scr_CharacterController>().TakeDamage(explosionDamage, scr_Models.DamageType.Electric);
                    Debug.Log(enemies[i]);
                }
            }
        }

        Invoke("Delay", 0.00f);
    }
    private void Delay()
    {
        Destroy(gameObject);
    }
    private void Setup()
    {
        physics_mat = new PhysicMaterial();
        physics_mat.bounciness = bouncinnes;
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;

        GetComponent<SphereCollider>().material = physics_mat;

        rb.useGravity = useGravity;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
