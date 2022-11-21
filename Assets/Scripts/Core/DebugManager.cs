using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    /// <summary>
    /// Manager for debugging game
    /// </summary>
    public class DebugManager : MonoBehaviour
    {
        private static DebugManager instance;

        public GameObject debugWindow;
        public TextMeshProUGUI debugText;

        private StringBuilder stringBuilder;
        private Dictionary<string, string> debugLines;

        /// <summary>
        /// Show debug window
        /// </summary>
        public static void ShowDebugWindow()
        {
            instance.debugWindow.SetActive(true);
        }

        /// <summary>
        /// Hide debug window
        /// </summary>
        public static void HideDebugWindow()
        {
            instance.debugWindow.SetActive(false);
        }

        /// <summary>
        /// Add a line of debug text
        /// If a line key exists, update it
        /// </summary>
        public static void AddLineDebugText(string text, string key)
        {
            if (instance.debugLines.ContainsKey(key))
                instance.debugLines[key] = text;
            else
                instance.debugLines.Add(key, text);

            instance.UpdateDebugText();
        }

        /// <summary>
        /// Remove a line of debug text
        /// </summary>
        public static void RemoveLineDebugText(string key)
        {
            instance.debugLines.Remove(key);
            instance.UpdateDebugText();
        }

        /// <summary>
        /// Clear debug text
        /// </summary>
        public static void ClearDebugText()
        {
            instance.debugLines.Clear();
            instance.UpdateDebugText();
        }

        private void UpdateDebugText()
        {
            stringBuilder.Clear();

            foreach (var line in instance.debugLines)
                stringBuilder.Append(line.Value + "\n");

            instance.debugText.text = stringBuilder.ToString();
        }

        private void Awake()
        {
            instance = this;

            stringBuilder = new StringBuilder();
            debugLines = new Dictionary<string, string>();
        }
    }
}