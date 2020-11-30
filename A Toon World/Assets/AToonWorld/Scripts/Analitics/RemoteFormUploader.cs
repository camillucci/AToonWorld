using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

#if AnaliticsEnabled

public class RemoteFormUploader : MonoBehaviour
{
    private const string _developmentForm = "1FAIpQLSfnUUwmk5rI8CJO47DiFcpG0qE6mqTvW6AQQAP8LRCg6iazgA";
    private const string _productionForm = "1TXuOnD_bbq78st2ieqD96lKQdTVh1R_ZpwdwKHjQu-4";

    private string _formId;
    private string _uri;
    private List<Analitic> _analitics;

    private RemoteFormUploader(List<Analitic> analitics)
    {
        _analitics = analitics;
        
        #if Production
        _spreadsheetId = _productionForm;
        #else
        _formId = _developmentForm;
        #endif

        _uri = $"https://docs.google.com/forms/u/0/d/e/{_formId}/formResponse";
    }

    public static RemoteFormUploader Create(List<Analitic> analitics) => new RemoteFormUploader(analitics);

    public void Upload()
    {
        string content = string.Empty;
        foreach (Analitic analitic in _analitics)
            content += analitic.ToString() + Environment.NewLine;

        WWWForm form = new WWWForm();
        form.AddField("entry.1195978271", content);

        UnityWebRequest request = UnityWebRequest.Post(_uri, form);

        request.SendWebRequest();
        
        #if UNITY_EDITOR
        if (request.isNetworkError)
            Debug.Log(request.error);
        else
            Debug.Log("Form upload complete!");
        #endif
    }
}

#endif