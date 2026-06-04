using UnityEngine;

public class StairsZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entró a escalera");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Salió de escalera");
    }
}