using System;
using UnityEngine;

public class Enemy_02_Shoot : MonoBehaviour
{
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private GameObject damageOrb;
    private Character _cc;

    private void Awake()
    {
        _cc = GetComponent<Character>();
    }

    private void Update()
    {
        _cc.RotateToTarget();
    }

    public void ShootTheDamageOrb()
    {
        Instantiate(damageOrb, shootingPoint.position, Quaternion.LookRotation(shootingPoint.forward));
    }
}
