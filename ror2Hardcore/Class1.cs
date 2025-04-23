using BepInEx; //mod loader library
using UnityEngine; //engine library that ror2 uses
using RoR2;//use game's code
using RoR2.Navigation;
using System.Linq;
using System;

//Mod: No healing from items,drones or shrines, only from kills

[BepInPlugin("com.yourname.hardcore", "Hardcore mode", "1,0,0")]//mod unique id, mod name, version number

public class ror2Hardcore : BaseUnityPlugin {

    public void Awake() {//runs when mod starts (when game loads)
        SceneDirector.onGenerateInteractableCardSelection += RemoveHealingShrine;

    }

    private void RemoveHealingShrine(SceneDirector director, DirectorCardCategorySelection dccs) {
        if (dccs == null) {
            return;
        }
        for (int i = 0; i < dccs.categories.Length; i++){
            var category = dccs.categories[i];
           
            category.cards = category.cards.
                Where(card => !card.spawnCard.name.Equals("iscShrineHealing", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            dccs.categories[i] = category; //Apply the updated category back
        }



        Logger.LogInfo("Shrine of the Woods removed from spawn pool.");//Debugger
    }


}

