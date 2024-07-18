namespace FSharpScripts

open Godot
open GlobalFunctions

module GoalFS =
    let mutable position = Vector3(0f, 0f, 0f)
    let mutable change = 0.15f
    let mutable goal = Unchecked.defaultof<Node3D>
    
    let mutable ended = false
    let mutable t = 0f
    let mutable originalPos = Vector3(0f, 0f, 0f)
    let mutable endPos = Vector3(0f, 0f, 0f)
    let mutable midpoint = Vector3(0f, 0f, 0f)
    
    let onAreaEntered () =
        ended <- true
        originalPos <- goal.GlobalPosition
        endPos <-
            let cameraPos = getRoot().GetNode<Node3D>("Camera").GetNode<Node3D>("EndPos")
            cameraPos.GlobalPosition
        midpoint <- (originalPos + endPos) / 2f
    
    let ready () =
        goal <- getRoot().GetNode<Node3D>("WorldGenerator").GetNode<Node3D>("Goal")
        goal.GetNode<Area3D>("Area3D").add_AreaEntered (fun _ -> onAreaEntered())
        position <- goal.Position
        t <- 0f
        ended <- false
        
    let physicsProcess delta =
        if not ended then
            goal.Translate(Vector3(0f, change * delta, 0f))
            
            goal.RotateY(degToRad 45f * delta)
            if goal.Position.Y - position.Y > 0.27f || position.Y - goal.Position.Y > 0f then change <- -change
        else
             t <- t + delta * 0.5f
            
             if t <= 1f then
                 let q1 = originalPos.Lerp(midpoint, t)
                 let q2 = midpoint.Lerp(endPos, t)
                
                 goal.Position <- q1.Lerp(q2, t)
                 
                 // Camera movement
                 let camera = getRoot().GetNode<Node3D>("Camera").GetNode<Node3D>("Camera3D")
                 camera.RotateX(delta * -12f |> degToRad); camera.RotateZ(delta * -12f |> degToRad)
             elif t <= 2.5f then
                 goal.RotateY(degToRad 45f * delta)
             else
                Array.set WorldFS.completedLevels WorldFS.level true
                getRoot().GetTree().ChangeSceneToFile("res://LevelSelect.tscn") |> ignore