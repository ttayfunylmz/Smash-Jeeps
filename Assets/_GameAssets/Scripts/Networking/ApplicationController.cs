using Cysharp.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton _clientPrefab;
    [SerializeField] private HostSingleton _hostPrefab;

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async UniTask LaunchInMode(bool isDedicatedServer)
    {
        if(isDedicatedServer)
        {

        }
        else
        {
            HostSingleton hostSingletonInstance = Instantiate(_hostPrefab);
            hostSingletonInstance.CreateHost();
            
            ClientSingleton clientSingletonInstance = Instantiate(_clientPrefab);
            bool authenticated = await clientSingletonInstance.CreateClient();

            if(authenticated)
            {
                clientSingletonInstance.ClientManager.GoToMenu();
            }
        }
    }
}
