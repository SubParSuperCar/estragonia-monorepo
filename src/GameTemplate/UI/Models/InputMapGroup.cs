using System.Collections.Generic;
using Godot;

namespace GameTemplate.UI.Models;

public class InputMapGroup(HashSet<Key>? reservedKeys = null)
{
	public HashSet<Key> ReservedKeys { get; } = reservedKeys ?? [];
	public Dictionary<Key, InputMapItem> KeyMappings { get; } = new();
	public Dictionary<JoyButton, InputMapItem> JoypadMappings { get; } = new();
}
