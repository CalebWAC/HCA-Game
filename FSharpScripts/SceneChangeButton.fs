#nowarn "3391"

namespace FSharpScripts

open Godot
open GlobalFunctions

module SceneChangeButtonFS =
    type SceneChanger(name: string, scene: string) = 
        let mutable button = Unchecked.defaultof<Button>
       
        let startScene () = button.GetTree().ChangeSceneToFile(scene) |> ignore
        
        member this.Ready () =
            button <- if name <> "BackButtonLevel" then getScreenRoot().GetNode<Button>(name) else getRoot().GetNode<Control>("Control").GetNode<Button>(name)
            button.add_Pressed startScene
                
            if name[..4] = "Level" && WorldFS.completedLevels[(name[5].ToString() |> int) - 1] = true then
                button.Icon <- ResourceLoader.Load($"res://Assets/{name}Filled.png") :?> Texture2D
            
            // Reset mechanics
            PlayerFS.powerUps <- [||]
            MovingBlockFS.movingBlocks.Clear()