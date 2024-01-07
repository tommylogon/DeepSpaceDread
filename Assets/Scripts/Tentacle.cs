using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
    public int tentacleLength = 10;
    public LineRenderer tentacleLineRenderer;
    public Vector3[] segmentPoses;
    public Vector3[] segmentV;

    public Transform targetDir;
    public float targetDistance;
    public float smoothSpeed;

    // Start is called before the first frame update
    void Start()
    {
        tentacleLineRenderer = GetComponent<LineRenderer>();
        segmentPoses = new Vector3[tentacleLength];
        segmentV = new Vector3[tentacleLength];

        for(int i = 0; i < segmentPoses.Length; i++)
        {
            segmentPoses[i] = targetDir.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        segmentPoses[0] = targetDir.position;
        for(int i = 1; i < tentacleLength; i++)
        {
            segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], segmentPoses[i - 1]+targetDir.right * targetDistance, ref segmentV[i], smoothSpeed);
        }
        tentacleLineRenderer.SetPositions(segmentPoses);
    }
}
