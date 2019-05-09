using System;
using System.Windows.Forms;
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
    class Screen
    {
        private Texture screen;
        private float[] screenSquare;

        public Screen(string name= "")
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            //screen = new Texture(projectPath + "\\Textures\\" + name, 25);
            screen = new Texture(projectPath + "\\Textures\\CALLofDUTY.jpg", 25, true); 
            Initialize();
        }

        public void Initialize()
        {
            //front
            screenSquare = new float[] {
                 7.0f, 10.0f, -10.0f,        //1
                 0,0,1,
                 1,1,
                 0,1,0,

                 -7.0f, 10.0f, -10.0f,       //2
                 0,0,1,
                 0,1,
                 0,1,0,

                 7.0f, -5.0f, -10.0f,      //3
                 0,0,1,
                 1,0,
                 0,1,0,

                 -7.0f, 10.0f, -10.0f,        //4
                 0,0,1,
                 0,1,
                 0,1,0,

                 -7.0f, -5.0f, -10.0f,        //5
                 0,0,1,
                 0,0,
                 0,1,0,

                 7.0f, -5.0f, -10.0f,       //6
                 0,0,1,
                 1,0,
                 0,1,0,
            };          
        }

        public Texture GetScreenTexture()
        {
            return screen;           
        }
        public float[] GetScreenSquare()
        {
            return screenSquare;
        }
    }
}
