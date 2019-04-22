using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;
using GlmNet;
using System.IO;
using Graphics._3D_Models;
namespace Graphics
{
    class Renderer
    {
        Shader sh;
        int transID;
        int viewID;
        int projID;

        int EyePositionID;
        int AmbientLightID;
        int DataID;

        mat4 ProjectionMatrix;
        mat4 ViewMatrix;

        public float Speed = 1;
        public float skyboxSize = 100.0f;

        uint vertexBufferID;

        public Camera cam;

        public md2LOL zombie;
        public md2LOL zombie2;
        public Model3D jeep;
        public Model3D jeep2;
        public Model3D house;

        Texture up;
        Texture down;
        Texture left;
        Texture right;
        Texture front;
        Texture back;

        mat4 modelmatrix;
        public void Initialize()
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");

            up = new Texture(projectPath + "\\Textures\\SunSetUp2048.png", 2);
            down = new Texture(projectPath + "\\Textures\\grass.png", 3, true);
            right = new Texture(projectPath + "\\Textures\\SunSetLeft2048.png", 4, true);
            left = new Texture(projectPath + "\\Textures\\SunSetRight2048.png", 5, true);
            front = new Texture(projectPath + "\\Textures\\SunSetFront2048.png", 6, true);
            back = new Texture(projectPath + "\\Textures\\SunSetBack2048.png", 7, true);

