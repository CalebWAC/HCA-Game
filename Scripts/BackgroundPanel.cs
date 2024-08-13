using System.Linq;
using Godot;
using FSharpScripts;

public partial class BackgroundPanel : Panel
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (WorldFS.completedLevelsW1.All(level => level))
		{
			GlobalFunctions.getScreenRoot().GetNode<Control>("Panel2").Visible = true;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
