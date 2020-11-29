using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

#if AnaliticsEnabled

public class RemoteSheetUploader : MonoBehaviour
{
    private const string _developmentSheet = "19w9AmPQU_z7VHOo3uQ7e2JX41fxOvHAS3s7BnDBKff4";
    private const string _productionSheet = "1rdUast21nZktUL0IMcyeQtqaQA8Mwt7xCTNQNCqsjcM";

    private string _spreadsheetId;
    private Analitic _analitic;

    private RemoteSheetUploader(Analitic analitic)
    {
        _analitic = analitic;
        
        #if Production
        _spreadsheetId = _productionSheet;
        #else
        _spreadsheetId = _developmentSheet;
        #endif
    }

    public static RemoteSheetUploader Create(Analitic analitic) => new RemoteSheetUploader(analitic);

    public async UniTaskVoid Upload()
    {
        string uri = $"https://sheets.googleapis.com/v4/spreadsheets/{_spreadsheetId}/values/{getAnaliticSheetName()}!A1:append";

        using (UnityWebRequest request = UnityWebRequest.Post(uri, createJson()))
        {
            var response = await request.SendWebRequest();
            Debug.Log(response.responseCode);
        }
    }

    private string createJson() => JsonUtility.ToJson(_analitic);

    private string getAnaliticSheetName() => _analitic.eventName.ToString();
}

#endif