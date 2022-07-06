using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearnOpenTK.Common;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Pert1
{
    internal class Cubemap
    {
        float[] skyboxVertices = {
            // positions          
            -1.0f,  1.0f, -1.0f,
            -1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

            -1.0f,  1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
             1.0f, -1.0f,  1.0f
        };

        int _vao;
        int _vbo;

        int textureID;
        Shader skyboxShader;

        Matrix4 model;

        public Cubemap(string vertPath, string fragPath)
        {
            skyboxShader = new Shader(vertPath, fragPath);
        }

        public void Load()
        {
            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, skyboxVertices.Length * sizeof(float),
                skyboxVertices, BufferUsageHint.StaticDraw);

            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float,
                false, 3 * sizeof(float), 0);


            textureID = LoadCubeMap();

            skyboxShader.Use();
            skyboxShader.SetInt("skybox", 0);

            model = Matrix4.Identity;

        }

        private int LoadCubeMap()
        {
            string[] skyboxPath =
            {
                "F:/grafkom/Grafkom B/pert1/backup_pert1 - Copy/Pert1/Pert1/skybox/right.png",
                "F:/grafkom/Grafkom B/pert1/backup_pert1 - Copy/Pert1/Pert1/skybox/left.png",
                "F:/grafkom/Grafkom B/pert1/backup_pert1 - Copy/Pert1/Pert1/skybox/top.png",
                "F:/grafkom/Grafkom B/pert1/backup_pert1 - Copy/Pert1/Pert1/skybox/bottom.png",
                "F:/grafkom/Grafkom B/pert1/backup_pert1 - Copy/Pert1/Pert1/skybox/front.png",
                "F:/grafkom/Grafkom B/pert1/backup_pert1 - Copy/Pert1/Pert1/skybox/back.png",
            };
            int textureID;
            GL.GenTextures(1, out textureID);
            GL.BindTexture(TextureTarget.TextureCubeMap, textureID);

            Console.WriteLine("Cubemap: " + textureID);
            for (int i = 0; i < skyboxPath.Length; i++)
            {
                using ( var image = new Bitmap(skyboxPath[i]))
                {
                    Console.WriteLine(skyboxPath[i] + " LOADED");

                    var data = image.LockBits(
                        new Rectangle(0, 0, image.Width, image.Height),
                        ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);


                    GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i,
                        0,
                        PixelInternalFormat.Rgb,
                        1024,
                        1024,
                        0,
                        PixelFormat.Bgra,
                        PixelType.UnsignedByte,
                        data.Scan0);

                }
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            }

            return textureID;
        }

        public void Render(Camera _camera)
        {

            GL.DepthFunc(DepthFunction.Lequal);

            Matrix4 view = _camera.GetViewMatrix().ClearTranslation().Inverted();
            Matrix4 projection = _camera.GetProjectionMatrix();

            skyboxShader.Use();
            //skyboxShader.SetMatrix4("model", model);
            skyboxShader.SetMatrix4("view", view);
            skyboxShader.SetMatrix4("projection", projection);
            skyboxShader.SetInt("skybox", 0);

            GL.BindVertexArray(_vao);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, textureID);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.BindVertexArray(0);

            GL.DepthFunc(DepthFunction.Less);
        }
    }
}
