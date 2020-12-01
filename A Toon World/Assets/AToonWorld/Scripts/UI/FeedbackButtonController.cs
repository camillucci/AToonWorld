using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackButtonController : MonoBehaviour
{
    public enum Feedback { VeryBad = 0, Bad = 1, Medium = 2, Happy = 3, VeryHappy = 4 }

    [SerializeField] private GameObject _veryBad;
    [SerializeField] private GameObject _bad;
    [SerializeField] private GameObject _medium;
    [SerializeField] private GameObject _happy;
    [SerializeField] private GameObject _veryHappy;

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
}