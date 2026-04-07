using System.Collections.Generic;

namespace Template.Main;

public interface IFocussable
{
    public void GrabFocus();
    public void ReleaseFocus();
}

public class FocusStack
{
    private readonly Stack<IFocussable> _stack = new();

    public FocusStack(params IFocussable[] focusItems)
    {
        foreach (var focusItem in focusItems) _stack.Push(focusItem);
    }

    public void Pop()
    {
        var popped = _stack.Pop();
        popped.ReleaseFocus();
        _stack.Peek().GrabFocus();
    }

    public void Push(IFocussable focusItem)
    {
        if (_stack.Count > 0) _stack.Peek().ReleaseFocus();

        _stack.Push(focusItem);
        focusItem.GrabFocus();
    }

    public void Peek()
    {
        _stack.Peek();
    }
}
