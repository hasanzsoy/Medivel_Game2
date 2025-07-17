using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform target;         // Düþmanýn kafasýna konumlanacak hedef (genelde düþman kendisi)
    public Vector3 offset = new(0, 2f, 0); // Kafa üstü pozisyon ayarý

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + offset;

        // Kamera yönüne doðru bakmasý için billboard etkisi
        transform.forward = Camera.main.transform.forward;
    }
}
