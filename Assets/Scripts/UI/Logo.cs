using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Logo : MonoBehaviour
{
    private static bool hasPlayed = false;

    private Image currentImage;
    public Image childImage;
    public GameObject populatedItems;

    float FadeDuration = 1.5f;

    private void Start()
    {
        if (hasPlayed)
        {
            // Skip fade entirely
            currentImage = GetComponent<Image>();
            currentImage.gameObject.SetActive(false);
            return;
        }

        hasPlayed = true;
        currentImage = GetComponent<Image>();

        //need to make sure only called at the start of the game
        StartCoroutine(FadeInOut());
    }

    IEnumerator FadeInOut()
    {
        yield return StartCoroutine(Fade(0f, 1f, FadeDuration,childImage));

        yield return StartCoroutine(Fade(1f, 0f, FadeDuration, childImage));

        yield return StartCoroutine(Fade(1f, 0f, FadeDuration, currentImage));

        //turn on gpu images
        currentImage.gameObject.SetActive(false);

    }

    IEnumerator Fade(float start, float end, float fadeDuration, Image img)
    {
        float t = 0f;
        Color c = img.color;
        
        while(t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(start, end, t/fadeDuration);

            c.a = alpha;
            img.color = c;
            yield return null;
        }
    }
}
