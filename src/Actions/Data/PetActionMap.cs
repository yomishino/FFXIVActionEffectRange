using System.Collections.Generic;

namespace ActionEffectRange.Actions.Data
{
    public static class PetActionMap
    {
        public static readonly (uint, uint[])[] MapArray = new (uint, uint[])[]
        {
            (25799, new uint[]{ 25841 }),     // radiant aegis
            (25801, new uint[]{ 25842 }),     // Searing light


            //(25802, new uint[]{}),  // summon ruby 
            //(25803, new uint[]{}),  // summon topaz
            //(25804, new uint[]{}),  // summon emerald 
            (25805, new uint[]{ 25852}),   // summon ifrit -> inferno
            (25838, new uint[]{ 25852}), // summon ifrit ii -> inferno
            (25806, new uint[]{ 25853 }), // summon titan -> earthen fury
            (25839, new uint[]{ 25853 }),  // summon titan ii -> earthen fury
            (25807, new uint[]{ 25854 }),   // summon garuda -> aerial blast
            (25840, new uint[]{ 25854 }), // summon garuda ii -> aerial blast
            
            (25831, new uint[]{ 16517 }),   // summon phoenix -> everlasting flight

            (7429, new uint[]{ 7449 }), // enkindle bahamut -> akh morn
            (16516, new uint[]{ 16518 }),   // enkindle phoenix -> revelation
            
            (16537, new uint[]{ 803, 16550 }),       // whispering dawn -> ~ / angel's whisper
            (16538, new uint[]{ 805, 16551 }),       // fey illumination -> ~ / seraphic illumination
            (7437, new uint[]{ 7438 }),       // aetherpact -> fey union
            (16543, new uint[]{ 16544 }),     // fey blessing
            (16546, new uint[]{ 16547 }),     // consolation
        };

        public static Dictionary<uint, HashSet<uint>> AsDictionary()
        {
            var dict = new Dictionary<uint, HashSet<uint>>();
            foreach (var (action, petActions) in MapArray)
                dict.TryAdd(action, new(petActions));
            return dict;
        }
    }
}
