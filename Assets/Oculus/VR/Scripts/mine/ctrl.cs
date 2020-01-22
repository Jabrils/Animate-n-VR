using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ctrl : MonoBehaviour
{
    public Camera cam;
    public OVRGrabber[] grabber;
    //public static GameObject focal;
    public enum Mode { Animate, Play }
    public Mode mode;
    public static PrimitiveType[] pTypes = new PrimitiveType[] { PrimitiveType.Capsule, PrimitiveType.Cube, PrimitiveType.Cylinder, PrimitiveType.Plane, PrimitiveType.Quad, PrimitiveType.Sphere };
    public static Color[] cols = new Color[] { Color.black, Color.white, Color.red, Color.blue, Color.green, Color.grey, Color.cyan, Color.magenta };

    public static int frame = 0, lastFrame = 10, pType, col;
    public static float fps = 12;
    public static int scaleAxis;
    Vector3[] scaleAxises = new Vector3[] { Vector3.one, Vector3.right, Vector3.up, Vector3.forward };
    float playFrame, lean = .5f;
    int creation = 0;
    bool[] joyAxisReset = new bool[] { true, true };
    bool[] buttBottomReset = new bool[] { true, true };
    bool[] joyInReset = new bool[] { true, true };
    bool[] joyIndexReset = new bool[] { true, true };
    public static bool[] parent = new bool[] { true, true };
    Vector2[] axisHold = new Vector2[2];
    Vector2 scaleMaxClamp = new Vector2(.05f, 25f);
    Color mainCol = new Color(1, 0, 0, 1);
    Vector3 startLoc = Vector3.zero;
    public static int uiLoc;

    void CreateOnionSkins(int id, Mesh m)
    {
        GameObject[] b, a;

        b = new GameObject[2];
        a = new GameObject[2];

        // 
        for (int i = 0; i < 2; i++)
        {
            b[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            a[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);

            Destroy(b[i].GetComponent<BoxCollider>());
            Destroy(a[i].GetComponent<BoxCollider>());

            b[i].GetComponent<MeshFilter>().mesh = m;
            a[i].GetComponent<MeshFilter>().mesh = m;

            b[i].transform.localScale = Vector3.one * .5f;
            a[i].transform.localScale = Vector3.one * .5f;

            b[i].GetComponent<Renderer>().material = Resources.Load<Material>("red");
            a[i].GetComponent<Renderer>().material = Resources.Load<Material>("red");

            mainCol.a = .25f / (i + 1);
            a[i].GetComponent<Renderer>().material.color = mainCol;
            mainCol = new Color(0, 0, 1, .25f / (i + 1));
            b[i].GetComponent<Renderer>().material.color = mainCol;

            b[i].transform.localPosition = Vector3.down * 1000000000;
            a[i].transform.localPosition = Vector3.down * 1000000000;
        }

        MotionScene.objs[id][0].a = a;
        MotionScene.objs[id][0].b = b;
    }

    // Update is called once per frame
    void Update()
    {
        // 
        if (mode == Mode.Animate)
        {
            ModeAnimate();
        }
        else if (mode == Mode.Play)
        {
            ModePlay();
        }
    }

    void ModePlay()
    {
        playFrame += Time.deltaTime * fps;

        frame = Mathf.Clamp(Mathf.RoundToInt(playFrame), 0, lastFrame - 1);

        // 
        if (frame == lastFrame - 1)
        {
            playFrame = 0;
        }

        for (int i = 0; i < 2; i++)
        {
            if (buttBottomReset[i] && TouchControls.Bottom[i])
            {
                mode = Mode.Animate;
                buttBottomReset[i] = false;
            }

            if (!TouchControls.Bottom[i])
            {
                buttBottomReset[i] = true;
            }

            // Remove all onion skins while in play mode
            for (int j = 0; j < MotionScene.objs.Count; j++)
            {
                MotionScene.objs[j][0].b[i].transform.localPosition = Vector3.down * 1000000000;
                MotionScene.objs[j][0].a[i].transform.localPosition = Vector3.down * 1000000000;
            }
        }


        UpdateAllObjs();
    }

    void ModeAnimate()
    {
        // 
        for (int i = 0; i < 2; i++)
        {
            if (grabber[i].grabbedObject != null)
            {
                GrabbedObjControls(i);
            }

            // 
            if (grabber[0].grabbedObject == null && grabber[1].grabbedObject == null)
            {
                RawState(i);
                CopyFrameData();
            }

            ResetButtons(i);
        }

        // Copy last frame data

        // 
        UpdateOnionSkins();
    }

    void GrabbedObjControls(int i)
    {
        // Scale
        Transform meScale = grabber[i].grabbedObject.transform;
        meScale.localScale += (scaleAxises[scaleAxis] * TouchControls.Joystick[i].y * .01f);
        //meScale.localScale = new Vector3(Mathf.Clamp(meScale.localScale.x, scaleMaxClamp.x, scaleMaxClamp.y), Mathf.Clamp(meScale.localScale.y, scaleMaxClamp.x, scaleMaxClamp.y), Mathf.Clamp(meScale.localScale.y, scaleMaxClamp.x, scaleMaxClamp.y));

        // 
        if (joyIndexReset[i] && TouchControls.Index[i])
        {
            scaleAxis++;
            joyIndexReset[i] = false;

            // 
            if (scaleAxis == scaleAxises.Length)
            {
                scaleAxis = 0;
            }
        }

        // Parent ADD UI FOR THIS TO COMMUNICATE WHATS UP
        if (parent[i] && TouchControls.Top[i])
        {
            parent[i] = false;

            ParentToClosest(grabber[i].grabbedObject);
        }

        // 
        if (joyInReset[i] && TouchControls.In[i])
        {
            grabber[i].Delete();
        }
    }

    void CopyFrameData()
    {
        if (frame > 0 && TouchControls.Index[1])
        {
            for (int j = 0; j < MotionScene.objs.Count; j++)
            {
                if (MotionScene.objs[j][0].parent == null)
                {
                    MotionScene.objs[j][frame].pos = MotionScene.objs[j][0].obj.transform.position = MotionScene.objs[j][frame - 1].pos;
                    MotionScene.objs[j][frame].rot = MotionScene.objs[j][0].obj.transform.eulerAngles = MotionScene.objs[j][frame - 1].rot;
                    MotionScene.objs[j][frame].scale = MotionScene.objs[j][0].obj.transform.localScale = MotionScene.objs[j][frame - 1].scale;
                }
            }
        }
    }

    void UpdateOnionSkins()
    {
        for (int i = 0; i < MotionScene.objs.Count; i++)
        {
            if (MotionScene.objs[i][0].parent == null)
            {
                // Past 0
                if (frame > 0)
                {
                    MotionScene.objs[i][0].b[0].transform.localPosition = MotionScene.objs[i][frame - 1].pos;
                    MotionScene.objs[i][0].b[0].transform.eulerAngles = MotionScene.objs[i][frame - 1].rot;
                    MotionScene.objs[i][0].b[0].transform.localScale = MotionScene.objs[i][frame - 1].scale;
                }
                else
                {
                    MotionScene.objs[i][0].b[0].transform.localPosition = Vector3.down * 1000000000;
                }

                // Past 1
                if (frame > 1)
                {
                    MotionScene.objs[i][0].b[1].transform.localPosition = MotionScene.objs[i][frame - 2].pos;
                    MotionScene.objs[i][0].b[1].transform.eulerAngles = MotionScene.objs[i][frame - 2].rot;
                    MotionScene.objs[i][0].b[1].transform.localScale = MotionScene.objs[i][frame - 2].scale;
                }
                else
                {
                    MotionScene.objs[i][0].b[1].transform.localPosition = Vector3.down * 1000000000;
                }

                // Future 0
                if (frame < lastFrame - 1)
                {
                    MotionScene.objs[i][0].a[0].transform.localPosition = MotionScene.objs[i][frame + 1].pos;
                    MotionScene.objs[i][0].a[0].transform.eulerAngles = MotionScene.objs[i][frame + 1].rot;
                    MotionScene.objs[i][0].a[0].transform.localScale = MotionScene.objs[i][frame + 1].scale;
                }
                else
                {
                    MotionScene.objs[i][0].a[0].transform.localPosition = Vector3.down * 1000000000;
                }

                // Future 1
                if (frame < lastFrame - 2)
                {
                    MotionScene.objs[i][0].a[1].transform.localPosition = MotionScene.objs[i][frame + 2].pos;
                    MotionScene.objs[i][0].a[1].transform.eulerAngles = MotionScene.objs[i][frame + 2].rot;
                    MotionScene.objs[i][0].a[1].transform.localScale = MotionScene.objs[i][frame + 2].scale;
                }
                else
                {
                    MotionScene.objs[i][0].a[1].transform.localPosition = Vector3.down * 1000000000;
                }
            }
        }
    }

    void ResetButtons(int i)
    {
        // 
        if (buttBottomReset[i] && TouchControls.Bottom[i])
        {
            buttBottomReset[i] = false;
            mode = Mode.Play;
            playFrame = frame;
        }

        // 
        if (!TouchControls.Bottom[i])
        {
            buttBottomReset[i] = true;
        }

        if (!TouchControls.Index[i])
        {
            joyIndexReset[i] = true;
        }

        if (!TouchControls.Top[i])
        {
            parent[i] = true;
        }
    }

    public static GameObject GetClosestTo(OVRGrabbable gr)
    {
        GameObject[] grabs = GameObject.FindGameObjectsWithTag("Grab");

        float dist = Mathf.Infinity;
        int id = -1;

        // 
        for (int j = 0; j < grabs.Length; j++)
        {
            float d = Vector3.Distance(grabs[j].transform.localPosition, gr.transform.localPosition);

            if (d == 0)
            {
                continue;
            }

            // 
            if (d < dist)
            {
                dist = d;
                id = j;
            }
        }

        return grabs[id];
    }

    IEnumerator ColorObjs (Renderer a, Renderer b)
    {
        Color currentA = a.material.color;
        Color currentB = b.material.color;

        a.material.color = Color.yellow;
        b.material.color = Color.yellow;

        yield return new WaitForSeconds(.5f);

        a.material.color = currentA;
        b.material.color = currentB;

    }

    public void ParentToClosest(OVRGrabbable gr)
    {
        // Get all grabbable objects in the scene
        GameObject[] grabs = GameObject.FindGameObjectsWithTag("Grab");

        // Init variables that will keep track of closest
        float dist = Mathf.Infinity;
        int best = -1;

        // Loop through all grabbables
        for (int j = 0; j < grabs.Length; j++)
        {
            // Get a ref to the distance between current grabbable & the obj we want to make a child
            float d = Vector3.Distance(grabs[j].transform.localPosition, gr.transform.localPosition);

            // if its the same object, skip it
            if (d == 0)
            {
                continue;
            }

            // if the distance between the two is shorter than the current shortest distance record, save this
            if (d < dist)
            {
                dist = d;
                best = j;
            }
        }

        // Now that we have all the best grabbable, we can do some stuff to it 
        if (best > -1)
        {
            // Parent the main obj
            gr.transform.SetParent(grabs[best].transform);

            // UNTESTED START
            // Parent the befores
            for (int i = 0; i < 2; i++)
            {
                MotionScene.objs[gr.id][0].b[i].transform.SetParent(MotionScene.objs[best][0].b[i].transform);
            }

            // Parent the afters
            // UNTESTED END

            // Update child object's parent
            MotionScene.objs[gr.id][0].parent = grabs[best];

            // Update child object's offset

            StartCoroutine(ColorObjs(grabs[best].GetComponent<Renderer>(), gr.GetComponent<Renderer>()));
        }
    }

    void RawState(int i)
    {
        // Create using left index
        if (joyIndexReset[0] && TouchControls.Index[0])
        {
            joyIndexReset[i] = false;

            CreatePrimitive();
        }

        // 
        float jlY = TouchControls.Joystick[0].y;

        if (Mathf.Abs(jlY) > 0)
        {
            if (joyAxisReset[0])
            {
                joyAxisReset[0] = false;
                uiLoc -= (int)Mathf.Sign(jlY);
                uiLoc = Mathf.Clamp(uiLoc, 0, 4);
            }
        }
        else if (jlY == 0)
        {
            joyAxisReset[0] = true;
            axisHold[0].y = 0;
        }
        // Frame manipulation
        float jrX = TouchControls.Joystick[1].x;

        // 
        if (Mathf.Abs(jrX) > 0)
        {
            axisHold[1].x += Time.deltaTime;

            // 
            if (joyAxisReset[1])
            {
                joyAxisReset[1] = false;

                // 
                if (uiLoc == 0)
                {
                    frame += (int)Mathf.Sign(jrX);
                    frame = Mathf.Clamp(frame, 0, lastFrame - 1);
                    UpdateAllObjs();
                }
                else if (uiLoc == 1)
                {
                    fps += (int)Mathf.Sign(jrX);
                    fps = Mathf.Clamp(fps, 1, 120);
                }
                else if (uiLoc == 2)
                {
                    lastFrame += (int)Mathf.Sign(jrX);
                    lastFrame = Mathf.Clamp(lastFrame, 1, 1000);
                }
                else if (uiLoc == 3)
                {
                    pType += (int)Mathf.Sign(jrX);
                    pType = Mathf.Clamp(pType, 0, pTypes.Length-1);
                }
                else if (uiLoc == 4)
                {
                    col += (int)Mathf.Sign(jrX);
                    col = Mathf.Clamp(col, 0, cols.Length - 1);
                }

                //for (int j = 0; j < MotionScene.objs.Count; j++)
                //{
                //    MotionScene.objs[i][0].obj.transform.localPosition = MotionScene.objs[i][frame].pos;
                //    MotionScene.objs[i][0].obj.transform.eulerAngles = MotionScene.objs[i][frame].rot;
                //    MotionScene.objs[i][0].obj.transform.localScale = MotionScene.objs[i][frame].scale;
                //}
            }
        }
        else if (jrX == 0)
        {
            joyAxisReset[1] = true;
            axisHold[1].x = 0;
        }

        // 
        if (axisHold[1].x > lean)
        {
            if (uiLoc == 0)
            {
                frame += (int)Mathf.Sign(jrX);
                frame = Mathf.Clamp(frame, 0, lastFrame - 1);
                UpdateAllObjs();
            }
            else if (uiLoc == 1)
            {
                fps += (int)Mathf.Sign(jrX);
                fps = Mathf.Clamp(fps, 1, 120);
            }
            else if (uiLoc == 2)
            {
                lastFrame += (int)Mathf.Sign(jrX);
                lastFrame = Mathf.Clamp(lastFrame, 1, 1000);
            }
        }
    }

    void UpdateAllObjs()
    {
        for (int i = 0; i < MotionScene.objs.Count; i++)
        {
            if (MotionScene.objs[i][0].parent == null)
            {
                MotionScene.objs[i][0].obj.transform.localPosition = MotionScene.objs[i][frame].pos;
                MotionScene.objs[i][0].obj.transform.eulerAngles = MotionScene.objs[i][frame].rot;
                MotionScene.objs[i][0].obj.transform.localScale = MotionScene.objs[i][frame].scale;
            }
        }
    }

    GameObject CreatePrimitive()
    {
        GameObject g = GameObject.CreatePrimitive(pTypes[pType]);
        g.transform.localPosition = cam.transform.forward * .25f + (Vector3.up) + (Vector3.forward / 2);
        g.transform.localScale = Vector3.one * .5f;
        g.tag = "Grab";

        Renderer r = g.GetComponent<Renderer>();
        r.material.color = cols[col];

        Rigidbody rb = g.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        OVRGrabbable grab = g.AddComponent<OVRGrabbable>();
        grab.enabled = true;
        grab.id = creation;
        creation++;

        MotionObj[] mO = new MotionObj[10000];

        for (int i = 0; i < mO.Length; i++)
        {
            mO[i] = new MotionObj() { scale = Vector3.one * .5f };
        }

        // Set the object
        mO[0].obj = g;

        MotionScene.objs.Add(mO);

        CreateOnionSkins(grab.id, g.GetComponent<MeshFilter>().mesh);

        return g;
    }
}
