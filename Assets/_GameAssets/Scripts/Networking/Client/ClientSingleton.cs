using Cysharp.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    public static ClientSingleton Instance { get; private set; }

    public ClientManager ClientManager { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async UniTask<bool> CreateClient()
    {
        ClientManager = new ClientManager();
        return await ClientManager.InitAsync();
    }
}
