using Godot;
using FSharpScripts;

public partial class World2 : Button
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var button = new SceneChangeButtonFS.SceneChanger("World2", "res://Selection Screens/LevelSelect2.tscn");
        button.Ready();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}