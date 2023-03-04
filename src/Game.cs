namespace ActionEffectRange
{
    internal class Game
    {
        public static bool IsPlayerLoaded 
            => ClientState.LocalContentId != 0 && ClientState.LocalPlayer != null;

        public static Dalamud.Game.ClientState.Objects.SubKinds.PlayerCharacter?
            LocalPlayer => ClientState.LocalPlayer;

        public static bool IsPvPZone
            => DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.TerritoryType>()?
                .GetRow(ClientState.TerritoryType)?.IsPvpZone ?? false;

        public const uint InvalidGameObjectId
            = Dalamud.Game.ClientState.Objects.Types.GameObject.InvalidGameObjectId;

    }
}
