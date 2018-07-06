using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CameraPosition
{
    public Vector3 position;
    public Vector3 orientationEuler;
}

public class CameraBehaviour : MonoBehaviour
{
    public List<CameraPosition> availableCameraPositions;

    private int currentCameraPositionIndex;

    private CameraPosition targetCameraPosition;
    private Coroutine cameraMoveCoroutine;

    // Use this for initialization
    void Start ()
    {
        currentCameraPositionIndex = 4;
        cameraMoveCoroutine = null;
        SetPosition(availableCameraPositions[currentCameraPositionIndex]);
    }
	
	// Update is called once per frame
	void Update ()
    {
        bool change = false;
		if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            currentCameraPositionIndex = (currentCameraPositionIndex + 1) % availableCameraPositions.Count;
            change = true;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            currentCameraPositionIndex = (currentCameraPositionIndex - 1 + availableCameraPositions.Count) % availableCameraPositions.Count;
            change = true;
        }
        if (change)
        {
            SetPosition(availableCameraPositions[currentCameraPositionIndex]);
        }
    }

    private void SetPosition(CameraPosition newPosition)
    {
        targetCameraPosition = newPosition;

        if (cameraMoveCoroutine != null)
        {
            StopCoroutine(cameraMoveCoroutine);
            cameraMoveCoroutine = null;
        }
        cameraMoveCoroutine = StartCoroutine(WaitAndChangePosition(0.01f, 0));
    }

    IEnumerator WaitAndChangePosition(float delay, float t)
    {
        yield return new WaitForSeconds(delay);

        Quaternion currentOrientation = this.transform.rotation;
        Vector3 currentPosition = this.transform.position;

        Quaternion nextOrientation = Quaternion.Slerp(currentOrientation, Quaternion.Euler(targetCameraPosition.orientationEuler), t);
        Vector3 nextPosition = Vector3.Lerp(currentPosition, targetCameraPosition.position, t);

        this.transform.rotation = nextOrientation;
        this.transform.position = nextPosition;

        cameraMoveCoroutine = StartCoroutine(WaitAndChangePosition(0.01f, t+0.01f));
    }
}
