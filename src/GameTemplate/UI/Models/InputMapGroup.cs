using System.Collections.Generic;
using Godot;

namespace GameTemplate.UI.Models;

public class InputMapGroup
{
    public InputMapGroup(HashSet<Key>? reservedKeys = null)
    {
        ReservedKeys = reservedKeys ?? new HashSet<Key>();
    }

    public HashSet<Key> ReservedKeys { get; } = new();
    public Dictionary<Key, InputMapItem> KeyMappings { get; } = new();
    public Dictionary<JoyButton, InputMapItem> JoypadMappings { get; } = new();
}
