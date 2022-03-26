using System.Collections.Generic;
using System.Collections.Immutable;

namespace ActionEffectRange.Actions.Data.Predefined
{
    public static class PetActionMap
    {
        public static readonly ImmutableDictionary<uint, ImmutableHashSet<uint>> Dictionary = new KeyValuePair<uint, ImmutableHashSet<uint>>[]
        {
            new(25799, new uint[]{ 25841 }.ToImmutableHashSet()),     // radiant aegis
            new(25801, new uint[]{ 25842 }.ToImmutableHashSet()),     // Searing light

            //new(25802, new uint[]{}.ToImmutableHashSet()),  // summon ruby 
            //new(25803, new uint[]{}.ToImmutableHashSet()),  // summon topaz
            //new(25804, new uint[]{}.ToImmutableHashSet()),  // summon emerald 
            new(25805, new uint[]{ 25852 }.ToImmutableHashSet()),   // summon ifrit -> inferno
            new(25838, new uint[]{ 25852 }.ToImmutableHashSet()), // summon ifrit ii -> inferno
            new(25806, new uint[]{ 25853 }.ToImmutableHashSet()), // summon titan -> earthen fury
            new(25839, new uint[]{ 25853 }.ToImmutableHashSet()),  // summon titan ii -> earthen fury
            new(25807, new uint[]{ 25854 }.ToImmutableHashSet()),   // summon garuda -> aerial blast
            new(25840, new uint[]{ 25854 }.ToImmutableHashSet()), // summon garuda ii -> aerial blast
            
            new(25831, new uint[]{ 16517 }.ToImmutableHashSet()),   // summon phoenix -> everlasting flight

            new(7429, new uint[]{ 7449 }.ToImmutableHashSet()), // enkindle bahamut -> akh morn
            new(16516, new uint[]{ 16518 }.ToImmutableHashSet()),   // enkindle phoenix -> revelation
            
            new(16537, new uint[]{ 803, 16550 }.ToImmutableHashSet()),       // whispering dawn -> ~ / angel's whisper
            new(16538, new uint[]{ 805, 16551 }.ToImmutableHashSet()),       // fey illumination -> ~ / seraphic illumination
            new(7437, new uint[]{ 7438 }.ToImmutableHashSet()),       // aetherpact -> fey union
            new(16543, new uint[]{ 16544 }.ToImmutableHashSet()),     // fey blessing
            new(16546, new uint[]{ 16547 }.ToImmutableHashSet()),     // consolation

        }.ToImmutableDictionary();
    }
}
