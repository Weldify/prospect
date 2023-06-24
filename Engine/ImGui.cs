using System.Numerics;

namespace Prospect.Engine;

public static class ImGui {
	public static bool Begin( string title ) => ImGuiNET.ImGui.Begin( title );
	public static void End() => ImGuiNET.ImGui.End();

	public static void Text( string text ) => ImGuiNET.ImGui.Text( text );
	public static void TextColored( string text, Vector4 color ) => ImGuiNET.ImGui.TextColored( color, text );
	public static void InputText( string text, ref string input, uint maxLength ) => ImGuiNET.ImGui.InputText( text, ref input, maxLength );
	public static void SeparatorText( string text ) => ImGuiNET.ImGui.SeparatorText( text );

	public static bool Button( string text ) => ImGuiNET.ImGui.Button( text );

	public static void SameLine( float offset, float spacing ) => ImGuiNET.ImGui.SameLine( offset, spacing );

	public static void Separator() => ImGuiNET.ImGui.Separator();
}
