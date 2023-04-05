using UnityEditor;

namespace Derrixx.BehaviourTrees.Editor
{
	public static class ProjectPrefs
	{
		public static void SetInt(string key, int value) => EditorPrefs.SetInt(GetProjectKey(key), value);
		public static void SetFloat(string key, float value) => EditorPrefs.SetFloat(GetProjectKey(key), value);
		public static void SetBool(string key, bool value) => EditorPrefs.SetBool(GetProjectKey(key), value);
		public static void SetString(string key, string value) => EditorPrefs.SetString(GetProjectKey(key), value);
		
		public static int GetInt(string key) => EditorPrefs.GetInt(GetProjectKey(key));
		public static float GetFloat(string key) => EditorPrefs.GetFloat(GetProjectKey(key));
		public static bool GetBool(string key) => EditorPrefs.GetBool(GetProjectKey(key));
		public static string GetString(string key) => EditorPrefs.GetString(GetProjectKey(key));

		private static string GetProjectKey(string key) => $"{PlayerSettings.companyName}.{PlayerSettings.productName}_{key}";
	}
}