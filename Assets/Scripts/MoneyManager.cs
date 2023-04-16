using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MoneyManager : NetworkBehaviour
{
    public NetworkVariable<int> money = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner) {
            money.OnValueChanged += OnMoneyChanged;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsOwner) {
            money.OnValueChanged -= OnMoneyChanged;
        }
    }

    private void OnMoneyChanged(int previous, int current) {
        if (IsOwner) {
            UIManager.Instance.SetMoneyText(current);
        }
    }
}
