using UnityEditor;

namespace Yggdrasil.GoogleSpreadsheet
{
    public class EditorProgressHandler : IProgressHandler
    {
        public void Show(string title, string message)
        {
            EditorUtility.DisplayProgressBar(title, message, 0f);
        }

        public void Hide()
        {
            EditorUtility.ClearProgressBar();
        }
    }
}