using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;
using System.Globalization;

public class AnaliticsParser : EditorWindow {
    [SerializeField] private GameObject _editorDeathEventPrefab = null;
    private static AnaliticsParser _instance = null;

    private string _analiticsFilePath = "";
    private bool _analiticsParsed = false;

    #region Analitics Collections
    private List<Analitic> _analiticsCollection;

    #endregion
    private float _heatmapRadius = 0.2f;
    private bool _isHeatmap = false;


    #region Gizmos
    private Action _currentGizmosDrawer = null;
    #endregion

    #region SceneComponents
    private GameObject _analiticsObject;
    #endregion

    private double? _meanLevelTime = null;


    [MenuItem ("Inkverse/Analitics")]
    public static void  ShowWindow () {
        _instance = EditorWindow.GetWindow<AnaliticsParser>();
    }
    
    void OnGUI () {
        GUILayout.Label ("Base Settings", EditorStyles.boldLabel);

        if(GUILayout.Button("Open Analitics File (csv)"))
        {
            _analiticsFilePath = EditorUtility.OpenFilePanel("Analitics file path", "", "csv");
            if (_analiticsFilePath.Length != 0)
                ParseAnalitics();
        }
        
        if(_analiticsParsed)
        {
            //_isHeatmap = EditorGUILayout.Toggle("Is Heatmap", _isHeatmap);

            //Heatmap Radius
            if(_isHeatmap)
            {
                _heatmapRadius = EditorGUILayout.Slider("Heatmap Node Radius", _heatmapRadius, 0.001f, 1);
            }

            if(_meanLevelTime.HasValue)
            {
                EditorGUILayout.LabelField($"Mean Level Time: {_meanLevelTime.Value} seconds");
            }

            if(GUILayout.Button("Plot Data"))
            {
                if(_analiticsObject != null)
                    GameObject.Destroy(_analiticsObject);

                _analiticsObject = new GameObject("AnaliticsComponents");
                _analiticsObject.tag = "EditorOnly";

                PlotData();
            }
        }
    }

    private void OnDestroy() 
    {
        if(_analiticsObject != null)
            GameObject.DestroyImmediate(_analiticsObject, false);
    }

    // Custom Gizmos, Create as many as you'd like
    [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Selected)]
    //TODO: Replace first argument with the type you are editing
    private static void GizmoTest(Transform aTarget, GizmoType aGizmoType)
    {
        if (_instance == null)
        {
            return;
        }

        _instance._currentGizmosDrawer?.Invoke();
 
        // TODO: Perform gizmo drawing here
 
        #if UNITY_EDITOR
                // TODO: Place calls to handle drawing functions in here, otherwise build errors will result
        #endif
    }

    private void PlotData()
    {
        _meanLevelTime = _analiticsCollection.Where(a => a.eventName == EventName.LevelTime).Average(e => int.Parse(e.value[0]));
        IEnumerable<Analitic> deathEvents = _analiticsCollection.Where(a => a.eventName == EventName.PlayerDeath);
        
        List<Vector3> deathLocations = new List<Vector3>();
        CultureInfo italian = CultureInfo.GetCultureInfo("it-IT");
        foreach(Analitic deathAnalitic in deathEvents)
        {
            try
            {
                deathLocations.Add(new Vector3(
                    float.Parse(deathAnalitic.value[0], italian),
                    float.Parse(deathAnalitic.value[1], italian)
                ));
            }
            catch
            {
                deathLocations.Add(new Vector3(
                    float.Parse(deathAnalitic.value[0], CultureInfo.InvariantCulture),
                    float.Parse(deathAnalitic.value[1], CultureInfo.InvariantCulture)
                ));
            }
        }
        
        if(_isHeatmap)
        {
            //plot heatmap
        }
        else
        {
            deathLocations.ForEach(location => GameObject.Instantiate(_editorDeathEventPrefab, location, Quaternion.identity, _analiticsObject.transform));
            //_currentGizmosDrawer = () => {
            //    deathLocations.ForEach(location => Gizmos.DrawSphere(location, _heatmapRadius));
            //    //_currentGizmosDrawer = null;
            //};
        }
    }

    private void ParseAnalitics()
    {
        Debug.Log($"Opening {_analiticsFilePath}");
        _analiticsCollection = new List<Analitic>();

        using (FileStream fileStream = new FileStream(_analiticsFilePath, FileMode.Open))
        using(StreamReader streamReader = new StreamReader(fileStream))
        {
            streamReader.ReadLine(); //Headers
            while(streamReader.Peek() >= 0)
            {
                var analitic = new Analitic(streamReader.ReadLine());

                if(analitic.level == SceneManager.GetActiveScene().name)
                    _analiticsCollection.Add(analitic);
            }
        }

        _analiticsParsed = true;
    }
}
