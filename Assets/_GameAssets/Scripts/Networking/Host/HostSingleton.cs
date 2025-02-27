using Cysharp.Threading.Tasks;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    public static HostSingleton Instance { get; private set; }

    public HostManager HostManager { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CreateHost()
    {
        HostManager = new HostManager();
    }
}
