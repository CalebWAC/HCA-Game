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
                
            try                                                  // This will need to be changed
                if name[..4] = "Level" && WorldFS.completedLevelsW1[(name[5..6].ToString() |> int) - 1] = true then
                    button.Icon <- ResourceLoader.Load($"res://Assets/Level Images/{name}Filled.png") :?> Texture2D
            with | _ -> 
                if name[..4] = "Level" && WorldFS.completedLevelsW1[(name[5].ToString() |> int) - 1] = true then
                    button.Icon <- ResourceLoader.Load($"res://Assets/Level Images/{name}Filled.png") :?> Texture2D
            
            // Reset mechanics
            PlayerFS.powerUps <- [||]
            if name <> "BackButtonLevel" then
                WorldGeneratorFS.movingBlocks.Clear()
                WorldGeneratorFS.companionCubes.Clear()
                WorldGeneratorFS.cubeTriggers.Clear()
                WorldGeneratorFS.bridges.Clear()
                TerrainManipulatorFS.destructibleBlocks.Clear()