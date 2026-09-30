using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoReturnFx : MonoBehaviour
{
    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        StartCoroutine(ReturnWhenDone());
    }

    IEnumerator ReturnWhenDone()
    {
        yield return new WaitForSeconds(ps.main.duration);
        ParticlePool.Instance.ReceiveParticle(gameObject);
    }
}
