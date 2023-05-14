using System;
using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Node = Derrixx.BehaviourTrees.Runtime.Nodes.Node;

namespace Derrixx.BehaviourTrees.Editor.ViewScripts
{
	public class NodeView : UnityEditor.Experimental.GraphView.Node
	{
		private readonly Label _executionOrderLabel;
		private readonly Label _description;

		public Action<NodeView> OnNodeSelected;

		public NodeView(Node node) : base(GetUxmlPath())
		{
			Node = node;
			title = node.name;
			viewDataKey = node.Guid;
			
			SetupCapabilities();

			_executionOrderLabel = this.Q<Label>("execution-order");
			_executionOrderLabel.bindingPath = "executionOrder";
			
			_description = this.Q<Label>("description");

			SetupClass();
			Update();
			
			this.Bind(new SerializedObject(node));
		}
		
		public StackNodeView Stack { get; set; }
		public Node Node { get; }

		public override string ToString() => $"NodeView ({Node.name})";

		private void SetupCapabilities()
		{
			Capabilities capabilitiesToRemove = Node switch
			{
				ActionNode or CompositeNode => Capabilities.Movable,
				RootNode => Capabilities.Deletable | Capabilities.Copiable,
				_ => 0
			};

			capabilities &= ~capabilitiesToRemove;
			capabilities |= Capabilities.Stackable;
		}

		private static string GetUxmlPath()
		{
			var treeAsset = Resources.Load<VisualTreeAsset>("NodeView");
			return AssetDatabase.GetAssetPath(treeAsset);
		}

		public void Update()
		{
			title = Node.name;
			_description.text = Node.GetDescription();
		}

		public void SortChildren()
		{
			if (Node is CompositeNode compositeNode)
				compositeNode.Children.Sort((a, b) => a.Position.x < b.Position.x ? -1 : 1);
		}

		public void UpdateState()
		{
			if (!Application.isPlaying)
				return;

			RemoveFromClassList(StyleClassNames.RUNNING);
			RemoveFromClassList(StyleClassNames.FAILURE);
			RemoveFromClassList(StyleClassNames.SUCCESS);

			try
			{
#pragma warning disable CS8524
				string className = Node.CurrentState switch
#pragma warning restore CS8524
				{
					Node.State.Running when Node.Started => StyleClassNames.RUNNING,
					Node.State.Failure => StyleClassNames.FAILURE,
					Node.State.Success => StyleClassNames.SUCCESS,
				};

				AddToClassList(className);
			}
			catch (InvalidOperationException) { }
		}

		public override void OnSelected()
		{
			base.OnSelected();
			OnNodeSelected?.Invoke(this);
		}

		public void UpdateExecutionOrderLabel(int order)
		{
			_executionOrderLabel.text = order.ToString();
		}

		private void SetupClass()
		{
			string className = Node switch
			{
				ActionNode _ => "action",
				CompositeNode _ => "composite",
				RootNode _ => "root",
				DecoratorNode _ => "decorator",
				_ => null
			};

			AddToClassList(className);
		}
	}
}