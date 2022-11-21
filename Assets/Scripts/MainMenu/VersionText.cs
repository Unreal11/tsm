using UnityEngine;
using TMPro;

namespace MainMenu
{
    public class VersionText : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<TextMeshProUGUI>().text = "v" + Application.version;
        }
    }
}
