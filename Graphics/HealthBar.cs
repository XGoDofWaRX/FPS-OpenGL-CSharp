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
    enum HealthType { Player, Enemy }
    class HealthBar
    {
        private float maxHealth;        
        private vec3 position;
        HealthType myType;
        private Texture healthtTex;

        public HealthBar(vec3 pos, float max_health, HealthType type)
        {
            maxHealth = max_health;            
            position = pos;
            myType = type;
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            healthtTex = new Texture(projectPath + "\\Textures\\Green.png", 22, true);
        }
       
        public void drawHealth(float health)
        {
            healthtTex.Bind();
            if (myType == HealthType.Player)
            {
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glVertex3f(9f, position.y, 52.5f);
                Gl.glVertex3f(9f - (health * 7), position.y, 52.5f);
                Gl.glVertex3f(9f - (health * 7), position.y, 54);
                Gl.glVertex3f(9f, position.y, 54);
                Gl.glEnd();
            }
            else if (myType == HealthType.Enemy)
            {
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glVertex3f(-15, 0, 50);
                Gl.glVertex3f(-15 + (health * 30), 0, 50);
                Gl.glVertex3f(-15 + (health * 30), 0, 55);
                Gl.glVertex3f(-15, 0, 55);               
                Gl.glEnd();
            }
        }

        public void Update(vec3 pos)
        {
            //position = pos;
        }
    }
}
