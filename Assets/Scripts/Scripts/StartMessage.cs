using TMPro;
using UnityEngine;

public class StartMessage : MonoBehaviour
{

    public TextMeshProUGUI messageText;
    public float duration;

    private void Start()
    {
        messageText.gameObject.SetActive(true); ;


        Invoke("HideMessage", duration);
    }

    private void HideMessage()
    {
        messageText?.gameObject.SetActive(false); 
    }


}
