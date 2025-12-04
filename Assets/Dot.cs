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

    [Header("Debug")]
    public bool insideFOV = false;
    public bool hasLineOfSight = false;

    void Update()
    {
        CheckIfPlayerIsInFront();
        CheckFieldOfView();
        CheckLineOfSight();
        UpdateDirectionUI();

        if (insideFOV && hasLineOfSight)
        {
            RotateTowardPlayer();
        }
    }

    void CheckIfPlayerIsInFront()
    {
        Vector3 toPlayer = (player.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, toPlayer);

        // Player is in front if dot > 0
        insideFOV = dot > Mathf.Cos(fieldOfView * Mathf.Deg2Rad);
    }

    void CheckFieldOfView()
    {
        Vector3 toPlayer = (player.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, toPlayer);
        float threshold = Mathf.Cos(fieldOfView * Mathf.Deg2Rad);

        insideFOV = dot > threshold;
    }

    void CheckLineOfSight()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, viewDistance))
        {
            if (hit.transform == player)
            {
                hasLineOfSight = true;
                return;
            }
        }

        hasLineOfSight = false;
    }

   
    void RotateTowardPlayer()
    {
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0;

        Quaternion targetRot = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * 3f);
    }

    
    void UpdateDirectionUI()
    {
        Vector3 toPlayer = (player.position - transform.position).normalized;

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
