using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionObject
{
    GameObject _self;
    public GameObject self => _self;
    Vector3[] _pos;
    Vector3[] _rot;
    Vector3[] _scale;

    public MotionObject(GameObject obj, int frames)
    {
        _self = obj;

        _pos = new Vector3[frames];
        _rot = new Vector3[frames];
        _scale = new Vector3[frames];
    }

    /// <summary>
    /// Update the object based on the current frame
    /// </summary>
    /// <param name="frame"></param>
    public void UpdateObj(int frame)
    {
        _pos[frame] = self.transform.localPosition;
        _rot[frame] = self.transform.localEulerAngles;
        _scale[frame] = self.transform.localScale;
    }

    /// <summary>
    /// Set the object based on the current frame
    /// </summary>
    /// <param name="frame"></param>
    public void SetObj(int frame)
    {
        self.transform.localPosition = _pos[frame];
        self.transform.localEulerAngles = _rot[frame];
        self.transform.localScale = _scale[frame];
    }
}

public static class MotionScene
{
    // 
    public static List<MotionObject> motionObj = new List<MotionObject>();
}
