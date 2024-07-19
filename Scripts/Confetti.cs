using System.Linq;
using Godot;
using FSharpScripts;
public partial class Confetti : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (WorldFS.completedLevels.All(level => level))
		{
			Visible = true;
		} else
		{
			Visible = false;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
