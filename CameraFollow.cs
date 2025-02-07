using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public string playerTag = "Player"; // Tag assigned to the player prefab
    public float smoothSpeed = 0.3f; // Speed of the camera's movement
    public Vector3 offset; // Offset to maintain a desired position relative to the target
    public float deadZoneRange;// how far does the target has to move to the right from the center of the camera for it to move 
    private Transform target; // The current target for the camera
    private Vector3 velocity = Vector3.zero; // For SmoothDamp

    void Awake(){
        deadZoneRange = calculatesZone();
    }

    void LateUpdate()
    {
        UpdateTarget(); // Find the rightmost player
        if (target != null && needToMove(target))
        {
            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);

        }
    }

    private bool needToMove(Transform target)
    {
        bool tooRight = target.transform.position.x - deadZoneRange > transform.position.x;
        bool tooLeft = target.transform.position.x + deadZoneRange < transform.position.x;
        //if the target moves beyong the zone, move
        return tooLeft || tooRight;
    }

    void UpdateTarget()
    {
        // Find all GameObjects with the specified tag
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        if (players.Length == 0)
        {
            target = null;
            return;
        }

        // Determine the rightmost player
        float maxX = float.MinValue;
        Transform rightmostPlayer = null;

        foreach (GameObject player in players)
        {
            if (player.transform.position.x > maxX)
            {
                maxX = player.transform.position.x;
                rightmostPlayer = player.transform;
            }
        }

        // Update the target to the rightmost player
        target = rightmostPlayer;
    }
    private static float calculatesZone()
    {
        Camera camera = Camera.main;
        float halfWidth = camera.orthographicSize * camera.aspect;
        return halfWidth / 2;
    }
}