using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    private static DialogManager instance; // Singleton state reference

    private static bool isCreated, // Utilized to enforce a singleton state pattern
    isRunning; // Ensures that multiple dialogs cannot be generated at the same time

    // Private constructor; ensures there is no instance creation
    private DialogManager() {}

    private class Status { public bool isComplete = false; } // Utilized to terminate coroutines via reference by address

    [SerializeField]
    Color borderColor,
    defaultColor; // Default value utilized if a dialog is generated without a color parameter, also determines the alpha value for all dialogues

    [SerializeField]
    float dialogSpeed, // Time between each character appearing
    transitionDelay; // The minimum waiting time before potentially transitioning (transitioning: skipping current dialogue OR moving to next dialogue)

    [SerializeField] int borderThickness;

    [SerializeField] GameObject root; // The root object that contains the text as well as other UI elements

    [SerializeField]
    Image dialogBackground, // Background image for the dialog
    nameBackground; // Background image for the name

    [SerializeField]
    Text dialogText, // Where the dialogue is displayed
    formatNameText, // Original, invisible copy of the name text used to format its background
    nameText; // Reference, visible copy of the name text identical to the original

    private bool shouldTransition; // Determines whether a transition can occur

    private float defaultAlpha; // The transparency that all dialog backgrounds will inherit

    private string actualDialog; // What is currently being displayed on the user screen

    private string[] dialogs; // Reference to inputted list of dialogs; shared amongst coroutines

    // Ensures there is only one instance; hides root object
    private void Awake()
    {
        // Destroys script reference if it has already been established anywhere else
        if (isCreated)
        {
            Destroy(this);
            throw new Exception("DialogManager instance already established; terminating current reference.");
        }

        instance = this;
        isCreated = true;
        root.SetActive(false); // Once initiation is complete, make dialog interface not visible until it is called
        defaultAlpha = defaultColor.a;
        GenerateBorders();
    }

    // The main method that initiates the dialog process
    public void GenerateDialog(string[] dialogs, string name)
    {
        GenerateDialog(dialogs, name, defaultColor);
    }

    // The main method that initiates the dialog process with a color-specific parameter
    public void GenerateDialog(string[] dialogs, string name, Color color)
    {
        if (isRunning) throw new System.Exception("GenerateDialog instance already called from another source; terminating process.");
        if (dialogs.Length == 0) throw new System.Exception("Dialog list of length 0 is invalid.");
        isRunning = true;
        this.dialogs = dialogs;
        color.a = defaultAlpha;
        dialogBackground.color = nameBackground.color = color;
        formatNameText.text = nameText.text = name;
        StartCoroutine(Enter(0));
    }

    // Called at the beginning, as well as whenever a dialogue is completed
    private IEnumerator Enter(int currentIndex)
    {
        // First dialogue
        if (currentIndex == 0)
        {
            root.SetActive(true);
        }

        // Second dialogue and onward
        else
        {
            // Idles until transition key is pressed (given transitioning is allowed)
            while (!Input.GetKeyDown(Settings.dialogueInteractKey) || !shouldTransition)
            {
                yield return null;
            }

            dialogText.text = "";

            // Last dialogue
            if (currentIndex == dialogs.Length)
            {
                root.SetActive(false);
                dialogBackground.color = nameBackground.color = defaultColor;
                formatNameText.text = nameText.text = name;
                dialogs = null;
                isRunning = false;
                yield break;
            }
        }

        // Initiates the dialogue display process
        shouldTransition = false;
        Invoke("EnableTransition", transitionDelay);
        StartCoroutine(Exit(currentIndex));
    }

    // Dialogue display
    private IEnumerator Exit(int currentIndex)
    {
        // Obtains current dialog string and creates a status instance
        string currentDialog = dialogs[currentIndex];
        Status status = new Status();

        StartCoroutine(Display(currentDialog, status));

        // Idles until the dialogue is complete OR the transition key is pressed (given transitioning is allowed)
        while (actualDialog != currentDialog && (!Input.GetKeyDown(Settings.dialogueInteractKey) || !shouldTransition))
        {
            yield return null;
        }

        dialogText.text = currentDialog; // If the dialogue was skipped, the full dialogue is revealed
        status.isComplete = true; // If the dialogue was skipped, this boolean alteration ensures the current 'Display' coroutine is terminated as soon as possible
        shouldTransition = false;
        Invoke("EnableTransition", transitionDelay);
        StartCoroutine(Enter(currentIndex + 1)); // Moved onto the next dialog, if any
    }

    // Dialogue display
    private IEnumerator Display(string currentDialog, Status status)
    {
        int characterIndex = 0; // Current character in string

        actualDialog = "";
        dialogText.text = currentDialog;

        // Repeatedly adds a character until the dialogue is complete OR the dialogue was skipped (checked through 'status')
        while (characterIndex < currentDialog.Length && !status.isComplete)
        {
            actualDialog += currentDialog[characterIndex];
            dialogText.text = currentDialog.Substring(0, characterIndex + 1) + "<color=#00000000>" + currentDialog.Substring(characterIndex + 1) + "</color>";
            characterIndex++;
            yield return new WaitForSeconds(dialogSpeed);
        }
    }

    // During 'Enter' and 'Exit' transitions, this method is invoked with a timer to enforce the minimum transition delay
    private void EnableTransition()
    {
        shouldTransition = true;
    }

    // Creates the dialog outline based on input border thickness
    private void GenerateBorders()
    {
        GameObject dialogBorder = new GameObject("Dialog Border"); // The root object
        RectTransform dialogBorderTransform = dialogBorder.AddComponent<RectTransform>();
        Vector2 dialogSize = dialogBackground.GetComponent<RectTransform>().sizeDelta;

        dialogBorder.transform.SetParent(root.transform);
        dialogBorderTransform.anchoredPosition = Vector2.zero;

        // Collects all required variables
        GameObject dialogBorderLeft = new GameObject("Left", typeof(CanvasRenderer));
        GameObject dialogBorderRight = new GameObject("Right", typeof(CanvasRenderer));
        GameObject dialogBorderDown = new GameObject("Down", typeof(CanvasRenderer));
        GameObject dialogBorderUp = new GameObject("Up", typeof(CanvasRenderer));
        RectTransform dialogBorderLeftTransform = dialogBorderLeft.AddComponent<RectTransform>();
        RectTransform dialogBorderRightTransform = dialogBorderRight.AddComponent<RectTransform>();
        RectTransform dialogBorderDownTransform = dialogBorderDown.AddComponent<RectTransform>();
        RectTransform dialogBorderUpTransform = dialogBorderUp.AddComponent<RectTransform>();
        RectTransform nameBackgroundTransform = nameBackground.GetComponent<RectTransform>();
        RectTransform nameBoxTransform = nameText.transform.parent.GetComponent<RectTransform>();
        RectTransform nameTextTransform = nameText.GetComponent<RectTransform>();
        Image dialogBorderLeftImage = dialogBorderLeft.AddComponent<Image>();
        Image dialogBorderRightImage = dialogBorderRight.AddComponent<Image>();
        Image dialogBorderDownImage = dialogBorderDown.AddComponent<Image>();
        Image dialogBorderUpImage = dialogBorderUp.AddComponent<Image>();

        // Dialog border establishment
        dialogBorderLeft.transform.SetParent(dialogBorder.transform);
        dialogBorderRight.transform.SetParent(dialogBorder.transform);
        dialogBorderDown.transform.SetParent(dialogBorder.transform);
        dialogBorderUp.transform.SetParent(dialogBorder.transform);
        dialogBorderLeftImage.color = dialogBorderRightImage.color = dialogBorderDownImage.color = dialogBorderUpImage.color = borderColor;

        Vector2 horizontalSize = new Vector2(dialogSize.x, borderThickness);
        Vector2 verticalSize = new Vector2(borderThickness, dialogSize.y + 2 * borderThickness);

        dialogBorderLeftTransform.sizeDelta = verticalSize + new Vector2(0, borderThickness /*+ nameTextTransform.sizeDelta.y*/ + nameBackgroundTransform.rect.size.y);
        dialogBorderRightTransform.sizeDelta = verticalSize;
        dialogBorderDownTransform.sizeDelta = dialogBorderUpTransform.sizeDelta = horizontalSize;
        dialogBorderLeftTransform.anchoredPosition = new Vector2((-borderThickness - dialogSize.x) / 2f, (borderThickness /*+ nameTextTransform.sizeDelta.y*/ + nameBackgroundTransform.rect.size.y) / 2f);
        dialogBorderRightTransform.anchoredPosition = new Vector2((borderThickness + dialogSize.x) / 2f, 0);
        dialogBorderDownTransform.anchoredPosition = new Vector2(0, (-borderThickness - dialogSize.y) / 2f);
        dialogBorderUpTransform.anchoredPosition = new Vector2(0, (borderThickness + dialogSize.y) / 2f);

        // Name border establishment
        nameBoxTransform.anchoredPosition = new Vector2(-nameBackgroundTransform.offsetMin.x, borderThickness + nameTextTransform.sizeDelta.y - nameBackgroundTransform.offsetMin.y);

        Vector2 nameBorderRightSize = new Vector2(borderThickness, borderThickness /*+ nameTextTransform.sizeDelta.y*/ + nameBackgroundTransform.rect.size.y);
        Vector2 nameBorderUpSize = new Vector2(0, borderThickness);

        GameObject nameBorderRight = new GameObject("Right", typeof(CanvasRenderer));
        GameObject nameBorderUp = new GameObject("Up", typeof(CanvasRenderer));
        RectTransform nameBorderRightTransform = nameBorderRight.AddComponent<RectTransform>();
        RectTransform nameBorderUpTransform = nameBorderUp.AddComponent<RectTransform>();
        Image nameBorderRightImage = nameBorderRight.AddComponent<Image>();
        Image nameBorderUpImage = nameBorderUp.AddComponent<Image>();

        nameBorderRight.transform.SetParent(nameBackground.transform);
        nameBorderUp.transform.SetParent(nameBackground.transform);
        nameBorderRightImage.color = nameBorderUpImage.color = borderColor;
        nameBorderRightTransform.pivot = new Vector2(0, 0);
        nameBorderUpTransform.pivot = new Vector2(0.5f, 0);
        nameBorderRightTransform.anchorMin = nameBorderRightTransform.anchorMax = new Vector2(1, 0);
        nameBorderUpTransform.anchorMin = new Vector2(0, 1);
        nameBorderUpTransform.anchorMax = new Vector2(1, 1);
        nameBorderRightTransform.sizeDelta = nameBorderRightSize;
        nameBorderUpTransform.sizeDelta = nameBorderUpSize;
        nameBorderRightTransform.anchoredPosition = nameBorderUpTransform.anchoredPosition = Vector2.zero;
    }

    // Instance getter method
    public static DialogManager Instance
    {
        get
        {
            if (instance == null)
            {
                throw new Exception("DialogManager instance not yet established.");
            }

            return instance;
        }
    }
}