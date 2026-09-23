using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class TutorialSequence
{
    public List<GameObject> pages = new List<GameObject>();
    public int currentIndex = 0;
    public bool completed = false;
}

public class TutorialManager : MonoBehaviour
{
    [SerializeField] Transform hand;
    GameObject currentTutorialPage;

    Transform handPosition;

    public Dictionary<string, TutorialSequence> tutorials;

    private void Start()
    {
        tutorials = new Dictionary<string, TutorialSequence>();

        // Add your tutorial IDs here
        tutorials["DragTroop"] = new TutorialSequence();
        tutorials["UpgradeTroop"] = new TutorialSequence();
        tutorials["UpgradeTerritory"] = new TutorialSequence();
        tutorials["Shop"] = new TutorialSequence();
    }

    public void RegisterPage(string tutoID,GameObject page)
    {
        if (tutorials.ContainsKey(tutoID))
        {
            tutorials[tutoID].pages.Add(page);
        }
    }
    public void OpenTutorial(string tutoID)
    {
        if(tutorials != null)
        {
            if (!tutorials.ContainsKey(tutoID))
                return;

            var seq = tutorials[tutoID];

            if (seq.completed)
                return;

            if (seq.currentIndex >= seq.pages.Count)
                return;

            currentTutorialPage = seq.pages[seq.currentIndex];
            currentTutorialPage.SetActive(true);

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
                hand.gameObject.SetActive(true);
                hand.SetParent(handPosition);
                hand.transform.position = handPosition.position;
            }
        }
    }
    public void CloseTutorial(string tutoID)
    {

        if (!tutorials.ContainsKey(tutoID))
                return;
        var seq = tutorials[tutoID];

        if(currentTutorialPage != null)
        {
            currentTutorialPage.SetActive(false);
        }

        hand.gameObject.SetActive(false);

        seq.currentIndex++;

        if (seq.currentIndex >= seq.pages.Count)
        {
            seq.completed = true;
        }
        else
        {
            // Open next page automatically
            OpenTutorial(tutoID);
        }

    }
}
