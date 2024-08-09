using System.Linq;
using Godot;
using FSharpScripts;
public partial class Confetti : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		switch (Name.ToString()[Name.ToString().Length - 1])
		{
			case '1':
				if (WorldFS.completedLevelsW1.All(level => level)) Visible = true;
				else Visible = false;
				break;
			case '2':
				if (WorldFS.completedLevelsW2.All(level => level)) Visible = true;
				else Visible = false;
				break;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
