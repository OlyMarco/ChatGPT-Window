using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChatGPTWindow : EditorWindow
{

#if UNITY_EDITOR

    ChatGPTAPI chatGPT;
    GameObject gameObject;

    string postText = "",
        getText = "",
        scriptName = "";

    bool Posting = false;

    void Awake()
    {
        chatGPT = GameObject.Find("ChatGPT").GetComponent<ChatGPTAPI>();
    }

    void Update()
    {
        if (Posting)
        {
            if(chatGPT.getText.Length >= 3)
            {
                Posting = false;
                getText = chatGPT.getText;
            }
        }
    }

    ChatGPTWindow()
    {
        this.titleContent = new GUIContent("ChatGPT Window");
    }

    [MenuItem("ChatGPT/ChatGPT Window", false, -1)]
    static void ChatGPT()
    {
        EditorWindow.GetWindow(typeof(ChatGPTWindow));
    }

    void OnGUI()
    {
        GUILayout.Label("Time:" + System.DateTime.Now);

        GUILayout.Label("Input to ChatGPT:", GUILayout.MaxWidth(105));
        postText = EditorGUILayout.TextField(postText, GUILayout.MaxHeight(48));

        if (GUILayout.Button("Send") && postText != "")
        {
            chatGPT.POST(postText);
            getText = "Waiting...";

            Posting = true;
        }

        GUILayout.Label("ChatGPT Code:", GUILayout.MaxWidth(95));
        getText = EditorGUILayout.TextArea(getText, GUILayout.MaxHeight(256));

        if (GUILayout.Button("Refresh") && postText != "")
        {
            chatGPT.POST(postText);
            getText = "Waiting...";

            Posting = true;
        }

        scriptName = EditorGUILayout.TextField("Script Name", scriptName);

        gameObject = (GameObject)EditorGUILayout.ObjectField("", gameObject, typeof(GameObject), true);

        if (GUILayout.Button("Create a Script") && scriptName != "") chatGPT.NewScript(scriptName, getText);

        if (GUILayout.Button("Add to GameObject") && scriptName != "") chatGPT.ToGameObject(scriptName, gameObject);
    }

#endif

}