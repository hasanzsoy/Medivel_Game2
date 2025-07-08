using UnityEngine;
using UnityEngine.UI;

public class AnaKarakterCanSistemi : MonoBehaviour
{
    [SerializeField] private float can = 100; 
    public float Can => can; 
    public float maksimumCan = 100; 
    public Image canBar;

    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void CanAzalt(float miktar)
    {
        can -= miktar;
        if (can < 0)
        {
            can = 0;
            Olum();
        }
        Debug.Log("Can azaldý: " + can);
    }

    public void CanArttir(float miktar)
    {
        can += miktar;
        if (can > maksimumCan)
        {
            can = maksimumCan;
        }
        Debug.Log("Can arttý: " + can);
    }


    private void Olum()
    {
        // Ölüm iþlemleri -
        Debug.Log("Karakter öldü!");
        playerController.Die(); 
        //animatörü kapat

    }


    private void Update()
    {
        if (canBar != null)
        {
            canBar.fillAmount = can / maksimumCan;
        }
    }
}

