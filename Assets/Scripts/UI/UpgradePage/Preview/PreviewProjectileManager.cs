using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewProjectileManager : MonoBehaviour
{
    public static PreviewProjectileManager Instance;

    private bool previewActive = false;

    private void Awake()
    {
        Instance = this;
    }

    public void StartAttackRangePreview(PreviewProjectile projectile, Transform start, Transform target)
    {
        projectile.StopAllCoroutines();
        projectile.StartCoroutine(projectile.AttackRangeLoop(start, target));
    }

    public void StartFireRatePreview(PreviewProjectile[] projectiles,Transform shooter,Transform[] targets,float fireRate)
    {
        previewActive = true;
        foreach (PreviewProjectile p in projectiles)
            p.ImageController(false);

        StartCoroutine(FireRateRoutine(projectiles, shooter, targets, fireRate));
    }

    private IEnumerator FireRateRoutine(PreviewProjectile[] projectiles,Transform shooter,Transform[] targets,float fireRate)
    {
        while (previewActive)
        {
            for (int i = 0; i < projectiles.Length; i++)
            {
                if (!previewActive)
                    yield break;

                PreviewProjectile proj = projectiles[i];
                Transform target = targets[i];

                proj.SetUpFireRateProjectile(shooter, target);

                yield return new WaitForSeconds(fireRate);
            }
        }
    }
    public void StopPreview()
    {
        previewActive = false;
        StopAllCoroutines();
    }
}
