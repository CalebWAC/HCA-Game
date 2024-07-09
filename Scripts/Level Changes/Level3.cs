using Godot;
using FSharpScripts;

public partial class Level3 : Button
{
    private SceneChangeButtonFS.SceneChanger button;
	
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        button = new SceneChangeButtonFS.SceneChanger("Level3", "res://Levels/Level3.tscn");
        button.Ready();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}