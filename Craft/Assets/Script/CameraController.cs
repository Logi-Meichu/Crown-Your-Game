using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

// Transforms to act as start and end markers for the journey.
    public Transform startMarker;
    public Transform endMarker;
    public Transform midMarker;

    // Movement speed in units/sec.
    public float speed = 1.0F;

    // Time when the movement started.
    private float startTimeRoomIn;
    private float startTimeRoomOut;

    // Total distance between the markers.
    private float journeyLength;

    private bool outDone;

    void Start() {
    }

    // Follows the target position like with a spring
    void Update() {
        if(Input.GetKeyDown(KeyCode.Home)) startTimeRoomIn = Time.time;
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (outDone) return;
            startTimeRoomOut = Time.time;
            outDone = true;
        }
        RoomIn();
        RoomOut();
    }

    void RoomOut() {
        if (startTimeRoomOut == 0) return;
        // Calculate the journey length.
        journeyLength = Vector3.Distance(endMarker.position, midMarker.position);

        // Distance moved = time * speed.
        float distCovered = (Time.time - startTimeRoomOut) * speed;

        // Fraction of journey completed = current distance divided by total distance.
        float fracJourney = distCovered / journeyLength;

        // Set our position as a fraction of the distance between the markers.
        transform.position = Vector3.Lerp(endMarker.position, midMarker.position, fracJourney);
    }
    void RoomIn() {
        if (startTimeRoomIn == 0) return;
        // Calculate the journey length.
        journeyLength = Vector3.Distance(startMarker.position, endMarker.position);

        // Distance moved = time * speed.
        float distCovered = (Time.time - startTimeRoomIn) * speed;

        // Fraction of journey completed = current distance divided by total distance.
        float fracJourney = distCovered / journeyLength;

        // Set our position as a fraction of the distance between the markers.
        transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fracJourney);
    }
}
