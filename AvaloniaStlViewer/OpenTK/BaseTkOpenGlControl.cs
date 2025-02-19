using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Rendering;
using Avalonia.Threading;
using OpenTK.Graphics.OpenGL4;

namespace AvaloniaStlViewer.OpenTK;

public abstract class BaseTkOpenGlControl : OpenGlControlBase, ICustomHitTest
{
    private AvaloniaTkContext _tkContext;

    private GlInterface _glInterface;


    /// <summary>
    /// This is a function that renders every frame.
    /// </summary>
    protected virtual void OpenTkRender()
    {
        
    }
    
    /// <summary>
    /// This is where you should initialize OpenGL. Configure arrays or buffers.
    /// </summary>
    protected virtual void OnLoad()
    {
        
    }

    /// <summary>
    /// When a control is unloaded, it cleans up any data it has, such as a buffer or array.
    /// </summary>
    protected virtual void OnUnload()
    {
        
    }

    protected override void OnOpenGlInit(GlInterface gl)
    {
        _tkContext = new AvaloniaTkContext(gl);
        GL.LoadBindings(_tkContext);
        
        OnLoad();
    }

    protected override void OnOpenGlDeinit(GlInterface gl)
    {
        OnUnload();
    }
    
    protected override void OnOpenGlRender(GlInterface gl, int fb)
    {
        _glInterface = gl;

        PixelSize pixelSize = GetPixelSize();
        
        GL.Viewport(0, 0, pixelSize.Width, pixelSize.Height);

        if (Bounds is { Width: > 0, Height: > 0 })
        {
            OpenTkRender();
        }
        
        Dispatcher.UIThread.Post(RequestNextFrameRendering, DispatcherPriority.Render);
    }

    


    public bool HitTest(Point point) => Bounds.Contains(point);
    public GlInterface GetGlInterface() => _glInterface;
    
    private PixelSize GetPixelSize()
    {
        var scaling = TopLevel.GetTopLevel(this).RenderScaling;
        return new PixelSize(Math.Max(1, (int)(Bounds.Width * scaling)),
            Math.Max(1, (int)(Bounds.Height * scaling)));
    }
    
}