using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
public class UIGameManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button SubmitButton;

    public GameObject LoginPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SubmitButton.onClick.AddListener(OnsumitName);
        LoginPanel.SetActive(false);

        GameManager2.Instance.OnConnection += () =>
        {
            LoginPanel.SetActive(true);
            inputField.text = "";
            SubmitButton.interactable = true;
            inputField.interactable = true;
        };
    }

    public void OnsumitName()
    {
        string accountID= inputField.text;
        if (!string.IsNullOrEmpty(accountID))
        {
            GameManager2.Instance.RegisterPlayerServerRpc(accountID, NetworkManager.Singleton.LocalClientId);
            SubmitButton.interactable= false;
            inputField.interactable=false;

            LoginPanel.SetActive(false);
        }
    }
}
