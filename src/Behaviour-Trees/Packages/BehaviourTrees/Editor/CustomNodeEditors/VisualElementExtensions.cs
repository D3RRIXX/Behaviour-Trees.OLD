using UnityEngine.UIElements;

public static class VisualElementExtensions
{
	public static void SetActive(this VisualElement ve, bool value)
	{
		ve.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
	}
}