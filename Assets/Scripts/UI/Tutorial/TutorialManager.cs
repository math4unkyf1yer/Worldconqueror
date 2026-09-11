using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] Transform hand;
    GameObject currentTutorialPage;

    Transform handPosition;

    public Dictionary<string, bool> tutorialCompleted;

    private void Start()
    {
        tutorialCompleted = new Dictionary<string, bool>();

        // Add your tutorial IDs here
        tutorialCompleted["DragTroop"] = false;
        tutorialCompleted["UpgradeTroop"] = false;
        tutorialCompleted["UpgradeTerritory"] = false;
        tutorialCompleted["Shop"] = false;
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

            //hand position
            foreach (Transform child in currentTutorialPage.transform)
            {
                if (child.CompareTag("HandSpot"))
                {
                    handPosition = child;
                    break;
                }
            }
            if( handPosition != null )
            {
                hand.SetParent(handPosition);
                hand.transform.position = handPosition.position;
            }
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
