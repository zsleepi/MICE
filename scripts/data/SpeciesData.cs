using MICE.scripts.lib;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.data
{
    public static class SpeciesData
    {
        public static List<Species> list = [
            new Species("mouse", "mice", "micefolk", new CoreSkills(0,0,1,1,0,1), new Signature( Color.FromHtml("D35658"),Color.FromHtml("FCEFEA") )),
            new Species("fox", "foxes", "fox", new CoreSkills(0,0,1,0,0,2), new Signature(Color.FromHtml("EFB87C"), Color.FromHtml("E5511B") )),
            new Species("spider", "spiders", "spider", new CoreSkills(0,0,1,2,0,0), new Signature(Color.FromHtml("4F3D99"), Color.FromHtml("FF0C91") )),
        ];


        public static Species GetSpecies(string speciesName) {
            return list.Find( item => item.Name == speciesName );
        }
    }
}
