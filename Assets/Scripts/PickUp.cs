using UnityEngine;

public class PickUp : MonoBehaviour
{
    public enum PickUpType
    {
        Coin, Heal
    }

    public PickUpType type;
    public int value = 20;
    [SerializeField] private ParticleSystem collectedVFX;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Character>().PickUpItem(this);
            if (collectedVFX != null)
            {
                Instantiate(collectedVFX, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
