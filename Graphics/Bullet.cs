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
    class Bullet
    {
        private int damage;       
        private float speed;
        private Texture bulletTex;
        private Model3D model;

        vec3 mDirection;
        vec3 mPosition;

        private AABoundingBox collider;

        public Bullet(vec3 pos, vec3 dir)
        {          
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            model = new Model3D();
            model.LoadFile(projectPath + "\\ModelFiles\\models\\obj\\Bullet", 20, "Bullet.obj");
            bulletTex = new Texture(projectPath + "\\Textures\\Ground.jpg", 21, true);
            Initialize(pos, dir);
        }

        public void Initialize(vec3 pos, vec3 dir)
        {
            damage = 25;
            speed = 0.05f;
            mPosition = pos;
            mDirection = dir;
            mDirection = glm.normalize(mDirection);
            double angle = glm.atan(mDirection.z, mDirection.x);
            model.rotmatrix = glm.rotate((float)(-angle), new vec3(0, 1, 0));
            model.scalematrix = glm.scale(new mat4(1), new vec3(0.005f, 0.005f, 0.005f));
            model.transmatrix = glm.translate(new mat4(1), pos);

            collider = new AABoundingBox(model.GetCurrentVertices(), ColliderType.Bullet);
        }

        public void Draw(int matID)
        {
            bulletTex.Bind();
            model.Draw(matID);
        }

        public bool Update(float maxDist)
        {
            Move();
            if(!CheckMove(maxDist))
            {
                return true;
            }
            return false;
        }

        public void Move()
        {
            vec3 translation_vector = speed * mDirection;
            mPosition += translation_vector;
            model.transmatrix = glm.translate(new mat4(1),
                                                    new vec3(mPosition.x, mPosition.y, mPosition.z));
            collider.Translate(translation_vector);
        }

        public bool CheckMove(float maxDist)
        {
            if (mPosition.x > maxDist
                || mPosition.x < -maxDist
                || mPosition.z > maxDist
                || mPosition.z < -maxDist)
            {
                return false;
            }
            return true;
        }

        public int GetDamage()
        {
            return damage;
        }

        public AABoundingBox GetCollider()
        {
            return collider;
        }
    }
}
