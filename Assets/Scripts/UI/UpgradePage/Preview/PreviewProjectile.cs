using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewProjectile : MonoBehaviour
{
    public float speed;
    public float stopDistance;
    public float waitTime;

    private Image look;

    PreviewTroop targetTroop;

    public void ImageController(bool activate)
    {
        if (look == null) { look = GetComponent<Image>(); }
        look.enabled = activate;
    }

    public IEnumerator AttackRangeLoop(Transform start, Transform target)
    {
        targetTroop = target.GetComponent<PreviewTroop>();

        while (true)
        {
            // reset projectile
            transform.position = start.position;
            ImageController(true);

            // move toward target
            while (Vector3.Distance(transform.position, target.position) > stopDistance)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target.position,
                    speed * Time.deltaTime
                );

                yield return null;
            }

            // hit effect
            ImageController(false);
            targetTroop.ImageController(false);

            yield return new WaitForSeconds(waitTime);

            targetTroop.ImageController(true);
        }
    }

    public void SetUpFireRateProjectile(Transform shooter, Transform target)
    {
        StopAllCoroutines();
        StartCoroutine(FireRateProjectileRoutine(shooter, target));
    }

    private IEnumerator FireRateProjectileRoutine(Transform shooter, Transform target)
    {
        PreviewTroop targetTroop = target.GetComponent<PreviewTroop>();

        transform.position = shooter.position;
        ImageController(true);

        while (Vector3.Distance(transform.position, target.position) > stopDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );

            yield return null;
        }

        // hit
        ImageController(false);
        transform.position = shooter.position;
        targetTroop.ImageController(false);

        yield return new WaitForSeconds(waitTime); 
        targetTroop.ImageController(true);
    }

}
