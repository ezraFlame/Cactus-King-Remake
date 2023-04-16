using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkCommandLine : MonoBehaviour
{
    private NetworkManager netManager;

    void Start()
    {
        netManager = GetComponent<NetworkManager>();

        var args = GetCommandlineArgs();

        #if UNITY_SERVER
        if (args.TryGetValue("-port", out string port)) {
            GetComponent<UnityTransport>().ConnectionData.Port = ushort.Parse(port);
        }
        netManager.StartServer();
        #else
        if (args.TryGetValue("-mode", out string mode))
        {
            switch (mode)
            {
                case "server":
                    netManager.StartServer();
                    break;
                case "host":
                    netManager.StartHost();
                    break;
                case "client":
                    netManager.StartClient();
                    break;
            }
        } else {
            netManager.StartHost();
        }
        #endif
    }

    private Dictionary<string, string> GetCommandlineArgs()
    {
        Dictionary<string, string> argDictionary = new Dictionary<string, string>();

        var args = System.Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; ++i)
        {
            var arg = args[i].ToLower();
            if (arg.StartsWith("-"))
            {
                var value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
                value = (value?.StartsWith("-") ?? false) ? null : value;

                argDictionary.Add(arg, value);
            }
        }
        return argDictionary;
    }
}