using UnityEngine;
using UnityEngine.Networking;

#if AnaliticsEnabled

public class RemoteFormUploader
{
    private GoogleForm _form;
    private Analitic _analitic;

    private RemoteFormUploader(Analitic analitic)
    {
        _analitic = analitic;
        
        #if Production
        _form = GoogleForm.Production;
        #else
        _form = GoogleForm.Development;
        #endif
    }

    public static RemoteFormUploader Create(Analitic analitic) => new RemoteFormUploader(analitic);

    public void Upload()
    {
        UnityWebRequest request = UnityWebRequest.Post(_form.ID, CompileForm());
        request.SendWebRequest();
        
        #if UNITY_EDITOR
        if (request.isNetworkError)
            Debug.Log(request.error);
        #endif
    }

    private WWWForm CompileForm()
    {
        WWWForm form = new WWWForm();
        foreach ((string, string) pair in _form.CreateForm(_analitic))
            form.AddField(pair.Item1, pair.Item2);
        return form;
    }
}

#endif