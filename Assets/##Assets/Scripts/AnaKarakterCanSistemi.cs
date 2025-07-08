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
        Debug.Log("Can azald�: " + can);
    }

    public void CanArttir(float miktar)
    {
        can += miktar;
        if (can > maksimumCan)
        {
            can = maksimumCan;
        }
        Debug.Log("Can artt�: " + can);
    }


    private void Olum()
    {
        // �l�m i�lemleri -
        Debug.Log("Karakter �ld�!");
        playerController.Die(); 
        //animat�r� kapat

    }


    private void Update()
    {
        if (canBar != null)
        {
            canBar.fillAmount = can / maksimumCan;
        }
    }
}

