using System;
using Avalonia.OpenGL;
using OpenTK;

namespace AvaloniaStlViewer.OpenTK;

public class AvaloniaTkContext : IBindingsContext
{
    private readonly GlInterface _glInterface;

    public AvaloniaTkContext(GlInterface glInterface)
    {
        _glInterface = glInterface;
    }
    
    public IntPtr GetProcAddress(string procName)
    {
        return _glInterface.GetProcAddress(procName);
    }
}