using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSaver : MonoBehaviour
{
    public static SceneSaver Instance;

    public int runs = 0;
    public int lev = 0;
    public int exp = 0;
    public List<int> min = new List<int>();
    public List<int> it = new List<int>();
    public List<int> am = new List<int>();

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
