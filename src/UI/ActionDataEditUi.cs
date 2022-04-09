using ActionEffectRange.Actions;
using ActionEffectRange.Actions.Data.Template;
using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ExcSheets = Lumina.Excel.GeneratedSheets;

namespace ActionEffectRange.UI
{
    public abstract class ActionDataEditUi<T> where T : IDataItem
    {
        private bool isUiOpen;
        private bool shouldShowActionSearchMatches;
        private string actionSearchInput = string.Empty;
        private Vector2 actionSearchMatchesDisplayRectMin;
        private Vector2 actionSearchMatchesDisplayRectMax;
        protected IEnumerable<ExcSheets.Action>? actionSearchMatchedActions = null;
        protected ExcSheets.Action? selectedMatchedActionRow = null;
        
        protected abstract Vector2 InitialUiSize { get; }
        protected abstract string UiName { get; }
        protected abstract DataTableModel DataTableViewModel { get; set; }
        protected virtual Func<ExcSheets.Action, bool>?
            ActionSearchExtraFilter { get; } = null;

        public void Draw()
        {
            if (!isUiOpen) return;

            ImGui.SetNextWindowSize(
                ImGuiHelpers.ScaledVector2(InitialUiSize.X, InitialUiSize.Y), 
                ImGuiCond.FirstUseEver);
            ImGui.Begin($"ActionEffectRange: Configuration - {UiName}",
                ImGuiWindowFlags.NoCollapse);

            if (!ImGui.IsWindowFocused(ImGuiFocusedFlags.ChildWindows))
                shouldShowActionSearchMatches = false;

            DrawContents();

            if (ImGui.Button("Close"))
                CloseUI();

            ImGui.End();
        }

        public virtual void OpenUI() => isUiOpen = true;

        public virtual void CloseUI()
        {
            ClearEditInput();
            isUiOpen = false;
        }

        public abstract void DrawContents();

        protected void DrawActionSearchUi()
        {
            ImGui.PushID($"{UiName}_EditUiActionSearch");

            ImGui.Text("Search an action to add to the list");
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            var input = actionSearchInput;
            ImGui.InputTextWithHint("##actionSearchInput",
                "Enter Action ID or Action name ...", ref input, 64);

            // Check whether should show matching results
            if (ImGui.IsItemActivated() && !string.IsNullOrWhiteSpace(input))
                shouldShowActionSearchMatches = true;
            if (ImGui.IsAnyMouseDown() && !ImGui.IsAnyItemHovered())
                if (ImGui.IsAnyMouseDown() 
                    && !ImGui.IsMouseHoveringRect(
                        actionSearchMatchesDisplayRectMin, 
                        actionSearchMatchesDisplayRectMax))
                    shouldShowActionSearchMatches = false;

            // Update input cache and search if input changed
            if (actionSearchInput != input)
            {
                actionSearchInput = input;
                actionSearchMatchedActions 
                    = string.IsNullOrWhiteSpace(input) ? null
                    : ActionDataInterfacing.GetAllPartialMatchActionExcelRows(
                        actionSearchInput, true, int.MaxValue, true, ActionSearchExtraFilter)?
                    .ToList();
                shouldShowActionSearchMatches = actionSearchMatchedActions?.Any() ?? false;
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
                    if (actionSearchMatchedActions != null)
                    {
                        foreach (var row in actionSearchMatchedActions)
                        {
                            if (ImGui.Selectable(
                                ActionDataInterfacing.GetActionDescription(row)))
                            {
                                selectedMatchedActionRow = row;
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

        protected void ClearEditInput()
        {
            actionSearchInput = string.Empty;
            shouldShowActionSearchMatches = false;
            actionSearchMatchedActions = null;
            selectedMatchedActionRow = null;
        }


        protected class DataTableModel
        {
            private readonly string label;
            private readonly List<EditDataColumn> editDataColumns = new();

            public int EditDataColumnCount => editDataColumns.Count;
            public int TotalColumnCount => EditDataColumnCount + 4;

            public DataTableModel(string label) => this.label = label;

            public void DrawTable(IEnumerable<T> data)
            {
                if (ImGui.BeginTable(label, TotalColumnCount,
                    ImGuiTableFlags.BordersH | ImGuiTableFlags.SortMulti))
                {
                    DrawTableHeadersRow();
                    DrawTableContentRow(data);
                    ImGui.TableNextRow();   // dummy
                    ImGui.EndTable();
                }
                ImGui.NewLine();
            }

            private void DrawTableHeadersRow()
            {
                ImGui.TableSetupColumn("Action ID", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Action", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("ClassJob",
                    ImGuiTableColumnFlags.DefaultSort | ImGuiTableColumnFlags.WidthFixed);
                foreach (var col in editDataColumns)
                    if (col.hasInitWidth)
                        ImGui.TableSetupColumn(col.headerName, col.flags,
                            col.initWidthOrWeight);
                    else ImGui.TableSetupColumn(col.headerName, col.flags);
                ImGui.TableSetupColumn("##Delete",
                    ImGuiTableColumnFlags.NoSort | ImGuiTableColumnFlags.WidthFixed,
                    UiBuilder.IconFont.FontSize * ImGuiHelpers.GlobalScale * 2);
                ImGui.TableHeadersRow();
            }

            private void DrawTableContentRow(IEnumerable<T> data)
            {
                foreach (var entry in data)
                {
                    var excelRow = ActionData.GetActionExcelRow(entry.ActionId);
                    ImGui.TableNextRow();
                    // Action ID
                    ImGui.TableSetColumnIndex(0);
                    ImGui.Text(entry.ActionId.ToString());
                    // Action
                    ImGui.TableNextColumn();
                    ImGui.Text(excelRow?.Name ?? string.Empty);
                    // ClassJob
                    ImGui.TableNextColumn();
                    ImGui.Text(excelRow?.ClassJob.Value?.Name ?? string.Empty);
                    // Custom fields
                    foreach (var col in editDataColumns)
                    {
                        ImGui.TableNextColumn();
                        ImGui.Text(col.dataGetter(entry));
                    }
                    // Delete
                    ImGui.TableNextColumn();
                    if (ImGuiExt.IconButton(
                        (int)entry.ActionId, FontAwesomeIcon.Minus, "Delete"))
                        ActionData.RemoveFromConeAoEAngleList(entry.ActionId);
                }
            }

            public void AddDataColumn(string name, ImGuiTableColumnFlags flags,
                bool hasInitWidth, float initWidthOrWeight,
                Func<T, string> dataGetter)
            {
                editDataColumns.Add(
                    new(name, flags, hasInitWidth, initWidthOrWeight, dataGetter));
            }

            private class EditDataColumn
            {
                public readonly string headerName = null!;
                public readonly ImGuiTableColumnFlags flags;
                public readonly bool hasInitWidth;
                public readonly float initWidthOrWeight;
                public readonly Func<T, string> dataGetter;

                public EditDataColumn(string name, ImGuiTableColumnFlags flags,
                    bool hasInitWidth, float initWidthOrWeight,
                    Func<T, string> dataGetter)
                {
                    this.headerName = name;
                    this.flags = flags;
                    this.hasInitWidth = hasInitWidth;
                    this.initWidthOrWeight = initWidthOrWeight;
                    this.dataGetter = dataGetter;
                }
            }
        }

    }
}
