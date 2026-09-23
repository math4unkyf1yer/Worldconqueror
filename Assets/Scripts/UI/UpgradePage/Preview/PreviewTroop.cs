using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PreviewTroop : MonoBehaviour
{
    [SerializeField] Sprite[] troopSprite;

    public float speed;
    public float stopDistance = 0.1f;
    public float waitTime = 0.5f;
    private Image troopLook;
    private PreviewManager previewManager;


    [Header("Vigor")]
    private GameObject opponent;
    private float myVigor;
    private float opponentVigor;
    PreviewTroop EnemyTroop;
    private bool isActive = true;
    Transform vigorStart;

    [Header("Strenght")]
    PreviewTerritory EnemyTerritory;
    private float strenght;

    [Header("VFX")]
    public ParticleSystem particle;

    public void ImageController(bool look)
    {
        if(troopLook == null) { troopLook = GetComponent<Image>(); }
        if (troopLook != null)
        {
            troopLook.enabled = look;
        }
    }

    public void SpriteLook(int newSpriteIndex, Color colorImage)
    {
        if (!troopLook)
        {
            troopLook = GetComponent<Image>();
        }

        troopLook.sprite = troopSprite[newSpriteIndex];

        Color c = colorImage;
        c.a = 1f;

        troopLook.color = c;
    }
    public void SpeedPreview(Transform target, Transform start, float speedT)
    {
        StopAllCoroutines();
        speed = speedT;
        Debug.Log("Troop is ready to move");
        //move to target 
        StartCoroutine(SpeedRoutine(target,start));
    }

    private IEnumerator SpeedRoutine(Transform target, Transform start)
    {
        while (Vector3.Distance(transform.position, target.position) > stopDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position,target.position,speed * Time.deltaTime);

            yield return null;
        }
        ImageController(false);
        transform.position = start.position;
        yield return new WaitForSeconds(waitTime);
        ImageController(true);
        StartCoroutine(SpeedRoutine(target,start));
    }    

    public void VigorPreview(Transform target, Transform start,float speed ,float vigorValue, float enemyVigor, GameObject enemyTroop)
    {
        StopAllCoroutines();
        this.myVigor = vigorValue;
        this.opponentVigor = enemyVigor;
        this.speed = speed;

        vigorStart = start;

        if (enemyTroop != null) { EnemyTroop = enemyTroop.GetComponent<PreviewTroop>(); }

        StartCoroutine(VigorRoutine(target, start));
    }
    public void Lose()
    {
        isActive = false;
        ImageController(false); // hide weaker troop
        transform.position = vigorStart.position;
    }

    private IEnumerator VigorRoutine(Transform target, Transform start)
    {
        while (true)
        {

            if (Vector3.Distance(transform.position, target.position) <= stopDistance)
            {

                //  ImageController(false);
                transform.position = start.position;
                if (particle)
                {
                    particle.Stop();
                }

                yield return new WaitForSeconds(waitTime);

                EnemyTroop.ImageController(true);
                ImageController(true);
                isActive = true;
                EnemyTroop.isActive = true;
                continue;    // stop coroutine
            }

            if (isActive)
            {
                // Move toward target
                transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            }

            // Check distance to opponent troop
            if (EnemyTroop != null)
            {
                float dist = Vector3.Distance(transform.position, EnemyTroop.transform.position);

                if (dist <= stopDistance && isActive)
                {
                    if (particle)
                    {
                       particle.Play();
                       Debug.Log("Play");
                        
                    }
                    EnemyTroop.Lose();
                }
            }

            yield return null;
        }
    }

    public void StrenghtPreview(Transform target, Transform start,float speed, float strenght, PreviewTerritory enemyTerritory)
    {
        StopAllCoroutines();
        this.speed = speed;
        this.strenght = strenght;
        EnemyTerritory = enemyTerritory;

        StartCoroutine(StrenghtRoutine(target, start));
    }

    IEnumerator StrenghtRoutine(Transform target, Transform start)
    {
        while (true)
        {
            if (Vector3.Distance(transform.position, target.position) <= stopDistance)
            {
                EnemyTerritory.ChangeUnitNumber(strenght);
                ImageController(false);
                transform.position = start.position;

                yield return new WaitForSeconds(waitTime);

                EnemyTerritory.ChangeUnitNumber(0);
                ImageController(true);
                continue;    // stop coroutine
            }

            // Move toward target
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            yield return null;
        }
    }

}

