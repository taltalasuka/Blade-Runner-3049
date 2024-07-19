using System.Collections;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] private GameObject gateVisual;
    private Collider _gateCollider;
    [SerializeField] private float openDuration = 2f;
    [SerializeField] private float openTargetY = -1.5f;

    private void Awake()
    {
        _gateCollider = GetComponent<Collider>();
    }

    IEnumerator OpenGateAnimation()
    {
        float currentOpenDuration = 0;
        Vector3 startPos = gateVisual.transform.position;
        Vector3 targetPos = startPos + Vector3.up * openTargetY;
        while (currentOpenDuration < openDuration)
        {
            currentOpenDuration += Time.deltaTime;
            gateVisual.transform.position = Vector3.Lerp(startPos, targetPos, currentOpenDuration / openDuration);
            yield return null;
        }
        _gateCollider.enabled = false;
    }

    public void Open()
    {
        StartCoroutine(OpenGateAnimation());
    }
}
