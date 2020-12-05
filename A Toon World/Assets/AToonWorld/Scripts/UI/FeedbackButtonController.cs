using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackButtonController : MonoBehaviour
{
    public enum Feedback { VeryBad = 0, Bad = 1, Medium = 2, Happy = 3, VeryHappy = 4 }

    public void SendVeryHappy() => Send(Feedback.VeryHappy);
    public void SendHappy() => Send(Feedback.Happy);
    public void SendMedium() => Send(Feedback.Medium);
    public void SendBad() => Send(Feedback.Bad);
    public void SendVeryBad() => Send(Feedback.VeryBad);

    private void Send(Feedback feedback) 
    {
        #if AnaliticsEnabled
        Events.AnaliticsEvents.FeedbackSurvey.Invoke(new Analitic(feedback));
        #endif
    }

    public void RefreshButtons()
    {
        foreach(Button button in GetComponentsInChildren<Button>(true))
        {
            button.gameObject.SetActive(true);
            button.interactable = true;
        }
    }
}