            //zombie
            zombie = new md2LOL(projectPath + "\\ModelFiles\\zombie.md2");
            zombie.StartAnimation(animType_LOL.ATTACK1);
            zombie.rotationMatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0));
            zombie.scaleMatrix = glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.1f));
            zombie.TranslationMatrix = glm.translate(new mat4(1), new vec3(10, 0, 0));
            //zombie2
            zombie2 = new md2LOL(projectPath + "\\ModelFiles\\zombie.md2");
            zombie2.StartAnimation(animType_LOL.ATTACK2);
            zombie2.rotationMatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0));
            zombie2.scaleMatrix = glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.1f));
            zombie2.TranslationMatrix = glm.translate(new mat4(1), new vec3(-10, 0, 0));            

            //House
            house = new Model3D();
            house.LoadFile(projectPath + "\\ModelFiles\\models\\3DS\\House", 9, "house.obj");                       

            //jeep
            jeep = new Model3D();
            jeep.LoadFile(projectPath + "\\ModelFiles\\models\\3DS\\jeep", 10, "jeep1.3ds");
            jeep.scalematrix = glm.scale(new mat4(1), new vec3(0.3f, 0.3f, 0.3f));
            jeep.transmatrix = glm.translate(new mat4(1), new vec3(-6, 0, 0));
            jeep.rotmatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0));

            //jeep2
            jeep2 = new Model3D();
            jeep2.LoadFile(projectPath + "\\ModelFiles\\models\\3DS\\jeep", 10, "jeep1.3ds");
            jeep2.scalematrix = glm.scale(new mat4(1), new vec3(0.3f, 0.3f, 0.3f));
            jeep2.transmatrix = glm.translate(new mat4(1), new vec3(6, 0, 0));
            jeep2.rotmatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0));

            float[] skybox = {
                //up
                -skyboxSize, skyboxSize, skyboxSize,        //0
                 0,0,1,
                 0,0,
                 0,1,0,

                 skyboxSize, skyboxSize, -skyboxSize,       //1
                 0,0,1,
                 1,1,
                 0,1,0,

                 -skyboxSize, skyboxSize, -skyboxSize,      //2
                 0,0,1,
                 0,1,
                 0,1,0,

                 skyboxSize, skyboxSize, skyboxSize,        //3
                 0,0,1,
                 1,0,
                 0,1,0,

                -skyboxSize, skyboxSize, skyboxSize,        //4
                 0,0,1,
                 0,0,
                 0,1,0,

                 skyboxSize, skyboxSize, -skyboxSize,       //5
                 0,0,1,
                 1,1,
                 0,1,0,
                 
                //ground
                -skyboxSize, 0.0f, skyboxSize,        //6
                 0,0,1,
                 0,0,
                 0,1,0,

                 skyboxSize, 0.0f, -skyboxSize,       //7
                 0,0,1,
                 1,1,
                 0,1,0,

                 -skyboxSize, 0.0f, -skyboxSize,      //8
                 0,0,1,
                 0,1,
                 0,1,0,

                 skyboxSize, 0.0f, skyboxSize,        //9
                 0,0,1,
                 1,0,
                 0,1,0,

                -skyboxSize, 0.0f, skyboxSize,        //10
                 0,0,1,
                 0,0,
                 0,1,0,

                 skyboxSize, 0.0f, -skyboxSize,       //11
                 0,0,1,
                 1,1,
                 0,1,0,

                 //left
                -skyboxSize, 0.0f, skyboxSize,        //12
                 0,0,1,
                 0,0,
                 0,1,0,

                 -skyboxSize, skyboxSize, -skyboxSize,       //13
                 0,0,1,
                 1,1,
                 0,1,0,

                 -skyboxSize, skyboxSize, skyboxSize,      //14
                 0,0,1,
                 0,1,
                 0,1,0,

                 -skyboxSize, 0.0f, -skyboxSize,        //15
                 0,0,1,
                 1,0,
                 0,1,0,

                 -skyboxSize, 0.0f, skyboxSize,        //16
                 0,0,1,
                 0,0,
                 0,1,0,

                 -skyboxSize, skyboxSize, -skyboxSize,       //17
                 0,0,1,
                 1,1,
                 0,1,0,

                 //right
                 skyboxSize, 0.0f, skyboxSize,        //18
                 0,0,1,
                 1,0,
                 0,1,0,

                 skyboxSize, skyboxSize, -skyboxSize,       //19
                 0,0,1,
                 0,1,
                 0,1,0,

                 skyboxSize, 0.0f, -skyboxSize,      //20
                 0,0,1,
                 0,0,
                 0,1,0,

                 skyboxSize, skyboxSize, skyboxSize,        //21
                 0,0,1,
                 1,1,
                 0,1,0,

                 skyboxSize, 0.0f, skyboxSize,        //22
                 0,0,1,
                 1,0,
                 0,1,0,

                 skyboxSize, skyboxSize, -skyboxSize,       //23
                 0,0,1,
                 0,1,
                 0,1,0,

                 //front
                 -skyboxSize, 0.0f, -skyboxSize,        //24
                 0,0,1,
                 0,0,
                 0,1,0,

                 skyboxSize, skyboxSize, -skyboxSize,       //25
                 0,0,1,
                 1,1,
                 0,1,0,

                 -skyboxSize, skyboxSize, -skyboxSize,      //26
                 0,0,1,
                 0,1,
                 0,1,0,

                 skyboxSize, 0.0f, -skyboxSize,        //27
                 0,0,1,
                 1,0,
                 0,1,0,

                 -skyboxSize, 0.0f, -skyboxSize,        //28
                 0,0,1,
                 0,0,
                 0,1,0,

                 skyboxSize, skyboxSize, -skyboxSize,       //29
                 0,0,1,
                 1,1,
                 0,1,0,

                 //back
                 -skyboxSize, 0.0f, skyboxSize,        //30
                 0,0,1,
                 1,0,
                 0,1,0,

                 skyboxSize, skyboxSize, skyboxSize,       //31
                 0,0,1,
                 0,1,
                 0,1,0,

                 -skyboxSize, skyboxSize, skyboxSize,      //32
                 0,0,1,
                 1,1,
                 0,1,0,

                 skyboxSize, 0.0f, skyboxSize,        //33
                 0,0,1,
                 0,0,
                 0,1,0,

                 -skyboxSize, 0.0f, skyboxSize,        //34
                 0,0,1,
                 1,0,
                 0,1,0,

                 skyboxSize, skyboxSize, skyboxSize,       //35
                 0,0,1,
                 0,1,
                 0,1,0
            };
            vertexBufferID = GPU.GenerateBuffer(skybox);

            Gl.glClearColor(0, 0, 0.4f, 1);
            cam = new Camera();
            cam.Reset(0, 4, 20, 0, 0, 0, 0, 1, 0);

            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();

            transID = Gl.glGetUniformLocation(sh.ID, "trans");
            projID = Gl.glGetUniformLocation(sh.ID, "projection");
            viewID = Gl.glGetUniformLocation(sh.ID, "view");


            modelmatrix = glm.scale(new mat4(1), new vec3(1, 1, 1));

            sh.UseShader();

            DataID = Gl.glGetUniformLocation(sh.ID, "data");
            vec2 data = new vec2(5, 50);
            Gl.glUniform2fv(DataID, 1, data.to_array());

            int LightPositionID = Gl.glGetUniformLocation(sh.ID, "LightPosition_worldspace");
            vec3 lightPosition = new vec3(100.0f, 55.0f, 48.0f);
            Gl.glUniform3fv(LightPositionID, 1, lightPosition.to_array());
            //setup the ambient light component.
            AmbientLightID = Gl.glGetUniformLocation(sh.ID, "ambientLight");
            vec3 ambientLight = new vec3(1.5f, 1.3f, 1.3f);
            Gl.glUniform3fv(AmbientLightID, 1, ambientLight.to_array());
            //setup the eye position.
            EyePositionID = Gl.glGetUniformLocation(sh.ID, "EyePosition_worldspace");
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LESS);
        }

        public void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT|Gl.GL_DEPTH_BUFFER_BIT);
            sh.UseShader();

            Gl.glUniformMatrix4fv(projID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());
            Gl.glUniformMatrix4fv(viewID, 1, Gl.GL_FALSE, ViewMatrix.to_array());

            Gl.glUniform3fv(EyePositionID, 1, cam.GetCameraPosition().to_array());

            zombie.Draw(transID);
            zombie2.Draw(transID);
            house.Draw(transID);
            jeep.Draw(transID);
            jeep2.Draw(transID);

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, vertexBufferID);
            Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, modelmatrix.to_array());
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 11 * sizeof(float), IntPtr.Zero);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 11 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glEnableVertexAttribArray(2);
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 11 * sizeof(float), (IntPtr)(6 * sizeof(float)));
            Gl.glEnableVertexAttribArray(3);
            Gl.glVertexAttribPointer(3, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 11 * sizeof(float), (IntPtr)(8 * sizeof(float)));
            up.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
            down.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 6, 6);
            left.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 12, 6);
            right.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 18, 6);
            front.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 24, 6);
            back.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 30, 6);
        }
        public void Update(float deltaTime)
        {
            cam.UpdateViewMatrix();
            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();
            zombie.UpdateExportedAnimation();
            zombie2.UpdateExportedAnimation();            
        }
        public void SendLightData(float red, float green, float blue, float attenuation, float specularExponent)
        {
            vec3 ambientLight = new vec3(red, green, blue);
            Gl.glUniform3fv(AmbientLightID, 1, ambientLight.to_array());
            vec2 data = new vec2(attenuation, specularExponent);
            Gl.glUniform2fv(DataID, 1, data.to_array());
        }
        public void CleanUp()
        {
            sh.DestroyShader();
        }
    }
}
