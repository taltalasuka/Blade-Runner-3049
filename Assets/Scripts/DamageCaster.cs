using System.Collections.Generic;
using UnityEngine;

public class DamageCaster : MonoBehaviour
{
    private Collider _damageCasterCollider;
    [SerializeField] private int damage = 30;
    [SerializeField] private string targetTag;
    private List<Collider> _damageTargetList;

    private void Awake()
    {
        _damageCasterCollider = GetComponent<Collider>();
        _damageCasterCollider.enabled = false;
        _damageTargetList = new List<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag) && !_damageTargetList.Contains(other))
        {
            Character targetCc = other.GetComponent<Character>();
            if (targetCc != null)
            {
                targetCc.ApplyDamage(damage, transform.parent.position);
                PlayerVFXManager playerVFXManager = transform.parent.GetComponent<PlayerVFXManager>();
                if (playerVFXManager != null)
                {
                    RaycastHit hit;
                    Vector3 oriPos = transform.position + (-_damageCasterCollider.bounds.extents.z) * transform.forward;
                    bool isHit = Physics.BoxCast(oriPos, _damageCasterCollider.bounds.extents / 2, transform.forward,
                        out hit, transform.rotation, _damageCasterCollider.bounds.extents.z, 1 << 6);
                    if (isHit)
                    {
                        playerVFXManager.PlaySlash(hit.point + new Vector3(0, 0.5f, 0));
                    }
                }
            }
            _damageTargetList.Add(other);
        }
    }

    public void EnableDamageCaster()
    {
        _damageTargetList.Clear();
        _damageCasterCollider.enabled = true;
    }

    public void DisableDamageCaster()
    {
        _damageTargetList.Clear();
        _damageCasterCollider.enabled = false;
    }
}
