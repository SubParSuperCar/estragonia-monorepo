using Avalonia.Data.Converters;

namespace GameMenu.UI.Converters;

public sealed class BooleanToOnOffConverter() : FuncValueConverter<bool, string>(value => value ? "On" : "Off");
