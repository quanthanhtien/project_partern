using UnityEngine;

public class SpiderSilkProjectile : MonoBehaviour
{
    public float speed = 25f;
    public float lifetime = 5f;
    public float damage = 10f;
    public GameObject hitEffect; 
    
    private Vector3 direction;
    private bool initialized = false;
    
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        initialized = true;
    }
    
    void Update()
    {
        if (initialized)
        {
            transform.position += direction * speed * Time.deltaTime;
            transform.forward = direction;
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.gameObject);
    }
    
    void OnTriggerEnter(Collider other)
    {
        HandleHit(other.gameObject);
    }
    
    private void HandleHit(GameObject hitObject)
    {
        
    }
}