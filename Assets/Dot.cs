using UnityEngine;
using TMPro;

public class Dot : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public TextMeshProUGUI directionText;

    [Header("Settings")]
    public float fieldOfView = 45f;
    public float viewDistance = 15f;
    public float rotationSpeed = 3f;

    [Header("Debug")]
    public bool insideFOV;
    public bool hasLineOfSight;

    void Update()
    {
        Vector3 toPlayer = (player.position - transform.position).normalized;

        insideFOV = IsInsideFOV(toPlayer);
        hasLineOfSight = HasLineOfSight(toPlayer);

        UpdateDirectionUI(toPlayer);

        if (insideFOV && hasLineOfSight)
            RotateTowardPlayer(toPlayer);
    }

    bool IsInsideFOV(Vector3 toPlayer)
    {
        float dot = Vector3.Dot(transform.forward, toPlayer);
        float threshold = Mathf.Cos(fieldOfView * Mathf.Deg2Rad);
        return dot > threshold;
    }

    bool HasLineOfSight(Vector3 toPlayer)
    {
        if (Physics.Raycast(transform.position, toPlayer, out RaycastHit hit, viewDistance))
            return hit.transform == player;

        return false;
    }


    void RotateTowardPlayer(Vector3 toPlayer)
    {
        Vector3 lookDir = toPlayer;
        lookDir.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }


    void UpdateDirectionUI(Vector3 toPlayer)
    {
        float dot = Vector3.Dot(transform.forward, toPlayer);
        Vector3 cross = Vector3.Cross(transform.forward, toPlayer);

        if (dot > 0.7f)
            directionText.text = "PLAYER IN FRONT";
        else if (dot < -0.7f)
            directionText.text = "PLAYER BEHIND";
        else if (cross.y > 0)
            directionText.text = "PLAYER LEFT";
        else
            directionText.text = "PLAYER RIGHT";
    }


    void OnDrawGizmos()
    {
        if (player == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 3);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, player.position);

        Gizmos.color = Color.yellow;
        Quaternion leftRot = Quaternion.Euler(0, -fieldOfView, 0);
        Quaternion rightRot = Quaternion.Euler(0, fieldOfView, 0);
        Gizmos.DrawLine(transform.position, transform.position + leftRot * transform.forward * 3);
        Gizmos.DrawLine(transform.position, transform.position + rightRot * transform.forward * 3);
    }
}
