using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRubyShared;

public class GameCamera : MonoBehaviour {

    public Ball player;
    public float XFactor, YFactor = 5;
    public Vector3 OriginOffset;
    [HideInInspector]public Vector3 offset;
	void Start () {
        var panGesture = new PanGestureRecognizer();
        panGesture.MinimumNumberOfTouchesToTrack = 1;
        panGesture.StateUpdated += PanGesture_StateUpdated;
        FingersScript.Instance.AddGesture(panGesture);
        offset = OriginOffset;
    }

    private void PanGesture_StateUpdated(GestureRecognizer gesture)
    {
        var pangesture = (PanGestureRecognizer)gesture;
        if (pangesture.State == GestureRecognizerState.Executing && Input.GetMouseButton(1))
        {
            var vector =new Vector3(/*pangesture.DeltaY / YFactor*/0, -pangesture.DeltaX / XFactor, 0);
            Rotate(vector);
        }
    }

    public void Rotate(Vector3 vector)
    {
        var rot = Quaternion.Euler(vector);
        offset = rot * offset;
    }

    // Update is called once per frame
    void LateUpdate () {
        float horizontalDrift = Input.GetAxis("Horizontal");
        Rotate(new Vector3(0, horizontalDrift, 0));
        if (player)
        {
            var point = player.transform.position;
            point.y = transform.position.y;
            transform.position = player.transform.position+ offset;
            transform.LookAt(point, Vector3.up);
        }
    }
}
