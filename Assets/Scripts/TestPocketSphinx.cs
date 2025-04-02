using UnityEngine;
using System.Runtime.InteropServices;

public class TestPocketSphinx : MonoBehaviour
{
    [DllImport("pocketsphinx")]
    private static extern void ps_init();

    void Start()
    {
        Debug.Log("Initializing PocketSphinx...");
        ps_init();
        Debug.Log("PocketSphinx Initialized Successfully!");
    }
}
