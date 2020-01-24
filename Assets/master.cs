using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class master : MonoBehaviour
{
    GameObject UI, Info;
    TextMeshPro txt_Frame, txt_Rate, txt_Last, txt_Geo, txt_Color, txt_Debug;
    TextMeshPro txt_Scale;
    public static string save;
    string[] cols = new string[] { "Black", "White", "Red", "Blue", "Green", "Gray", "Cyan", "Magenta" };
    string[] scal = new string[] { "All", "X", "Y", "Z" };

    // Start is called before the first frame update
    void Start()
    {
        UI = GameObject.Find("UI");
        Info = GameObject.Find("Info");

        txt_Frame = GameObject.Find("txt_Frame").GetComponent<TextMeshPro>();
        txt_Rate = GameObject.Find("txt_Rate").GetComponent<TextMeshPro>();
        txt_Last = GameObject.Find("txt_Last").GetComponent<TextMeshPro>();
        txt_Geo = GameObject.Find("txt_Geo").GetComponent<TextMeshPro>();
        txt_Color = GameObject.Find("txt_Color").GetComponent<TextMeshPro>();

        txt_Scale = GameObject.Find("txt_Scale").GetComponent<TextMeshPro>();
        txt_Debug = GameObject.Find("txt_Debug").GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        UI.gameObject.SetActive(ctrl.mode != ctrl.Mode.Play);
        Info.gameObject.SetActive(ctrl.mode != ctrl.Mode.Play);

        // 
        if (ctrl.mode == ctrl.Mode.Play)
        {
            return;
        }

        txt_Frame.text = $"Frame: {ctrl.frame + 1}";
        txt_Rate.text = $"FPS: {ctrl.fps}";
        txt_Last.text = $"Last Frame: {ctrl.lastFrame}";
        txt_Geo.text = $"Geometry: {ctrl.pTypes[ctrl.pType]}";
        txt_Color.text = $"Color: {cols[ctrl.col]}";

        txt_Scale.text = $"Scaling: {scal[ctrl.scaleAxis]}";
        txt_Debug.text = $"Debug: {ctrl.debutInfo}";

        txt_Frame.color = ctrl.uiLoc == 0 ? Color.yellow : Color.white;
        txt_Rate.color = ctrl.uiLoc == 1 ? Color.yellow : Color.white;
        txt_Last.color = ctrl.uiLoc == 2 ? Color.yellow : Color.white;
        txt_Geo.color = ctrl.uiLoc == 3 ? Color.yellow : Color.white;
        txt_Color.color = ctrl.uiLoc == 4 ? ctrl.cols[ctrl.col] : Color.white;
    }

    public static void Exp(string e)
    {
        save = e;
    }
}
