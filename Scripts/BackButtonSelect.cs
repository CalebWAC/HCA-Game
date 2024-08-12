using Godot;
using FSharpScripts;

public partial class BackButtonSelect : Button
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var button = new SceneChangeButtonFS.SceneChanger("BackButtonSelect", "res://Selection Screens/WorldSelect.tscn");
        button.Ready();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}