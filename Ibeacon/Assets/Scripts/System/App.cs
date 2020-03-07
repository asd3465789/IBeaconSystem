using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App : MonoBehaviour
{
    private void Start()
    {
        AppManager.Instance.Init();
    }

    private void Update()
    {
        AppManager.Instance.AppManagerUpdate();
    }
}
