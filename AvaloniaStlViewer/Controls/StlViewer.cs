using Avalonia.Input;
using AvaloniaStlViewer.OpenTK;
using AvaloniaStlViewer.STL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace AvaloniaStlViewer.Controls;

public class StlViewer : BaseTkOpenGlControl
{
    private int _vertexBufferObject;
    private int _vertexArrayObeject;
    private int _normalBufferObject;

    private Shader.Shader _shader;
    private StlMesh _mesh;

    private readonly Matrix4 _baseModel = Matrix4.Identity * Matrix4.CreateScale(0.1f);
    private Matrix4 _model;
    private Matrix4 _view;
    private Matrix4 _projection;

    private readonly string _vertPath = "Shader/stl.vert";
    private readonly string _fragPath = "Shader/stl.frag";

    private bool _mouseLeftDown;
    private Vector2 _lastMousePos;
    private float _rotationX;
    private float _rotationY;

    public StlViewer()
    {
        PointerPressed += OnPointerPressed;
        PointerReleased += OnPointerReleased;
        PointerMoved += OnPointerMoved;
    }

    private void OnPointerPressed(object? sender, PointerEventArgs e)
    {
        var props = e.GetCurrentPoint(this);
        if (props.Properties.IsLeftButtonPressed)
        {
            _mouseLeftDown = true;
            _lastMousePos = new Vector2((float)props.Position.Y, (float)props.Position.X);
        }
    }

    private void OnPointerReleased(object? sender, PointerEventArgs e)
    {
        var props = e.GetCurrentPoint(this);
        if (props.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
        {
            _mouseLeftDown = false;
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if(!_mouseLeftDown) return;

        var props = e.GetCurrentPoint(this);
        var currentPos = new Vector2((float)props.Position.Y, (float)props.Position.X);
        var delta = currentPos - _lastMousePos;

        _rotationX += delta.X * 0.5f;
        _rotationY += delta.Y * 0.5f;

        // 모델 매트릭스 업데이트 (스케일 유지하면서 회전)
        var rotation = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_rotationY)) *
                       Matrix4.CreateRotationX(MathHelper.DegreesToRadians(_rotationX));
        _model = _baseModel * rotation;

        _lastMousePos = currentPos;
    }
    
    protected override void OnLoad()
    {
        _mesh = StlReader.ReadFile("Resources/xyz.stl");
        
        // Vector3 배열을 float 배열로 변환
        float[] vertices = new float[_mesh.Vertices.Count * 3];
        for (int i = 0; i < _mesh.Vertices.Count; i++)
        {
            vertices[i * 3] = _mesh.Vertices[i].X;
            vertices[i * 3 + 1] = _mesh.Vertices[i].Y;
            vertices[i * 3 + 2] = _mesh.Vertices[i].Z;
        }

        float[] normals = new float[_mesh.Normals.Count * 3];
        for (int i = 0; i < _mesh.Normals.Count; i++)
        {
            normals[i * 3] = _mesh.Normals[i].X;
            normals[i * 3 + 1] = _mesh.Normals[i].Y;
            normals[i * 3 + 2] = _mesh.Normals[i].Z;
        }

        
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        
        // VAO 설정
        _vertexArrayObeject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObeject);
        
        // Vertex 버퍼 생성
        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer,
            vertices.Length * sizeof(float),
            vertices,
            BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        // Normal 버퍼 생성
        _normalBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _normalBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer,
            normals.Length * sizeof(float),
            normals,
            BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(1);
        
        // 쉐이더 생성
        _shader = new Shader.Shader(_vertPath, _fragPath);
        _shader.Use();

        // 기본 뷰 및 프로젝션 매트릭스 설정
        _model = Matrix4.Identity * Matrix4.CreateScale(0.1f);
        _view = Matrix4.CreateTranslation(0.0f, 0.0f, -10.0f);
        _projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(60f),
            (float)(Bounds.Width / Bounds.Height),
            0.1f,
            100.0f);
    }

    protected override void OnUnload()
    {
        _shader.Dispose();
    }

    protected override void OpenTkRender()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.BindVertexArray(_vertexArrayObeject);

        _shader.Use();
        
        _shader.SetMatrix4("model", _model);
        _shader.SetMatrix4("view", _view);
        _shader.SetMatrix4("projection", _projection);
        
        GL.DrawArrays(PrimitiveType.Triangles, 0, _mesh.Vertices.Count);
    }
}