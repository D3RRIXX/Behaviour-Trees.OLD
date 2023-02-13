using UnityEngine.UIElements;

namespace Derrixx.BehaviourTrees.Editor
{
	public class SplitView : TwoPaneSplitView
	{
		public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { }
	}
}