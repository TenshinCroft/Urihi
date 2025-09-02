using UnityEngine;

public class PortaInicio : MonoBehaviour
{
public Transform doorTransform;
    public float openAngle = 90f;
    public float openSpeed = 2f;
    private bool isPlayerNear = false;
    private bool isOpen = false;

    void Update()
    {
        if (isPlayerNear && ChavePortaInicio.hasKey && !isOpen)
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        isOpen = true;
        StartCoroutine(RotateDoor());
    }

    System.Collections.IEnumerator RotateDoor()
    {
        Quaternion startRotation = doorTransform.rotation;
        Quaternion endRotation = Quaternion.Euler(doorTransform.eulerAngles + Vector3.up * openAngle);
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * openSpeed;
            doorTransform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}
