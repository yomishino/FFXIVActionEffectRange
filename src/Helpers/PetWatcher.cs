using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel;
using GeneratedSheets = Lumina.Excel.GeneratedSheets;

namespace ActionEffectRange.Helpers
{
    internal static class PetWatcher
    {
        private static readonly ExcelSheet<GeneratedSheets.Pet>? petSheet
            = DataManager.GetExcelSheet<GeneratedSheets.Pet>();

        public static bool HasPetPresent => BuddyList.PetBuddyPresent;

        public static GameObject? GetPet()
            => BuddyList.PetBuddy?.GameObject;

        public static uint GetPetObjectId()
            => GetPet()?.ObjectId ?? 0;

        public static Vector3 GetPetPosition()
            => GetPet()?.Position ?? new();

        public static float GetPetRotation()
            => GetPet()?.Rotation ?? 0;

        // Pet sheet key id
        public static uint GetPetType()
            => BuddyList.PetBuddy?.DataID ?? 0;

        public static bool IsNamelessPet(uint petType)
            => petType > 0 
            && string.IsNullOrEmpty(petSheet?.GetRow(petType)?.Name.RawString);

        public static bool IsBunshin(uint petType)
            => petType == 19 || petType == 22;

        // Bunshin is named but has no interactable models
        public static bool IsNamelessPetOrBunshin(uint petType)
            => IsNamelessPet(petType) || IsBunshin(petType);

        public static bool IsCurrentPetNameless()
            => IsNamelessPet(GetPetType());

        public static bool IsCurrentPetNamelessOrBunshin()
            => IsNamelessPetOrBunshin(GetPetType());

        // Can't find out how to reliably check from pets so just check the job
        public static bool IsCurrentPetACNPet()
            => !IsCurrentPetNamelessOrBunshin() 
            && ClassJobWatcher.IsCurrentClassJobACNRelated();

        public static bool IsCurrentPetNonACNNamedPet()
            => !IsCurrentPetNamelessOrBunshin()
            && !ClassJobWatcher.IsCurrentClassJobACNRelated();

        public static bool IsCurrentPet(uint objectId)
            => GetPetObjectId() == objectId;

    }
}
