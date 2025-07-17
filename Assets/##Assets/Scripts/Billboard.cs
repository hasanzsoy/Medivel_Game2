using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform target;         // D��man�n kafas�na konumlanacak hedef (genelde d��man kendisi)
    public Vector3 offset = new(0, 2f, 0); // Kafa �st� pozisyon ayar�

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + offset;

        // Kamera y�n�ne do�ru bakmas� i�in billboard etkisi
        transform.forward = Camera.main.transform.forward;
    }
}
