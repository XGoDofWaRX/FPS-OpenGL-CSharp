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
    class Player
    {
        private int health;
        private bool dead;
        private float speed;
        private md2LOL model;
        public Camera camera;

        double fireRate;
        double nextFire;
        DateTime now;

        public Player()
        {            
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            model = new md2LOL(projectPath + "\\ModelFiles\\zombie.md2");
            Initialize();          
        }

        public void Initialize()
        {
            health = 100;
            dead = false;
            speed = 0.5f;           
            vec3 initialPosition = new vec3(0, 0, 0);
            model.rotationMatrix = glm.rotate((float)((-1.0f) * Math.PI), new vec3(0, 1, 1));
            model.scaleMatrix = glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.1f));
            model.TranslationMatrix = glm.translate(new mat4(1), initialPosition);
            camera = new Camera();
            camera.Reset(initialPosition.x, initialPosition.y + 4.5f, initialPosition.z, 0, 0, 0, 0, 1, 0);
            fireRate = 50.0f;
            nextFire = 0.0f;
            now = DateTime.Now;
            //model.AnimationSpeed = 0.001f;
            //model.StartAnimation(animType_LOL.STAND);
        }

        public void Draw(int matID)
        {
            //model.Draw(matID);
        }

        public void Update()
        {            
            camera.UpdateViewMatrix();
            nextFire += (DateTime.Now - now).TotalSeconds;
            //model.UpdateExportedAnimation();
        }

        public void Yaw(float angleDegrees)
        {
            camera.Yaw(angleDegrees);
            model.rotationMatrix = glm.rotate(model.rotationMatrix, angleDegrees, new vec3(0, 0, 1));            
        }

        public void Walk(int dir, float maxDist)
        {
            if (camera.Walk(dir * speed, maxDist))
            {
                model.TranslationMatrix = glm.translate(model.TranslationMatrix,
                                                        (dir * speed) * new vec3(camera.GetLookDirection().x,
                                                                                 0,
                                                                                 camera.GetLookDirection().z));
            }
        }

        public void Strafe(int dir, float maxDist)
        {
            if (camera.Strafe(dir * speed, maxDist))
            {
                model.TranslationMatrix = glm.translate(model.TranslationMatrix,
                                                        (dir * speed) * new vec3(camera.GetRightDirection().x,
                                                                                 0,
                                                                                 camera.GetRightDirection().z));
            }
        }

        public void Fire(List<Bullet> bulletList)
        {
            if (nextFire >= fireRate)
            {
                Bullet bullet = new Bullet(camera.GetCameraPosition(), camera.GetLookDirection());
                bulletList.Add(bullet);
                now = DateTime.Now;
                nextFire = 0.0f;
            }
        }

        public bool isDead()
        {
            return dead;
        }

        public int GetHealth()
        {
            return health;
        }
    }
}
