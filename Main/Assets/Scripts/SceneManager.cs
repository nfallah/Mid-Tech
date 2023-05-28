using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    private string[] dialogs;

    private void Awake()
    {
        Canvas.ForceUpdateCanvases(); // Due to Unity's scriptloading, dialog border generation messes up without this call
    }

    void Start()
    {
        dialogs = new string[] {
            "Hey!",
            "How's it going?",
            "Just wanted to let you know that I'm a human being!",
            "To clarify in case you forgot, I am 100% a human being and NOT a skinwalker in an ethically obtained human body.",
            "Also me when I purposefully spread misinformation on the interwebs -- we do a slight bit of trolling.",
            "Your IP address is: 192.162.1.2. Don't turn around. Keep reading, because *they* know. If you start looking around now you'll arouse their suspicions. Don't turn around."
        };

        //DialogManager.getDialogManager().GenerateDialog(dialogs, "I am a human being", Color.yellow);
        //KillYourself();
        //Invoke("Testing", 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Testing()
    {
        DialogManager.getDialogManager().GenerateDialog(dialogs, "AGABABABAGAGAGAGA :)");
    }
}