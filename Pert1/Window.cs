using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using LearnOpenTK.Common;
using System.Globalization;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;



namespace Pert1
{
    static class Constants
    {
        public const string path = "../../../Shaders/";
        public const string obj = "../../../";

    }
    internal class Window : GameWindow
    {
        private List<Vector3> _pointLightPositions = new List<Vector3>()
    {
            new Vector3(74895.79f, -1472.0786f, 1446.9585f),
            new Vector3(74765.17f,-1472.0786f, 1446.9585f),
            new Vector3(74895.79f, -1439.9519f, 1446.9585f),
            new Vector3(74765.17f,-1439.9519f, 1446.9585f)
        };
        private List<Vector3> point_light_color_difuse = new List<Vector3>()
        {
            new Vector3(1f, 1, 1f),
            new Vector3(1f, 1, 1f),
            new Vector3(1f, 1, 1f),
            new Vector3(1f, 1, 1f)
        };

        Asset3d[] _object3d = new Asset3d[4];
        float second;
        float degr = 0;
        Camera _camera;
        bool _firstMove = true;
        Vector2 _lastPos;
        Vector3 _objecPost = new Vector3(0.0f, 0.0f, 0.0f);
        float _rotationSpeed = 0.08f;
        Asset3d[] cahaya = new Asset3d[2];
        int counter = 0;
        bool camera_run = false;
        bool camera_thirdp = false;
        bool mil_1 = true;
        bool mil_2 = true;
        bool mil_3 = true;
        bool mil_4 = true;
        bool mil_5 = true;
        bool mil_6 = true;
        bool mil_7 = true;
        bool mil_8 = true;
        bool mil_9 = true;

