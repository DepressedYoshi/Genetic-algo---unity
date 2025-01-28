using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public string playerTag = "Player"; // Tag assigned to the player prefab
    public float smoothSpeed = 0.1f; // Speed of the camera's movement
    public Vector3 offset; // Offset to maintain a desired position relative to the target
    private Transform target; // The current target for the camera

    void LateUpdate()
    {
        UpdateTarget(); // Find the rightmost player
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
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

            if (player.transform.position.y < 0.75f) continue;

            if (player.transform.position.x > maxX)
            {
                maxX = player.transform.position.x;
                rightmostPlayer = player.transform;
            }
        }

        // Update the target to the rightmost player
        target = rightmostPlayer;
    }
}
