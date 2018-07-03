using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturePointMap : MonoBehaviour
{
    [SerializeField]
    public CapturePoint pointA;
    [SerializeField]
    private CapturePoint pointB;
    [SerializeField]
    private CapturePoint pointC;

    [SerializeField]
    private CapturePoint pointD;
    [SerializeField]
    private CapturePoint pointE;
    [SerializeField]
    private CapturePoint pointF;

    [SerializeField]
    private CapturePoint pointG;
    [SerializeField]
    private CapturePoint pointH;
    [SerializeField]
    private CapturePoint pointI;

    public Dictionary<CapturePoint.CapturePointId, CapturePoint> capturePoints;

    // Use this for initialization
    void Awake () {
		capturePoints = new Dictionary<CapturePoint.CapturePointId, CapturePoint>();

        capturePoints.Add(CapturePoint.CapturePointId.A, pointA);
        capturePoints.Add(CapturePoint.CapturePointId.B, pointB);
        capturePoints.Add(CapturePoint.CapturePointId.C, pointC);

        capturePoints.Add(CapturePoint.CapturePointId.D, pointD);
        capturePoints.Add(CapturePoint.CapturePointId.E, pointE);
        capturePoints.Add(CapturePoint.CapturePointId.F, pointF);

        capturePoints.Add(CapturePoint.CapturePointId.G, pointG);
        capturePoints.Add(CapturePoint.CapturePointId.H, pointH);
        capturePoints.Add(CapturePoint.CapturePointId.I, pointI);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
