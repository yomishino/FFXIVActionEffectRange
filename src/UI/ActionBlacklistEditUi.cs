using ActionEffectRange.Actions;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ActionEffectRange.UI
{

    public static class ActionBlacklistEditUI
    {
        private static bool isUiOpen;
        private static bool shouldShowActionSearchMatches;
        private static string actionBlacklistSearchInput = string.Empty;
        private static Vector2 actionSearchMatchesDisplayRectMin;
        private static Vector2 actionSearchMatchesDisplayRectMax;
        private static List<Lumina.Excel.GeneratedSheets.Action>? actionBlacklistMatchedActions = null;
        private static Lumina.Excel.GeneratedSheets.Action? selectedActionBlacklistActionRow = null;

        
        public static void Draw()
        {
            if (!isUiOpen) return;

            ImGui.SetNextWindowSize(
                ImGuiHelpers.ScaledVector2(400, 400), ImGuiCond.FirstUseEver);
            ImGui.Begin("ActionEffectRange: Configuration - Action Blacklist", 
                ImGuiWindowFlags.NoCollapse);
            
            DrawIntro();
            DrawAddedData();
            DrawActionSearchUi();
            DrawActionInputPreviewUi();

            if (ImGui.Button("Close"))
                CloseUI();

            ImGui.End();
        }

        public static void OpenUI() => isUiOpen = true;

        public static void CloseUI() 
        {
            ClearActionBlacklistInput();
            isUiOpen = false; 
        }

        private static void DrawIntro()
        {
            ImGuiExt.MultiTextWrapped(
                "Action in this blacklist will not be drawn.",
                "You can use this blacklist to prevent a particular action from being drawn regardless of other settings.");
            ImGui.NewLine();
        }

        private static void DrawAddedData()
        {
            if (ImGui.BeginTable("ActionEffectRange_Tbl_ActionBlacklistConfig", 
                4, ImGuiTableFlags.BordersH | ImGuiTableFlags.SortMulti))
            {
                ImGui.TableSetupColumn("Action ID", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Action", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("ClassJob", ImGuiTableColumnFlags.DefaultSort | ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("##Remove", ImGuiTableColumnFlags.NoSort | ImGuiTableColumnFlags.WidthFixed,
                    UiBuilder.IconFont.FontSize * ImGuiHelpers.GlobalScale * 2);
                ImGui.TableHeadersRow();

                foreach (var actionId in ActionData.GetCustomisedActionBlacklistCopy())
                {
                    var excelRow = ActionData.GetActionExcelRow(actionId);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.Text(actionId.ToString());
                    ImGui.TableNextColumn();
                    ImGui.Text(excelRow?.Name ?? string.Empty);
                    ImGui.TableNextColumn();
                    ImGui.Text(excelRow?.ClassJob.Value?.Name ?? string.Empty);
                    ImGui.TableNextColumn();
                    if (ImGuiExt.IconButton((int)actionId, FontAwesomeIcon.Minus, "Unblacklist"))
                    {
                        ActionData.RemoveFromActionBlacklist(actionId);
                    }
                }
                ImGui.TableNextRow();   // dummy
                ImGui.EndTable();
            }
            ImGui.NewLine();
        }

        private static void DrawActionSearchUi()
        {
            ImGui.PushID("ActionBlacklistActionSearch");

            ImGui.Text("Search an action to add to the Blacklist");
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            var input = actionBlacklistSearchInput;
            ImGui.InputTextWithHint("##actionBlacklistSearchInput",
                "Enter Action ID or Action name ...", ref input, 64);

            // Check whether should show matching results
            if (ImGui.IsItemActivated() && !string.IsNullOrWhiteSpace(input))
                shouldShowActionSearchMatches = true;
            if (ImGui.IsAnyMouseDown() && !ImGui.IsAnyItemHovered())
                if (ImGui.IsAnyMouseDown() && !ImGui.IsMouseHoveringRect(
                    actionSearchMatchesDisplayRectMin, actionSearchMatchesDisplayRectMax))
                    shouldShowActionSearchMatches = false;

            // Update input cache and search if input changed
            if (actionBlacklistSearchInput != input)
            {
                actionBlacklistSearchInput = input;
                actionBlacklistMatchedActions 
                    = string.IsNullOrWhiteSpace(input) ? null
                    : ActionDataInterfacing.GetAllPartialMatchActionExcelRows(
                        actionBlacklistSearchInput, true, int.MaxValue, true,
                        a => a != null && ActionData.IsPlayerCombatAction(a))?
                    .ToList();
                shouldShowActionSearchMatches 
                    = actionBlacklistMatchedActions?.Any() ?? false;
            }

            // Show matching results
            if (shouldShowActionSearchMatches)
            {
                actionSearchMatchesDisplayRectMin = ImGui.GetCursorScreenPos();
                var displayRectSize = new Vector2(
                    ImGui.GetContentRegionAvail().X, ImGui.GetTextLineHeight() * 10);
                actionSearchMatchesDisplayRectMax 
                    = actionSearchMatchesDisplayRectMin + displayRectSize;
                if (ImGui.BeginChildFrame(
                    1, displayRectSize, ImGuiWindowFlags.NoFocusOnAppearing))
                {
                    if (actionBlacklistMatchedActions != null)
                    {
                        foreach (var row in actionBlacklistMatchedActions)
                        {
                            if (ImGui.Selectable(
                                ActionDataInterfacing.GetActionDescription(row)))
                            {
                                selectedActionBlacklistActionRow = row;
                                shouldShowActionSearchMatches = false;
                            }
                        }
                    }
                    ImGui.EndChildFrame();
                }
            }
            ImGui.PopID();
            ImGui.NewLine();
        }

        private static void DrawActionInputPreviewUi()
        {
            ImGui.PushID("ActionBlacklistInputPreview");
            if (selectedActionBlacklistActionRow != null)
            {
                ImGui.Text($"Add this action to Blacklist?");
                ImGui.Indent();
                ImGui.Text(ActionDataInterfacing.GetActionDescription(
                    selectedActionBlacklistActionRow));
                ImGui.Unindent();
                if (ImGuiExt.IconButton(1, FontAwesomeIcon.Plus, "Add to Blacklist"))
                {
                    ActionData.AddToActionBlacklist(selectedActionBlacklistActionRow.RowId);
                    ClearActionBlacklistInput();
                }
                ImGui.SameLine();
                if (ImGuiExt.IconButton(2, FontAwesomeIcon.Times, "Clear Input"))
                    ClearActionBlacklistInput();
            }
            else
                ImGuiComponents.DisabledButton(FontAwesomeIcon.Plus);
            ImGui.NewLine();
            ImGui.PopID();
        }

        private static void ClearActionBlacklistInput()
        {
            actionBlacklistSearchInput = string.Empty;
            shouldShowActionSearchMatches = false;
            actionBlacklistMatchedActions = null;
            selectedActionBlacklistActionRow = null;
        }
    }
}
