using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    private string[] loremIpsum;

    void Start()
    {
        loremIpsum = new string[] {
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
            "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
            "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
        };

        // DialogManager.Instance.GenerateDialog(loremIpsum, "Lorem Ipsum", Color.red);
        // Invoke("NextDialog", 10);
    }

    void Update()
    {

    }

    private void NextDialog()
    {
        DialogManager.Instance.GenerateDialog(loremIpsum, "King Latthew J. Moaces XVI");
    }
}