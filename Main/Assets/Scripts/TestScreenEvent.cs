using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScreenEvent : ScreenEvent
{
    public override void Execute(int screenEventIndex)
    {
        switch (screenEventIndex)
        {
            case 0:
                Debug.Log("Top Half!");
                break;
            case 1:
                Debug.Log("Bottom Half!");
                break;
        }
    }
}