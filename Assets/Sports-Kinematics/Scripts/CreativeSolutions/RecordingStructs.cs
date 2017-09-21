using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;

public struct BallData
{
    public Vector3 position;
    public bool isOccluded;
}

public struct StrikerData
{
    public Vector3 position;
    public Vector3 rotation;
    public bool isOccluded;
}

public struct Joint
{
    public string name;
    public Vector3 position;
    public bool isOccluded;
}

public struct SkeletonData
{
    public List<Joint> joints;
}

public struct ConfigData
{
    public List<string> jointsToOcclude;
    public float jointOccludeStartTime;
    public float jointOccludeEndTime;

    public float ballOccludeStartTime;
    public float ballOccludeEndTime;

    public float strikerOccludeStartTime;
    public float strikerOccludeEndTime;
}

public struct FileNames
{
    public string actionName;
    public string ballDataFileName;
    public string strikerDataFileName;
    public string skeletonDataFileName;
    public string configDataFileName;
}