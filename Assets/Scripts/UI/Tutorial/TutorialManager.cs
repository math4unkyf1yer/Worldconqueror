using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] Transform hand;
    GameObject currentTutorialPage;

    public Dictionary<string, bool> tutorialCompleted;

    private void Start()
    {
        tutorialCompleted = new Dictionary<string, bool>();

        // Add your tutorial IDs here
        tutorialCompleted["DragTroop"] = false;
        tutorialCompleted["UpgradeTroop"] = false;
        tutorialCompleted["UpgradeTerritory"] = false;
    }
    public void OpenTutorial(GameObject tutorialPage,string tutoID)
    {
        if(tutorialCompleted != null)
        {
            if (tutorialCompleted.ContainsKey(tutoID) && tutorialCompleted[tutoID])
            {
                return;
            }

            currentTutorialPage = tutorialPage;
            currentTutorialPage.SetActive(true);
            hand.gameObject.SetActive(true);
            hand.SetParent(currentTutorialPage.transform);
            hand.transform.position = currentTutorialPage.transform.position;
        }
    }
    public void CloseTutorial(string tutoID)
    {
        if(currentTutorialPage != null)
        {
            if (currentTutorialPage.activeInHierarchy)
            {
                currentTutorialPage.SetActive(false);
                hand.gameObject.SetActive(false);

                if (tutorialCompleted.ContainsKey(tutoID))
                {
                    tutorialCompleted[tutoID] = true;
                }
            }
        }
    }
}
