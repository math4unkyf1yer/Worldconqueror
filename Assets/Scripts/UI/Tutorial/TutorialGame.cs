using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
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

    //preview tutorial 
    public GameObject closeTutorial;
    public GameObject[] openTutorial;
    public Transform[] positions;
    public PreviewTroop troopPreview;
    public PreviewHazard hazardPreview;

    [SerializeField] TextMeshProUGUI tutorialGameText;
    private string tutorialTitle;

    int index = 0;
    public void SetTutorialType(TutorialType typeCl, List<TerretoryController> game)
    {
        type = typeCl;
        territories = game;
        tutorialRef = AssignLevel.Instance.tutorialMenu;
        StartTutorial();    
    }

    public void StartTutorial()
    {
        if (type == TutorialType.handDrag)
        {
            // Get the sequence
            var seq = tutorialRef.tutorials["DragTroop"];

            // If completed, stop
            if (seq.completed)
                return;

            // If we are not on the correct step, stop
            if (seq.currentIndex != 0)
                return;

            HandTutorial();
        }
        else if (type != TutorialType.none)
        {
            GameState.TutorialActive = true;
            UiPreviewTutorial();
        }
    }

    //slow preview --- troop preview --- territory preview 
    void UiPreviewTutorial()
    {
        if (closeTutorial)
        {
            closeTutorial.SetActive(true);
        }
        HazardType typeHazard = HazardType.Slow;
        switch (type)
        {
            //open the close 
            case TutorialType.slow:
                typeHazard = HazardType.Slow;
                tutorialTitle = "Slow Hazard";
                break;
            case TutorialType.speed:
                typeHazard = HazardType.Speed;
                tutorialTitle = "Speed Hazard";
                break;
            case TutorialType.damage:
                typeHazard = HazardType.Damage;
                tutorialTitle = "Damage Hazard";
                break;
        }
        openTutorial[0].SetActive(true);
        //move troop A-B  (need to slow down and vfx)
        tutorialGameText.text = tutorialTitle;
        hazardPreview.SetHazard(troopPreview, typeHazard);
        troopPreview.SpeedPreview(positions[1], positions[0], 85);
    }

    public void ClosePreviewTutorial()
    {
        if (closeTutorial)
        {
            closeTutorial.SetActive(false);
            openTutorial[index].SetActive(false);
            //keep track if tutorial open or not 
            GameState.TutorialActive = false;
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
        /* Mark step complete
        var seq = tutorialRef.tutorials["DragTroop"];
        seq.currentIndex++;

        // If no more steps, mark completed
        if (seq.currentIndex >= seq.pages.Count)
        {
            seq.completed = true;
        }*/
    }

}

public static class GameState
{
    public static bool TutorialActive;
}
