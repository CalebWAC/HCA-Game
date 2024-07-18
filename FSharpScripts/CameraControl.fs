namespace FSharpScripts

open Godot
open GlobalFunctions

module CameraControlFS =
    let mutable camera = Unchecked.defaultof<Node3D>
    let sensitivity = 0.3f
    let mutable clicked = false
    let mutable cameraAngleV = 0f
    let mutable t = 0f
    
    let ready () =
        camera <- getRoot().GetNode<Node3D>("Camera")
        t <- 0f
        
    let process delta =
        if t < 3f then
            camera.RotateY(delta * 2.1f)
            t <- t + delta
        elif t < 10f then
            if WorldFS.level = 0 then
                getRoot().GetTree().Paused <- true
                getRoot().GetNode<Control>("TutorialIntro").Visible <- true
            elif WorldFS.level = 1 then
                getRoot().GetTree().Paused <- true
                getRoot().GetNode<Control>("TutorialWater").Visible <- true
            elif WorldFS.level = 2 then
                getRoot().GetTree().Paused <- true
                getRoot().GetNode<Control>("TutorialLavaWall").Visible <- true
            elif WorldFS.level = 5 then
                getRoot().GetTree().Paused <- true
                getRoot().GetNode<Control>("TutorialGoalFragment").Visible <- true
            elif WorldFS.level = 6 then
                getRoot().GetTree().Paused <- true
                getRoot().GetNode<Control>("TutorialMovingBlock").Visible <- true
            elif WorldFS.level = 9 then
                getRoot().GetTree().Paused <- true
                getRoot().GetNode<Control>("TutorialCompanionCube").Visible <- true
            
            t <- 100f

    let input (event : InputEvent) =
        if not GoalFS.ended then
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