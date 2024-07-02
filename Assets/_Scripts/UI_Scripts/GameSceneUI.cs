using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GameSceneUI : MonoBehaviour
{
    private void Awake()
    {
        transform.Find("mainMenuBtn").GetComponent<Button_UI>().ClickFunc = () =>
        {
            Debug.Log("Click Main Menu");
            Loader.Load(Loader.Scene.Loading);
            Loader.Load(Loader.Scene.Room1_Tutorial);
        };
    }
}
