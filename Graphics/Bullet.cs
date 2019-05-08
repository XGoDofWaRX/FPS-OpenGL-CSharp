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
        private float damage;       
        private float speed;
        private Texture bulletTex;
        private Model3D model;

        vec3 mDirection;
        vec3 mPosition;
        float maxDistance;
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
            damage = 0.25f;
            speed = 0.1f;
            maxDistance = 100.0f;
            mPosition = pos;
            mDirection = dir;
            mDirection = glm.normalize(mDirection);
            float scale_value = 0.005f;
            vec3 scale_vec = new vec3(scale_value);
            double angle = glm.atan(mDirection.z, mDirection.x);
            model.rotmatrix = glm.rotate((float)(-angle), new vec3(0, 1, 0));
            model.scalematrix = glm.scale(new mat4(1), scale_vec);
            model.transmatrix = glm.translate(new mat4(1), mPosition);

            collider = new AABoundingBox(model.GetCurrentVertices(), ColliderType.Bullet);
            collider.Scale(scale_vec);
            collider.SetCenter(mPosition);
        }

        public void Draw(int matID)
        {
            bulletTex.Bind();
            model.Draw(matID);
        }

        public bool Update(List<AABoundingBox> objects)
        {            
            maxDistance -= speed;
            Move();
            if(FinishedMoving() || Collided(objects))
            {
                return true;
            }
            return false;
        }

        public void Move()
        {
            vec3 translation_vector = speed * mDirection;
            mPosition += translation_vector;
            model.transmatrix = glm.translate(new mat4(1), new vec3(mPosition.x, mPosition.y, mPosition.z));
            collider.Translate(translation_vector);
        }

        public bool FinishedMoving()
        {            
            if (maxDistance <= 0)
            {
                return true;
            }
            return false;
        }

        public bool Collided(List<AABoundingBox> objects)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (collider.CheckCollision(objects[i], new vec3(0)))
                {
                    return true;
                }
            }
            return false;
        }

        public float GetDamage()
        {
            return damage;
        }
        public AABoundingBox GetCollider()
        {
            return collider;
        }
    }
}
