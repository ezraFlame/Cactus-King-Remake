using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MoneyMaker : MonoBehaviour
{
    [SerializeField]
    private int amount;
    [SerializeField]
    private float moneyTime;
    private float moneyCounter;

    private void Update() {
        if (moneyCounter > 0) moneyCounter -= Time.deltaTime;
    }

    private void OnTriggerStay(Collider other) {
        if (NetworkManager.Singleton.IsServer && moneyCounter <= 0 && other.TryGetComponent<MoneyManager>(out MoneyManager moneyManager)) {
            moneyManager.money.Value += amount;
            moneyCounter = moneyTime;
        }
    }
}
