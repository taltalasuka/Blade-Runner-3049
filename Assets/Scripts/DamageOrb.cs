using UnityEngine;

public class DamageOrb : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private int damage = 10;
    [SerializeField] private ParticleSystem hitVFX;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(transform.position + speed * Time.deltaTime * transform.forward);
    }

    private void OnTriggerEnter(Collider other)
    {
        Character cc = other.gameObject.GetComponent<Character>();
        if (cc != null && cc.isPlayer)
        {
            cc.ApplyDamage(damage, transform.position);
        }
        Instantiate(hitVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
