using UnityEngine;
using UnityEngine.UI;

public class AutoLocalizeTextUI : MonoBehaviour
{
	[Tooltip("UI Text component to place text.")]
	public Text textField;

	[Tooltip("The page name to reference the text from.")]
	public string sheetTitle;

	[Tooltip("The key to look up.")]
	public string textKey;
}
