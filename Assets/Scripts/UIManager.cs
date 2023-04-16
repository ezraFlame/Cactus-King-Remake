using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private UnityTransport transport;

    [SerializeField]
    private GameObject pauseMenu;
    [HideInInspector]
    public bool paused;
    [SerializeField]
    private PlayerInput input;

    [SerializeField]
    private TMP_InputField ipAddressInput;
    [SerializeField]
    private TMP_InputField portInput;

    [SerializeField]
    private TMP_Text moneyText;

    [HideInInspector]
    public bool isInspecting;
    [SerializeField]
    private GameObject inspectMenu;
    [SerializeField]
    private TMP_Text inspectMoneyText;

    private void Start() {
        pauseMenu.SetActive(false);
        paused = false;

        inspectMenu.SetActive(false);
        isInspecting = false;
    }

    private void Awake() {
        transport = FindObjectOfType<UnityTransport>();
        input = GetComponent<PlayerInput>();
        Instance = this;
    }

    public void Quit() {
        if (Application.isEditor) return;
        Application.Quit();
    }

    async public void Join() {
        if (!paused) {
            Debug.Log("You must be paused to join a game!");
            return;
        }

        if (!ushort.TryParse(portInput.text, out ushort result)) {
            Debug.Log("Port " + portInput.text + " is not valid!");
            return;
        }

        NetworkManager.Singleton.Shutdown();

        await Task.Delay(500);

        transport.ConnectionData.Port = result;
        transport.ConnectionData.Address = ipAddressInput.text;
        NetworkManager.Singleton.StartClient();
    }

    public void TogglePause() {
        paused = !paused;
        pauseMenu.SetActive(paused);
        Cursor.visible = paused;
        FindObjectOfType<Cinemachine.CinemachineVirtualCamera>().enabled = !paused;
        if (paused) Cursor.lockState = CursorLockMode.None;
        else Cursor.lockState = CursorLockMode.Locked;
    }

    public void SetMoneyText(int money) {
        moneyText.text = "" + money;
    }

    public void OpenInspect(int money) {
        isInspecting = true;
        inspectMenu.SetActive(true);
        inspectMoneyText.text = "$" + money;
    }

    public void CloseInspect() {
        isInspecting = false;
        inspectMenu.SetActive(false);
    }
}
