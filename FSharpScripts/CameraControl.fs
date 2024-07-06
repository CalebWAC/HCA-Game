namespace FSharpScripts

open Godot
open GlobalFunctions

module CameraControlFS =
    let mutable camera = Unchecked.defaultof<Node3D>
    let sensitivity = 0.3f
    let mutable cameraAngleV = 0f
    let mutable clicked = false
    
    let ready () =
        camera <- getRoot().GetNode<Node3D>("Camera")
        
    let process delta =
        ()
        
        
        
    let input (event : InputEvent) =
        match event with
        | :? InputEventMouseButton ->
            let mouseEvent = event :?> InputEventMouseButton
            if mouseEvent.ButtonIndex = MouseButton.Left then
                if not clicked && mouseEvent.Pressed then clicked <- true
                if clicked && not mouseEvent.Pressed then clicked <- false
        | :? InputEventMouseMotion when clicked ->
            let mouseEvent = event :?> InputEventMouseMotion
            camera.GlobalRotate(Vector3.Up, -mouseEvent.Relative.X * sensitivity |> degToRad)
        | _ -> ()