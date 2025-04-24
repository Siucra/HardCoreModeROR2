using BepInEx; //mod loader library
using UnityEngine; //engine library that ror2 uses
using RoR2;//use game's code
using RoR2.Navigation;
using System.Linq;
using System;
using Console = System.Console;

//Mod: No healing from items,drones or shrines, only from kills

[BepInPlugin("com.yourname.hardcore", "Hardcore mode", "1,0,0")]//mod unique id, mod name, version number

public class ror2Hardcore : BaseUnityPlugin {

    public void Awake() {//runs when mod starts (when game loads)
        SceneDirector.onGenerateInteractableCardSelection += RemoveHealingShrine;

    }

    //SceneDirector -> thing in charge of seeting up level, DirectorCardCategorySelection dccs -> a list of spawnable interactables
    private void RemoveHealingShrine(SceneDirector director, DirectorCardCategorySelection dccs) {
        if (dccs == null) {//if the list is empty, stop the function (error catching)
            return;
        }
        for (int i = 0; i < dccs.categories.Length; i++){//loops through each category 
            var category = dccs.categories[i]; //saves current category to a variable so it can be utilised
           
            category.cards = category.cards. //replace the cards in this category and keep only cards that doesnt have "iscShrineHealing", converts updated list into array
                Where(card => !card.spawnCard.name.Equals("iscShrineHealing", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            dccs.categories[i] = category; //Apply the updated category back
        }



        Console.WriteLine("Shrine of the Woods removed from spawn pool.");//Debugger
    }


}

