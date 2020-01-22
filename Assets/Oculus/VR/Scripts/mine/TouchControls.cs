using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TouchControls 
{
    public static bool[] Bottom { get { return new bool[] { OVRInput.Get(OVRInput.Button.Three), OVRInput.Get(OVRInput.Button.One) }; } }
    public static bool[] Top { get { return new bool[] { OVRInput.Get(OVRInput.Button.Four), OVRInput.Get(OVRInput.Button.Two) }; } }
    public static bool[] Palm { get { return new bool[] { OVRInput.Get(OVRInput.Button.PrimaryHandTrigger), OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) }; } }
    public static bool[] Index { get { return new bool[] { OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger), OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) }; } }
    public static bool[] In { get { return new bool[] { OVRInput.Get(OVRInput.Button.PrimaryThumbstick), OVRInput.Get(OVRInput.Button.SecondaryThumbstick) }; } }
    public static bool[] Right { get { return new bool[] { OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight), OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight) }; } }
    public static bool[] Left { get { return new bool[] { OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft), OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft) }; } }
    public static bool[] Up { get { return new bool[] { OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp), OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp) }; } }
    public static bool[] Down { get { return new bool[] { OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown), OVRInput.Get(OVRInput.Button.SecondaryThumbstickDown) }; } }
    public static Vector2[] Joystick { get { return new Vector2[] { OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick), OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick) }; } }
}
