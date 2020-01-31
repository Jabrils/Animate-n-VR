using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionObject
{
    public GameObject self => _self;
    public GameObject[] oSkinP => _oSkinP;
    public GameObject[] oSkinN => _oSkinN;
    public bool hide => _hide;
    bool _hide;
    GameObject _self;
    GameObject[] _oSkinP, _oSkinN;
    DataObject[] _data;

    public MotionObject(GameObject obj, int frames)
    {
        _self = obj;

        _data = new DataObject[frames];

        _oSkinP = new GameObject[ctrl.oSkinCount];
        _oSkinN = new GameObject[ctrl.oSkinCount];

        for (int i = 0; i < ctrl.oSkinCount; i++)
        {
            CreateOnion(new Color(1, 0, 0, .25f / (i + 1)), ref _oSkinP[i]);
            CreateOnion(new Color(0, 1, 0, .25f / (i + 1)), ref _oSkinN[i]);
        }
    }

    public void ToggleHide(bool w)
    {
        _hide = w;
    }

    void CreateOnion(Color mainCol, ref GameObject gO)
    {
        gO = GameObject.Instantiate(_self);
        gO.GetComponent<Renderer>().material = Resources.Load<Material>("red");
        gO.GetComponent<Renderer>().material.color = mainCol;

        gO.transform.localScale = Vector3.zero;

        GameObject.Destroy(gO.GetComponent<Collider>());
        GameObject.Destroy(gO.GetComponent<OVRGrabbable>());
        GameObject.Destroy(gO.GetComponent<Rigidbody>());
    }

    /// <summary>
    /// Update the object data based on the current frame
    /// </summary>
    /// <param name="frame"></param>
    public void UpdateObjData(int frame)
    {
        _data[frame].Set(_self.transform);
    }

    /// <summary>
    /// Move the object based on the current frame
    /// </summary>
    /// <param name="frame"></param>
    public void MoveObj(int frame, bool updateOS = false, bool noOnion = false)
    {
        // if we are hiding then we will shirk & return
        if (hide && ctrl.mode == ctrl.Mode.Play)
        {
            self.transform.localScale = Vector3.zero;
            HideOnions();
            return;
        }

        // else well take from whatever frame
        Take(ref _self, _data[frame]);

        // then check if we have no onions
        if (noOnion)
        {
            HideOnions();
            return;
        }

        // 
        if (updateOS)
        {
            // 
            for (int i = 0; i < ctrl.oSkinCount; i++)
            {

                // 
                if (frame - (i + 1) >= 0)
                {
                    Take(ref _oSkinP[i], _data[frame - (i + 1)]);
                }
                else
                {
                    _oSkinP[i].transform.localScale = Vector3.zero;
                }

                // 
                if (frame + (i + 1) <= ctrl.lastFrame)
                {
                    Take(ref _oSkinN[i], _data[frame + (i + 1)]);
                }
                else
                {
                    _oSkinN[i].transform.localScale = Vector3.zero;
                }
            }
        }
    }

    private void HideOnions()
    {
        for (int i = 0; i < ctrl.oSkinCount; i++)
        {
            _oSkinP[i].transform.localScale = Vector3.zero;
            _oSkinN[i].transform.localScale = Vector3.zero;
        }
    }

    void Take(ref GameObject t, DataObject dO)
    {
        t.transform.localPosition = dO.pos;
        t.transform.localEulerAngles = dO.rot;
        t.transform.localScale = dO.scale;
    }

    public void Delete()
    {
        GameObject.Destroy(_self);

        // 
        for (int i = 0; i < ctrl.oSkinCount; i++)
        {
            GameObject.Destroy(oSkinP[i]);
            GameObject.Destroy(oSkinN[i]);
        }

        MotionScene.motionObj.Remove(this);
    }
}

public struct DataObject
{
    public Vector3 pos;
    public Vector3 rot;
    public Vector3 scale;

    public void Set(Transform m)
    {
        pos = m.localPosition;
        rot = m.localEulerAngles;
        scale = m.localScale;
    }
}

public static class MotionScene
{
    // JSON
    public static List<MotionObject> motionObj = new List<MotionObject>();

    public static void Save()
    {
        for (int i = 0; i < motionObj.Count; i++)
        {
        }
    }
}