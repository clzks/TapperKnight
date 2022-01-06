using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ForceStart : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void FirstLoad()
    {
        if (SceneManager.GetActiveScene().name == "TestScene")
        {
            return;
        }

        if (SceneManager.GetActiveScene().name.CompareTo("LoadingScene") != 0)
        {
            SceneManager.LoadScene("LoadingScene");
        }
    }
}
