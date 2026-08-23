using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialButton : MonoBehaviour
{
    [SerializeField] Transform hand;
    GameObject currentTutorialPage;
    public void OpenTutorial(GameObject tutorialPage)
    {
        currentTutorialPage = tutorialPage;
        currentTutorialPage.SetActive(true);
        hand.gameObject.SetActive(true);
        hand.SetParent(currentTutorialPage.transform);
    }
    public void CloseTutorial()
    {
        if (currentTutorialPage.activeInHierarchy)
        {
            currentTutorialPage.SetActive(false);
            hand.gameObject.SetActive(false);
        }
    }
}
