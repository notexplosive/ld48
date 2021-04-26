﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LD48.Data
{
    public class LevelDialogue
    {
        public static string[] IntroSequence = new string[] {
            "Approaching 1km depth marker...",
            "Specimen is in position.\nHalting sedative agent.",
            "Signal strength: Strong\nSpecimen condition: Normal\nReady to begin test, waiting for specimen to wake up." };

        public static string[] ForLevels = new string[]
        {
            "Specimen is awake and alert. It appears the specimen has a hightened perception when it prepares to strike.\n(Hold Mouse Button to slow down time)",
            "The specimen attacks by propelling a thin rigid spike from the orifice. We're not yet sure what it's made of but we know it can cut through steel and can extend to more than 200 meters.",
            "The specimen sleeps between meals despite lacking any apparent physiological need to do so. It eats everything in sight and then falls asleep. I imagine if it weren't affixed to an anchor it would roam around eating everything alive.",
            
            // Seaweed
            "The specimen is carnivorous in nature, it seems to find certain plant material agitating. We need to research into this potential weakness.",

            "Since the specimen does not match any known phylum, we've taken it upon ourselves to name this species \"Oculus Leviathan.\" As far as we know, the specimen is the only one of its kind.",
            "My colleagues have described the specimen's appearance as a \"giant eyeball,\" hence the name Oculus Leviathan. However despite the eye-like appearance of the orifice, I think \"giant mouth\" is a better description.",
            "Oculus Leviathan has no known predators. With its rigid spike and hard shell, Oculus Leviathan is a threat to any ecosystem it exists in.",
            
            // Jellyfish
            "Scans show a deep sea creature that agitates the specimen similar to how certain plant life does. This indicates that there may be more weaknesses to discover.",

            // Second to last level
            "We're reaching the depths where our instruments start to get finicky. We still have basic vitals on the specimen but that's about it. The specimen seems content, even under this much water pressure.",
            "Signal strength is very weak... We're going to keep pushing the specimen farther down.",
            "Just a little more...",

            // Finale
            "The specimen has been disconnected from the winch. It has escaped into the Marianas Trench...\n\nBack where it came from."
        };
    }
}