using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib.bodylogic
{
    public class ActorBody
    {
        // Mandatory, every body is made of something.
        public string bodyMaterial;

        // Mandatory. every creature has one and only one
        // grants one single body armor slot. armor is kind and fits all bodies. :)
        // everyone also gets 2 ring slots and 1 necklace slot for balance.
        // comes in a few types. anthro, feral, and taur. snakes, insects, amorphous, and other abnormal body types fall under feral for that species. anthros can only have 2 legs.
        // only anthros and taurs get top underwear and humanoid breasts.
        public string body;

        // the head. featureless creatures like slimes dont have one. some creatures might have multiple. heads can have horns and ears etc
        // each head comes with a head, face, and mouth equip slot.
        public Head[] heads;

        // leg configuration. can be none. increases in pairs and has unique logic for quadrupeds and taurs
        // bipeds can wear pants and boots. anyone with legs can wear a pair of underwear as a treat and a pair of socks for each set of legs. hoofed fellas get a special boots slot for horseshoes
        public Legs[] legs;

        // an arm is any limb capable of holding objects. you can have any number.
        // each arm has an item equip slot
        public Arms[] arms;

        //wings
        public Wings wings;

        // tails, abdomens. you can have up to 9.
        public Tail[] tails;

        // just about any creature can have these, rows of them, even.
        public string crotchboobs;

        // some have them, some dont :3 can be none, null, penis, vagina, or both
        // comes with equip slot according to configuration
        public string genitals;

        // yeah. most people have one. comes with equip slot wink wink
        public string anus;

    }
}
