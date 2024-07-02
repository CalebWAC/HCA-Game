namespace FSharpScripts

open Godot
open GlobalFunctions

module HookFS =
    
    let onInputEvent camera (event : InputEvent) clickPos =
        try
            let mouseClick = event :?> InputEventMouseButton
            if mouseClick.ButtonIndex = MouseButton.Left && mouseClick.Pressed then
                GD.Print "Clicked"
        with | _ -> GD.Print "Downcast failed"
    
    let ready () =
        let self = getRoot().GetNode<Node3D>("WorldGenerator").GetNode<Node3D>("Hook")
        self.GetNode<Area3D>("Area3D").add_InputEvent (fun camera event clickPos _ _ ->
            onInputEvent camera event clickPos)