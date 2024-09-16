using UnityEditor;
using UnityEngine;

public class API_KeyEncryption : EditorWindow
{
    private const float WIDTH = 500f;
    private const float HEIGHT = 150f;

    private string _key = string.Empty;
    private string _result = string.Empty;
    
    [MenuItem("OpenAI/OpenAPI Key")]
    private static void Init()
    {
        var window = (API_KeyEncryption)GetWindow(typeof(API_KeyEncryption));
        window.Show();

        window.titleContent.text = "OpenAI Init";

        window.maxSize = window.minSize = new Vector2(WIDTH, HEIGHT);
    }

    private void OnGUI()
    {
        // TextField ======================================================================
        GUILayout.Space(10);
        _key = EditorGUILayout.TextField("Input Key", _key);
        GUILayout.Space(10);
        
        // Button ======================================================================
        GUI.enabled = !string.IsNullOrEmpty(_key);
        if (GUILayout.Button("Save Key"))
        {
            _result = KeyEncryption.EncodingBase64(_key)
                ? "Key successfully saved and encoded!"
                : "Failed: Key cannot be empty.";
  
            _key = string.Empty;
        }
        GUILayout.Space(30);
        
        // Result ======================================================================
        if (string.IsNullOrEmpty(_result) == false)
        {
            GUILayout.Label(_result, EditorStyles.boldLabel);
        }
    }
}