        Cubemap cubemap;
        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }
        public Matrix4 generateArbRotationMatrix(Vector3 axis, Vector3 center, float degree)
        {
            var rads = MathHelper.DegreesToRadians(degree);

            var secretFormula = new float[4, 4] {
                { (float)Math.Cos(rads) + (float)Math.Pow(axis.X, 2) * (1 - (float)Math.Cos(rads)), axis.X* axis.Y * (1 - (float)Math.Cos(rads)) - axis.Z * (float)Math.Sin(rads),    axis.X * axis.Z * (1 - (float)Math.Cos(rads)) + axis.Y * (float)Math.Sin(rads),   0 },
                { axis.Y * axis.X * (1 - (float)Math.Cos(rads)) + axis.Z * (float)Math.Sin(rads),   (float)Math.Cos(rads) + (float)Math.Pow(axis.Y, 2) * (1 - (float)Math.Cos(rads)), axis.Y * axis.Z * (1 - (float)Math.Cos(rads)) - axis.X * (float)Math.Sin(rads),   0 },
                { axis.Z * axis.X * (1 - (float)Math.Cos(rads)) - axis.Y * (float)Math.Sin(rads),   axis.Z * axis.Y * (1 - (float)Math.Cos(rads)) + axis.X * (float)Math.Sin(rads),   (float)Math.Cos(rads) + (float)Math.Pow(axis.Z, 2) * (1 - (float)Math.Cos(rads)), 0 },
                { 0, 0, 0, 1}
            };
            var secretFormulaMatix = new Matrix4
            (
                new Vector4(secretFormula[0, 0], secretFormula[0, 1], secretFormula[0, 2], secretFormula[0, 3]),
                new Vector4(secretFormula[1, 0], secretFormula[1, 1], secretFormula[1, 2], secretFormula[1, 3]),
                new Vector4(secretFormula[2, 0], secretFormula[2, 1], secretFormula[2, 2], secretFormula[2, 3]),
                new Vector4(secretFormula[3, 0], secretFormula[3, 1], secretFormula[3, 2], secretFormula[3, 3])
            );

            return secretFormulaMatix;
        }
        protected override void OnLoad()
        {
            base.OnLoad();
            //ganti background
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            //cahaya kotak keliling
            cahaya[0] = new Asset3d();
            cahaya[0].createBoxVertices2(0f, 0f, 0f, 30f);
            cahaya[0].load(Constants.path + "shader.vert", Constants.path + "shader1.frag", Size.X, Size.Y);
            _pointLightPositions.Add(cahaya[0]._centerPosition);
            point_light_color_difuse.Add(new Vector3(1, 1, 1));

            //read object 1, space station
            _object3d[0] = new Asset3d();
            _object3d[0].readFileOBJ(_object3d[0], Constants.obj + "Space Station V 2001 a space odysee obj/space station V.obj");
            _object3d[0].load(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            _object3d[0].translate(73274.336f, -2116.2864f, 1793.9042f);

            //read object 2, orion spaceship
            _object3d[1] = new Asset3d();
            _object3d[1].readFileOBJ(_object3d[1], Constants.obj + "orion obj/orion 2001 a space odyssey.obj");
            _object3d[1].load(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            _object3d[1].translate(70000,0,-500);
            _pointLightPositions.Add(_object3d[1]._centerPosition - new Vector3(40, 0, 20));
            point_light_color_difuse.Add(new Vector3(1, 1, 1));
            _pointLightPositions.Add(_object3d[1]._centerPosition - new Vector3(100, 50, 25));
            point_light_color_difuse.Add(new Vector3(1, 1, 1));
            _pointLightPositions.Add(_object3d[1]._centerPosition - new Vector3(-50, 50, 25));
            point_light_color_difuse.Add(new Vector3(1, 1, 1));
            _object3d[1].rotatede(_object3d[1]._centerPosition, _object3d[1]._euler[1], 90);
            Console.WriteLine(_object3d[1]._centerPosition);

            //bumi
            _object3d[2] = new Asset3d();
            _object3d[2].readFileOBJ(_object3d[2], Constants.obj + "bulan/bulan.obj");
            _object3d[2].load(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            _object3d[2].scale(500,500,500);
            _object3d[2].translate(72000, -1000, -0);

            //matahari
            _object3d[3] = new Asset3d();
            _object3d[3].readFileOBJ(_object3d[3], Constants.obj + "bulan/bulan.obj");
            _object3d[3].load(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            _object3d[3].scale(500, 500, 500);

            //skybox
            cubemap = new Cubemap(Constants.obj + "skyboxShader.vert", Constants.obj + "skyboxShader.frag");
            cubemap.Load();

            //camera
            _camera = new Camera(new Vector3(70000, 0, -350), Size.X / Size.Y);
            //_camera = new Camera(new Vector3(73420.29f, -2004.4227f, -292.43942f), Size.X / Size.Y);

            CursorGrabbed = true;

            //collision range
            Vector3 _objec0_kiri_atas = new Vector3(_object3d[0]._centerPosition + new Vector3(0,0,0));

        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            second += (float)args.Time;
            Matrix4 temp = Matrix4.Identity;
            //temp = temp * Matrix4.CreateTranslation(0.5f, 0.5f, 0.0f);
            //degr += MathHelper.DegreesToRadians(20f);
            //temp = temp * Matrix4.CreateRotationX(degr);

            //_object3d[0].render(0,_time,temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            cahaya[0].render(0, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            //cahaya[0].rotatede(_object3d[0]._centerPosition, _object3d[0]._euler[1], (float)args.Time * 90);
            _pointLightPositions[4] = cahaya[0]._centerPosition;
            //_object3d[0].setFragVariable(new Vector3(0.1f, 0.5f, 0.5f), new Vector3(0,1,.8f), cahaya[0]._centerPosition, _camera.Position);

            for (int i = 0; i < _object3d.Length; i++)
            {
                _object3d[i].render(0, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
                _object3d[i].setFragVariable(new Vector3(1f, 1f, 1f), _camera.Position);
                _object3d[i].setDirectionalLight(new Vector3(-1f, 0, 0.0f), new Vector3(0.1f), new Vector3(1), new Vector3(0.5f));
                //_object3d[i].setPointLight(cahaya[0]._centerPosition, new Vector3(0.05f, 0.05f, 0.05f), new Vector3(0.8f, 0.8f, 0.8f), new Vector3(1.0f, 1.0f, 1.0f), 1.0f, 0.09f, 0.032f);
                _object3d[i].setSpotLight(_camera.Position, _camera.Front, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f),
                1.0f, 0.09f, 0.032f, MathF.Cos(MathHelper.DegreesToRadians(12.5f)), MathF.Cos(MathHelper.DegreesToRadians(17.5f)));
                _object3d[i].setPointLights(_pointLightPositions, new Vector3(0.001f, 0.001f, 0.001f), point_light_color_difuse, new Vector3(1.0f, 1.0f, 1.0f), 1.0f, 0.045f, 0.0075f);
            }

            cubemap.Render(_camera);

            //untuk rotate
            if (mil_6)
            {
                _object3d[0].rotatede(_object3d[0]._centerPosition, _object3d[0]._euler[2], (float)args.Time * 64.4f);
            }
            _object3d[0].resetEuler();
            //_object3d[1].rotatede(_object3d[0]._centerPosition, _object3d[0]._euler[2], (float)args.Time * 50);
            //_object3d[1].resetEuler();
            SwapBuffers();

            //animasi
            if (second <= 16f) //16
            {
                _object3d[1].setDirectionalLight(new Vector3(0, 0, 1), new Vector3(0.1f), new Vector3(1), new Vector3(0.5f));
                _object3d[2].setDirectionalLight(new Vector3(1, 0, 0), new Vector3(0.01960784313f, 0.71764705882f, 0.96862745098f), new Vector3(0.9f), new Vector3(0.5f));
                _object3d[1].translate(0, -35f * (float)args.Time, 35f * (float)args.Time);
                _camera.Position += _camera.Right * 30f * (float)args.Time;
                _camera.Position -= _camera.Up * 20f * (float)args.Time;
            }
            else if (second <= 26f) //26
            {
                _object3d[1].setDirectionalLight(new Vector3(0, 0, 1), new Vector3(0.1f), new Vector3(1), new Vector3(0.5f));
                _object3d[2].setDirectionalLight(new Vector3(1, 0, 0), new Vector3(0.01960784313f, 0.71764705882f, 0.96862745098f), new Vector3(0.9f), new Vector3(0.5f));

                //cut
                if (mil_1 == true)
                {
                    _camera.Yaw = _lastPos.X + 90;
                    _camera.Position = new Vector3(69990, -530, -150);
                    mil_1 = false;
                }
                else
                {
                    _object3d[1].translate(35f * (float)args.Time, -35f * (float)args.Time, 35f * (float)args.Time);
                    _camera.Position -= _camera.Right * 30f * (float)args.Time;
                }
            }
            else if (second <= 34f) //34
            {
                _object3d[1].setDirectionalLight(new Vector3(0, 0, -1), new Vector3(0.1f), new Vector3(0.96862745098f, 0.63921568627f, 0.01960784313f), new Vector3(0.5f));
                _object3d[2].setDirectionalLight(new Vector3(0, 0, -1), new Vector3(0.01960784313f, 0.25f, 0.35f), new Vector3(0.96862745098f, 0.63921568627f, 0.01960784313f), new Vector3(0.5f));
                _object3d[3].setDirectionalLight(new Vector3(0, 0, 1), new Vector3(.1f), new Vector3(0.96862745098f, 0.63921568627f, 0.01960784313f), new Vector3(0.5f));

                //cut
                if (mil_2 == true)
                {
                    _camera.Position = new Vector3(70500, -700, -0);
                    _object3d[3].translate(70533.56f, -700f, 3038.0388f);
                    mil_2 = false;
                }
                else
                {
                    _object3d[1].translate(0, 0, 35f * (float)args.Time);
                }
            }
            else if (second <= 60f) //60
            {
                _object3d[0].setDirectionalLight(new Vector3(1, 0, 0), new Vector3(0.1f), new Vector3(1), new Vector3(0.5f));
                _object3d[2].setDirectionalLight(new Vector3(-1f, 0, 0.0f), new Vector3(0.01960784313f, 0.25f, 0.35f), new Vector3(0.9f), new Vector3(0.5f));

                if (mil_3 == true)
                {
                    _camera.Position = new Vector3(73420.29f, -2004.4227f, -292.43942f);
                    _object3d[3].translate(70533.56f, -700f, 3038.0388f);
                    mil_3 = false;
                }
                else
                {
                    _object3d[0].translate(60 * (float)args.Time,  25f * (float)args.Time, -25 * (float)args.Time);
                    _camera.Position += _camera.Front * 30 * (float)args.Time;
                    _camera.Position += _camera.Up * 30f * (float)args.Time;
                    _camera.Position += _camera.Right * -55f * (float)args.Time;
                }
            }
            else if (second <= 84) //90
            {
                _object3d[2].setDirectionalLight(new Vector3(-1f, 0, 0.0f), new Vector3(0.01960784313f, 0.25f, 0.35f), new Vector3(0.9f), new Vector3(0.5f));
                if (mil_4 == true)
                {
                    _camera.Yaw = 21; //15
                    _camera.Position = new Vector3(70343.09f, -856.57275f, 605.3335f);
                    mil_4 = false;
                }
                else
                {
                    Console.WriteLine(_camera.Yaw);
                    _camera.Position += _camera.Front * 50f * (float)args.Time;
                    _camera.Position += _camera.Up * 40f * (float)args.Time;
                    _camera.Position += _camera.Right * 35f * (float)args.Time;

                    _object3d[1].translate(50f * (float)args.Time, 40f * (float)args.Time  , 50f * (float)args.Time);
                }
            }
            else if (second <= 109) //109
            {
                _object3d[2].setDirectionalLight(new Vector3(-1f, 0, 0.0f), new Vector3(0.01960784313f, 0.25f, 0.35f), new Vector3(0.9f), new Vector3(0.5f));
                _camera.Position = _object3d[1]._centerPosition + new Vector3(-100, -50, 100);
                _object3d[1].setSpotLight((_object3d[1]._centerPosition - new Vector3(67.48f, 26.857571f, -8.8159f)), new Vector3(0, 1, 0), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(238f / 255f, 240f / 255f, 132f / 255f), new Vector3(1.0f, 1.0f, 1.0f),
1.0f, 0.09f, 0.032f, MathF.Cos(MathHelper.DegreesToRadians(12.5f)), MathF.Cos(MathHelper.DegreesToRadians(17.5f)));
                if (mil_5 == true)
                    {
                        //Camera Pos: (71784, 484; 354,4077; 2270,8372)
                        //Camera Yaw: -46,99647
                        //Camera Pitch: -35,497654

                        _camera.Yaw = -32.99345f;
                        _camera.Pitch = -14.999657f;
                        mil_5 = false;
                    }
                    else
                    {

                        _object3d[1].translate(50f * (float)args.Time, -40f * (float)args.Time, 30f * (float)args.Time);

                        Console.WriteLine(_camera.Yaw);
                        _camera.Position += _camera.Front * 42f * (float)args.Time;
                        _camera.Position -= _camera.Up * 32f * (float)args.Time;
                        _camera.Position += _camera.Right * 52f * (float)args.Time;
                    }
            }
            else if (second <= 120)
            {
                //Camera Pos: (74817, 164; -1475,972; 1360,2125)
                //Camera Yaw: 84, 5037
                //Camera Pitch: 2,5016618

                //Camera Pos: (74843, 555; -1469,6273; 1420,9337)
                _object3d[2].setDirectionalLight(new Vector3(-1f, 0, 0.0f), new Vector3(0.01960784313f, 0.25f, 0.35f), new Vector3(0.9f), new Vector3(0.5f));
                if (mil_6 == true)
                {
                    _camera.Yaw = 84.5037f;
                    _camera.Pitch = 2.5016618f;
                    //pesawat Camera Pos: (74209,87; -1404,5017; 2690,0674)
                    //Camera Pos: (74798,51; -1304,5562; 2360,5593)
                    _object3d[1].translate(1800f, 0 , 500f);
                    _object3d[1].rotatede(_object3d[1]._centerPosition, _object3d[1]._euler[1], 90);
                    _camera.Position = new Vector3(74817.164f, -1475.972f, 1420.9337f);   /*1360.2125*/
                    mil_6 = false;
                }
                else
                {
                    _object3d[1].translate(20f * (float)args.Time, -30f * (float)args.Time, -75f * (float)args.Time);
                    _camera.Position -= _camera.Front * 2f * (float)args.Time;
                }

            }
            else if (second <= 140)
            {
                _object3d[2].setDirectionalLight(new Vector3(-1f, 0, 0.0f), new Vector3(0.01960784313f, 0.25f, 0.35f), new Vector3(0.9f), new Vector3(0.5f));
                if (mil_9 == true)
                {
                    _camera.Yaw = 614.7728f;
                    _camera.Pitch = -5.998293f;
                    _camera.Position = new Vector3(74819.26f, -1279.6855f, 2508.6309f);
                    mil_9 = false;
                }
                else
                {
                    var axis = new Vector3(0, 1, 0);
                    _camera.Position -= _object3d[1]._centerPosition;
                    _camera.Yaw += _rotationSpeed;
                    _camera.Position = Vector3.Transform(_camera.Position,
                        generateArbRotationMatrix(axis, _object3d[1]._centerPosition, 50f * (float)args.Time).ExtractRotation());
                    _camera.Position += _object3d[1]._centerPosition;

                    _camera._front = -Vector3.Normalize(_camera.Position - _object3d[1]._centerPosition);
                }
            }

            else if (second <= 150)
            {
                _object3d[2].setDirectionalLight(new Vector3(-1f, 0, 0.0f), new Vector3(0.01960784313f, 0.25f, 0.35f), new Vector3(0.9f), new Vector3(0.5f));
                if (mil_7 == true)
                {
                    //Camera Pos: (74576, 086; -884,96405; 1766,183)
                    //Camera Yaw: -25,507923
                    //Camera Pitch: -57,999313

                    _camera.Yaw = -25.507923f;
                    _camera.Pitch = -57.999313f;
                    _camera.Position = new Vector3(74576.086f, -884.96405f, 1766.183f);   /*1360.2125*/
                    mil_7 = false;
                }
                else
                {
                    _object3d[0].rotatede(_object3d[0]._centerPosition, _object3d[0]._euler[2], (float)args.Time * 64.4f);
                    _camera.Position -= _camera.Front * 30f * (float)args.Time;
                    _object3d[1].translate(0, -20f * (float)args.Time, -55f * (float)args.Time);

                }

            }
            else if (second <= 180)
            {
                _object3d[2].setDirectionalLight(new Vector3(-1f, 0, 0.0f), new Vector3(0.01960784313f, 0.25f, 0.35f), new Vector3(0.9f), new Vector3(0.5f));
                if (mil_8 == true)
                {
                    //Camera Pos: (75051,51; -219,10968; 1134,8787)
                    //Camera Yaw: 309,98373
                    //Camera Pitch: -91,996704

                    _camera.Yaw = 309.98373f;
                    _camera.Pitch = -91.996704f;
                    _camera.Position = new Vector3(75051.51f, -219.10968f, 1134.8787f);
                    mil_8 = false;
                }
                else
                {
                    _object3d[0].rotatede(_object3d[0]._centerPosition, _object3d[0]._euler[2], (float)args.Time * 64.4f);
                    _camera.Position += _camera.Front * 60f * (float)args.Time;
                    _object3d[1].rotatede(_object3d[0]._centerPosition, _object3d[0]._euler[2], (float)args.Time * 64.4f);

                }

            }

        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {   
            base.OnUpdateFrame(e);
            if (!IsFocused)
            {
                return;
            }

            var input = KeyboardState;
            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            if (input.IsKeyDown(Keys.C))
            {
                camera_run = true;
            }
            if (input.IsKeyDown(Keys.F))
            {
                camera_run = false;
            }
            if (input.IsKeyDown(Keys.O))
            {
                camera_thirdp = true;
            }
            if (input.IsKeyDown(Keys.P))
            {
                camera_thirdp = false;
            }
            const float cameraSpeed = 100f;
            const float sensitivity = 0.5f;
            if (camera_run == true)
            {  
                if (input.IsKeyDown(Keys.W))
                {
                    _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
                }
                if (input.IsKeyDown(Keys.S))
                {
                    _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
                }
                if (input.IsKeyDown(Keys.A))
                {
                    _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
                }
                if (input.IsKeyDown(Keys.D))
                {
                    _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
                }
                if (input.IsKeyDown(Keys.Space))
                {
                    _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
                }
                if (input.IsKeyDown(Keys.LeftShift))
                {
                    _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
                }

                var mouse = MouseState;

                if (_firstMove)
                {
                    _lastPos = new Vector2(mouse.X, mouse.Y);
                    _firstMove = false;
                }
                else
                {
                    var deltaX = mouse.X - _lastPos.X;
                    var deltaY = mouse.Y - _lastPos.Y;
                    _lastPos = new Vector2(mouse.X, mouse.Y);

                    _camera.Yaw += deltaX * sensitivity;
                    _camera.Pitch -= deltaY * sensitivity;
                }

                if (KeyboardState.IsKeyDown(Keys.N))
                {
                    var axis = new Vector3(0, 1, 0);
                    _camera.Position -= _objecPost;
                    _camera.Yaw += _rotationSpeed;
                    _camera.Position = Vector3.Transform(_camera.Position,
                        generateArbRotationMatrix(axis, _objecPost, _rotationSpeed).ExtractRotation());
                    _camera.Position += _objecPost;

                    _camera._front = -Vector3.Normalize(_camera.Position - _objecPost);
                }
                if (KeyboardState.IsKeyDown(Keys.Comma))
                {
                    var axis = new Vector3(0, 1, 0);
                    _camera.Position -= _objecPost;
                    _camera.Yaw -= _rotationSpeed;
                    _camera.Position = Vector3.Transform(_camera.Position,
                        generateArbRotationMatrix(axis, _objecPost, -_rotationSpeed).ExtractRotation());
                    _camera.Position += _objecPost;

                    _camera._front = -Vector3.Normalize(_camera.Position - _objecPost);
                }
                if (KeyboardState.IsKeyDown(Keys.K))
                {
                    var axis = new Vector3(1, 0, 0);
                    _camera.Position -= _objecPost;
                    _camera.Pitch -= _rotationSpeed;
                    _camera.Position = Vector3.Transform(_camera.Position,
                        generateArbRotationMatrix(axis, _objecPost, _rotationSpeed).ExtractRotation());
                    _camera.Position += _objecPost;
                    _camera._front = -Vector3.Normalize(_camera.Position - _objecPost);
                }
                if (KeyboardState.IsKeyDown(Keys.M))
                {
                    var axis = new Vector3(1, 0, 0);
                    _camera.Position -= _objecPost;
                    _camera.Pitch += _rotationSpeed;
                    _camera.Position = Vector3.Transform(_camera.Position,
                        generateArbRotationMatrix(axis, _objecPost, -_rotationSpeed).ExtractRotation());
                    _camera.Position += _objecPost;
                    _camera._front = -Vector3.Normalize(_camera.Position - _objecPost);
                }
            }
            Console.WriteLine("Camera Pos: " + _camera.Position);
            Console.WriteLine("Camera Yaw: " + _camera.Yaw);
            Console.WriteLine("Camera Pitch: " + _camera.Pitch);

            //third-person-object
            if (camera_thirdp == true)
            {
                _camera.Position = _object3d[1]._centerPosition + new Vector3(100, 50, 100);
                if (input.IsKeyDown(Keys.W))
                {
                    _object3d[1].translate(0, 0, -100f * (float)e.Time); // Forward
                }
                if (input.IsKeyDown(Keys.S))
                {
                    _object3d[1].translate(0, 0, 100f * (float)e.Time); // Backwards
                }
                if (input.IsKeyDown(Keys.A))
                {
                    _object3d[1].translate(-100f * (float)e.Time, 0, 0); // Left
                }
                if (input.IsKeyDown(Keys.D))
                {
                    _object3d[1].translate(100f * (float)e.Time, 0, 0); // Right
                }
                if (input.IsKeyDown(Keys.Space))
                {
                    _object3d[1].translate(0, 100f * (float)e.Time, 0); // Up
                }
                if (input.IsKeyDown(Keys.LeftShift))
                {
                    _object3d[1].translate(0, -100f * (float)e.Time, 0); // Down
                }

                var mouse = MouseState;

                if (_firstMove)
                {
                    _lastPos = new Vector2(mouse.X, mouse.Y);
                    _firstMove = false;
                }
                else
                {
                    var deltaX = mouse.X - _lastPos.X;
                    var deltaY = mouse.Y - _lastPos.Y;
                    _lastPos = new Vector2(mouse.X, mouse.Y);

                    _camera.Yaw += deltaX * sensitivity;
                    _camera.Pitch -= deltaY * sensitivity;
                }
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if (camera_run == true)
            {
                _camera.Fov -= e.OffsetY;
            }
            else { 
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            _camera.AspectRatio = Size.X / (float)Size.Y;
        }

    }
}
