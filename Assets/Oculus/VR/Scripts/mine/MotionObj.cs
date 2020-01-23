using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionObj
{
    public GameObject obj, parent;
    public GameObject[] onionPrev = new GameObject[2], onionNext = new GameObject[2];
    public Vector3 pos;
    public Vector3 rot;
    public Vector3 scale;
    public Color col;

}

public static class MotionScene
{
    // obj[obj #][frame] I THINK
    public static List<MotionObj[]> objs = new List<MotionObj[]>();
}
