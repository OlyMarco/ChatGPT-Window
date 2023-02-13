using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class PostData
{
    public string prompt;
    public int max_tokens;
    public string model;
    public float temperature;
    public int top_p;
    public float frequency_penalty;
    public float presence_penalty;
    public string stop;
}

[System.Serializable]
public class TextCallback
{
    public List<TextSample> choices;

    [System.Serializable]
    public class TextSample
    {
        public string text;
    }
}

public class ChatGPTAPI : MonoBehaviour
{
    //public static ChatGPTAPI singleton;

    List<string> Component = new List<string> { "Rigidbody", "CharacterController", "SkinnedMeshRenderer" };
    
    [HideInInspector]
    public string getText = "";
    string URL = "https://api.forchange.cn/";
    //string API_KEY = "";
    string form = "(Answer with C# code)Write a Unity script (including \"using\") with comments to achieve this thing: ";

    //void Start()
    //{
    //    if (!singleton) singleton = this;
    //    else if (singleton != this) Destroy(gameObject);
    //}

    public void NewScript(string scriptName, string codeText)
    {
        foreach (var com in Component) 
            if(codeText.IndexOf(com) != -1)
            codeText = codeText.Insert(codeText.IndexOf(string.Format("public class {0}", scriptName)), string.Format("[RequireComponent(typeof({0}))]\n", com));
        Write(string.Format(@"{0}\Assets\Scripts\{1}.cs", Directory.GetCurrentDirectory(), scriptName), codeText);
    }

    public void ToGameObject(string scriptName, GameObject gameObject)
    {
        Type Script_Name = Type.GetType(scriptName);
        gameObject.AddComponent(Script_Name);
    }

    void Write(string pos, string text)
    {
        using FileStream fs = new FileStream(pos, FileMode.Create);
        using StreamWriter wr = new StreamWriter(fs);
        wr.WriteLine(text);
        wr.Close();
    }

    public void POST(string postText)
    {
        getText = "";
        StartCoroutine(Post(form + postText));
    }

    //IEnumerator Post(string postText)
    //{
    //    PostData _post = new PostData
    //    {
    //        prompt = postText,
    //        max_tokens = 2048,
    //        model = "text-davinci-003",
    //        temperature = 1.0f,
    //        top_p = 1,
    //        frequency_penalty = 0.0f,
    //        presence_penalty = 0.6f,
    //        stop = "[\"\n\"]"
    //    };

    //    using UnityWebRequest uwr = new UnityWebRequest("https://api.openai.com/v1/completions", "POST");

    //    string _jsonText = JsonUtility.ToJson(_post);
    //    byte[] data = System.Text.Encoding.UTF8.GetBytes(_jsonText);
    //    uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);
    //    uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

    //    uwr.SetRequestHeader("Content-Type", "application/json");
    //    uwr.SetRequestHeader("Authorization", string.Format("Bearer {0}", API_KEY));

    //    yield return uwr.SendWebRequest();

    //    if (uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.ConnectionError)
    //        Debug.Log(uwr.error);
    //    else
    //    {
    //        string _msg = uwr.downloadHandler.text;
    //        TextCallback _textback = JsonUtility.FromJson<TextCallback>(_msg);
    //        getText = _textback.choices[0].text.Substring(2, _textback.choices[0].text.Length - 2);
    //        if (getText.IndexOf("using UnityEngine;") == -1) getText = getText.Insert(0, "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class noname : MonoBehaviour{\n") + "}";
    //    }
    //}

    IEnumerator Post(string postText)
    {
        PostData _post = new PostData
        {
            prompt = postText
        };

        //PostData _post = new PostData
        //{
        //    prompt = postText,
        //    max_tokens = 2048,
        //    temperature = 1.0f,
        //    top_p = 1,
        //    frequency_penalty = 0.0f,
        //    presence_penalty = 0.6f,
        //};

        using UnityWebRequest uwr = new UnityWebRequest(URL, "POST");

        string _jsonText = JsonUtility.ToJson(_post);
        byte[] data = System.Text.Encoding.UTF8.GetBytes(_jsonText);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(uwr.error);
        else
        {
            string _msg = uwr.downloadHandler.text;
            TextCallback _textback = JsonUtility.FromJson<TextCallback>(_msg);
            getText = _textback.choices[0].text.Substring(2, _textback.choices[0].text.Length - 2);
            if (getText.IndexOf("using UnityEngine;") == -1) getText = getText.Insert(0, "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class noname : MonoBehaviour{\n") + "}";
        }
    }
}