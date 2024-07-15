using Godot;
using FSharpScripts;

public partial class UnpauseButton : Button
{
	private UnpauseButtonFS.UnpauseButton button;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		button = new UnpauseButtonFS.UnpauseButton(GetParent().GetParent().Name.ToString());
		button.ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
