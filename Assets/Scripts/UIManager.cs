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
    public bool paused;
    [SerializeField]
    private PlayerInput input;

    [SerializeField]
    private TMP_InputField ipAddressInput;
    [SerializeField]
    private TMP_InputField portInput;

    [SerializeField]
    private TMP_Text moneyText;

    private void Start() {
        pauseMenu.SetActive(false);
        paused = false;
    }

    private void Awake() {
        transport = FindObjectOfType<UnityTransport>();
        input = GetComponent<PlayerInput>();
        Instance = this;
    }

    private void Update() {
        if (input.actions["Pause"].WasPressedThisFrame()) {
            paused = !paused;
            pauseMenu.SetActive(paused);
            Cursor.visible = paused;
            FindObjectOfType<Cinemachine.CinemachineVirtualCamera>().enabled = !paused;
            if (paused) Cursor.lockState = CursorLockMode.None;
            else Cursor.lockState = CursorLockMode.Locked;
        }
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

        await Task.Yield();

        transport.ConnectionData.Port = result;
        transport.ConnectionData.Address = ipAddressInput.text;
        NetworkManager.Singleton.StartClient();
    }

    public void SetMoneyText(int money) {
        moneyText.text = "" + money;
    }
}
