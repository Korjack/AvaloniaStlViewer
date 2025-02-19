using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace AvaloniaStlViewer.Shader;

public class Shader : IDisposable
{
    public readonly int Handle;
    private bool _disposedValue = false;
    
    private readonly Dictionary<string, int> _uniformLocations;

    public Shader(string vertexPath, string fragmentPath)
    {
        string vertexShaderSource = File.ReadAllText(vertexPath);
        string fragmentShaderSource = File.ReadAllText(fragmentPath);
        
        // 쉐이더 로드
        var vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        
        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        
        // 쉐이더 컴파일
        GL.CompileShader(vertexShader);

        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(vertexShader);
            Console.WriteLine(infoLog);
        }

        GL.CompileShader(fragmentShader);

        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(fragmentShader);
            Console.WriteLine(infoLog);
        }
        
        // 프로그램으로 연결
        Handle = GL.CreateProgram();
        
        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);
        
        GL.LinkProgram(Handle);
        
        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out success);
        if (success == 0)
        {
            string infoLog = GL.GetProgramInfoLog(Handle);
            Console.WriteLine(infoLog);
        }
        
        // 쉐이더 정리
        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, fragmentShader);
        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);
        
        // Unifom 갯수 확인
        GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out int numberOfUniforms);

        _uniformLocations = new Dictionary<string, int>();
        
        // Uniform 데이터 로드
        for (var i = 0; i < numberOfUniforms; i++)
        {
            // 유니폼 이름
            var key = GL.GetActiveUniform(Handle, i, out _, out _);
            
            // 유니폼 위치
            var location = GL.GetUniformLocation(Handle, key);
            
            _uniformLocations.Add(key, location);
        }
    }

    public void Use()
    {
        GL.UseProgram(Handle);
    }
    
    public int GetAttribLocation(string attribName)
    {
        return GL.GetAttribLocation(Handle, attribName);
    }

    public void SetInt(string name, int value)
    {
        int location = GL.GetUniformLocation(Handle, name);
        
        GL.Uniform1(location, value);
    }

    public void SetMatrix4(string name, Matrix4 data)
    {
        GL.UseProgram(Handle);
        GL.UniformMatrix4(_uniformLocations[name], true, ref data);
    }
    
    public void SetVector3(string name, Vector3 data)
    {
        GL.UseProgram(Handle);
        GL.Uniform3(_uniformLocations[name], data);
    }

    protected virtual void Dipose(bool disposing)
    {
        if (!_disposedValue)
        {
            GL.DeleteProgram(Handle);

            _disposedValue = true;
        }
    }
    
    public void Dispose()
    {
        Dipose(true);
        GC.SuppressFinalize(this);
    }

    ~Shader()
    {
        if (_disposedValue == false)
        {
            Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
        }
    }
}