
using UnityEngine;
using UnityEngine.VFX;

public class EnemyVFXManager : MonoBehaviour
{
    [SerializeField] private VisualEffect footstep;
    [SerializeField] private VisualEffect attackVFX;
    [SerializeField] private ParticleSystem beingHitVFX;
    [SerializeField] private VisualEffect beingHitSplashVFX;
    public void PlayAttackVFX()
    {
        attackVFX.SendEvent("OnPlay");
    }

    public void BurstFootStep()
    {
        footstep.SendEvent("OnPlay");
    }

    public void PlayBeingHitVFX(Vector3 attackerPos)
    {
        Vector3 forceForward = transform.position - attackerPos;
        forceForward.Normalize();
        forceForward.y = 0;
        beingHitVFX.transform.rotation = Quaternion.LookRotation(forceForward);
        beingHitVFX.Play();
        Vector3 splashPos = transform.position;
        splashPos.y += 2f;
        VisualEffect newSplashVFX = Instantiate(beingHitSplashVFX, splashPos, Quaternion.identity);
        newSplashVFX.SendEvent("OnPlay");
        Destroy(newSplashVFX.gameObject, 10f);
    }
}
