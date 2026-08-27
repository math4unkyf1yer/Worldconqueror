using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class TutorialGame : MonoBehaviour
{
    public System.Action OnPlayerDrag;
    private TutorialType type;
    [SerializeField] GameObject hand;
    List<TerretoryController> territories;
    private Transform pointA;
    private Transform pointB;
    Coroutine handCoruntine;

    private TutorialManager tutorialRef;
    public void SetTutorialType(TutorialType typeCl, List<TerretoryController> game)
    {
        type = typeCl;
        territories = game;
        tutorialRef = AssignLevel.Instance.tutorialMenu;
        StartTutorial();    
    }

    public void StartTutorial()
    {
        switch(type)
        {
            case TutorialType.handDrag:
                if(tutorialRef.tutorialCompleted.ContainsKey("DragTroop") && tutorialRef.tutorialCompleted["DragTroop"]) { return; }
                HandTutorial();
                tutorialRef.tutorialCompleted["DragTroop"] = true;
                break;
            case TutorialType.newAssassin:
                break;
        }
    }

    void HandTutorial()
    {
        //assigned a delegate to activate once the drag is called 
        foreach (var t in territories)
        {
             if(t.terretoryData.TerretoryID == 0) 
            {
                t.OnDragEvent += StopHand;
                pointA = t.transform; 
            }
             if(t.terretoryData.TerretoryID == 1) { pointB = t.transform; }
        }
        hand.SetActive(true);
        hand.transform.position = pointA.position;

        //move the hand to point A to B
        handCoruntine = StartCoroutine(MoveHandRoutine());
    }

    IEnumerator MoveHandRoutine()
    {
        float duration = 1.5f;

        while (true)   // repeat forever
        {
            // Move A -> B
            float t = 0f;
            Vector3 startPos = pointA.position;
            Vector3 endPos = pointB.position;

            while (t < duration)
            {
                t += Time.deltaTime;
                float normalized = t / duration;

                hand.transform.position = Vector3.Lerp(startPos, endPos, normalized);

                yield return null;
            }

            yield return new WaitForSeconds(0.2f);
            // Snap back instantly to A
            hand.transform.position = pointA.position;

            // Optional small delay before repeating
            yield return new WaitForSeconds(0.2f);
        }
    }

    void StopHand()
    {
        if (handCoruntine != null)
        {
            StopCoroutine(handCoruntine);
            handCoruntine = null;
        }

        hand.SetActive(false);
    }

}
