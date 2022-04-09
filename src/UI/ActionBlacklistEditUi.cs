using ActionEffectRange.Actions;
using ActionEffectRange.Actions.Data.Template;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using System.Numerics;

namespace ActionEffectRange.UI
{

    public class ActionBlacklistEditUI : ActionDataEditUi<BlacklistedActionDataItem>
    {
        protected override Vector2 InitialUiSize => new(400, 400);
        protected override string UiName => "Action Blacklist";

        protected override DataTableModel DataTableViewModel { get; set; } = null!;

        public ActionBlacklistEditUI()
        {
            DataTableViewModel = new(
                "ActionEffectRange_Tbl_ActionBlacklistConfig",
                actionId => ActionData.RemoveFromActionBlacklist(actionId));
        }

        public override void DrawContents()
        {
            DrawIntro();
            DataTableViewModel.DrawTable(
                ActionData.GetCustomisedActionBlacklistCopy());
            DrawActionSearchUi();
            DrawActionInputPreviewUi();
            
        }

        private static void DrawIntro()
        {
            ImGuiExt.MultiTextWrapped(
                "Action in this blacklist will not be drawn.",
                "You can use this blacklist to prevent a particular action from being drawn regardless of other settings.");
            ImGui.NewLine();
        }

        private void DrawActionInputPreviewUi()
        {
            ImGui.PushID("ActionBlacklistInputPreview");
            if (selectedMatchedActionRow != null)
            {
                ImGui.Text($"Add this action to Blacklist?");
                ImGui.Indent();
                ImGui.Text(ActionDataInterfacing.GetActionDescription(
                    selectedMatchedActionRow));
                ImGui.Unindent();
                if (ImGuiExt.IconButton(1, FontAwesomeIcon.Plus, "Add to Blacklist"))
                {
                    ActionData.AddToActionBlacklist(selectedMatchedActionRow.RowId);
                    ClearEditInput();
                }
                ImGui.SameLine();
                if (ImGuiExt.IconButton(2, FontAwesomeIcon.Times, "Clear Input"))
                    ClearEditInput();
            }
            else
                ImGuiComponents.DisabledButton(FontAwesomeIcon.Plus);
            ImGui.NewLine();
            ImGui.PopID();
        }
    }
}
