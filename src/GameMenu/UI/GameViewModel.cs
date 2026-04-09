using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Godot;

namespace GameMenu.UI;

public sealed partial class GameViewModel : ViewModel
{
	private Node? _anchorNode;

	[ObservableProperty] public partial Node? GameNode { get; set; }

	protected override Task LoadAsync()
	{
		if (GameNode is null) return Task.CompletedTask;
		_anchorNode = SceneTree?.Root.GetNode("Root/Game");
		_anchorNode?.AddChild(GameNode);

		return Task.CompletedTask;
	}

	protected override Task<bool> TryCloseCoreAsync()
	{
		if (GameNode is null) return base.TryCloseCoreAsync();
		_anchorNode?.RemoveChild(GameNode);
		GameNode.Free();
		GameNode = null;

		return base.TryCloseCoreAsync();
	}
}
