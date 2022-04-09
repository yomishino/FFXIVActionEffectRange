using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace ActionEffectRange.UI
{
    public static class ImGuiExt
    {
        public static void MultiTextWrapped(params string[] paras)
        {
            foreach (var p in paras) ImGui.TextWrapped(p);
        }

        public static void BulletTextWrapped(string text)
        {
            ImGui.Bullet();
            ImGui.SameLine();
            ImGui.TextWrapped(text);
        }

        public static void BulletTextWrappedWithHelpMarker(string text, string helpText)
        {
            ImGui.Bullet();
            ImGui.SameLine();
            ImGui.TextWrapped(text);
            if (ImGui.IsItemHovered()) ImGui.SetTooltip(helpText);
            ImGui.SameLine();
            ImGuiComponents.HelpMarker(helpText);
        }

        public static bool IconButton(int id, FontAwesomeIcon icon, string? tooltip = null)
        {
            var ret = ImGuiComponents.IconButton(id, icon);
            SetTooltipIfHovered(tooltip);
            return ret;
        }


        public static void CheckboxWithTooltip(string label, ref bool v, string? tooltip)
        {
            ImGui.Checkbox(label, ref v);
            SetTooltipIfHovered(tooltip);
        }

        public static void InputIntWithTooltip(string label, ref int v, 
            int step, int stepFast, int min, int max,
            ImGuiInputTextFlags flags, float itemWidth, string? tooltip)
        {
            ImGui.Text(label);
            SetTooltipIfHovered(tooltip);
            ImGui.SameLine();
            ImGui.SetNextItemWidth(itemWidth);
            ImGui.InputInt("##" + label, ref v, step, stepFast, flags);
            SetTooltipIfHovered(tooltip);
            if (v < min) v = min;
            if (v > max) v = max;
        }

        public static void DragIntWithTooltip(string label, ref int v, int spd, int min, int max, 
            float itemWidth, string? tooltip)
        {
            ImGui.Text(label);
            SetTooltipIfHovered(tooltip);
            ImGui.SameLine();
            ImGui.SetNextItemWidth(itemWidth);
            ImGui.DragInt("##" + label, ref v, spd, min, max);
            SetTooltipIfHovered(tooltip);
        }


        public static void DragFloatWithTooltip(string label, ref float v, float spd, 
            float min, float max, string format, float itemWidth, string? tooltip)
        {
            ImGui.Text(label);
            SetTooltipIfHovered(tooltip);
            ImGui.SameLine();
            ImGui.SetNextItemWidth(itemWidth);
            ImGui.DragFloat("##" + label, ref v, spd, min, max, format);
            SetTooltipIfHovered(tooltip);
        }


        public static void SetTooltipIfHovered(string? tooltip)
        {
            if (tooltip != null && ImGui.IsItemHovered()) 
                ImGui.SetTooltip(tooltip);
        }
    }
}
