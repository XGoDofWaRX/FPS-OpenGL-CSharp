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
    enum EnemyStat { IDLE, RUNNING, CHASING, ATTACKING}
    class Enemy
    {
        private int health;
        private bool dead;
        private float speed;
        private md2LOL model;

        float mAngleX = 0;
        vec3 mDirection;
        vec3 mPosition;

        double changeDirectionRate;
        double directionCounter;
        DateTime now;

        public EnemyStat state;
        private AABoundingBox collider;

        public Enemy()
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            model = new md2LOL(projectPath + "\\ModelFiles\\zombie.md2");
            Initialize();
        }

        public void Initialize()
        {
            health = 100;
            dead = false;
            speed = 0.03f;
            mAngleX = 0;
            float x = RandomNumber(-70, 70);
            float z = RandomNumber(-70, 70);
            mPosition = new vec3(x, 0, z);
            mDirection = new vec3(0, 0, 0) - mPosition;
            mDirection = glm.normalize(mDirection);
            model.rotationMatrix = glm.rotate((float)((-0.5f) * Math.PI), new vec3(1, 0, 0));
            model.scaleMatrix = glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.1f));
            model.TranslationMatrix = glm.translate(new mat4(1), mPosition);
            changeDirectionRate = 1000.0f;
            directionCounter = 900.0f;
            now = DateTime.Now;
            state = EnemyStat.RUNNING;

            model.AnimationSpeed = 0.003f;
            model.StartAnimation(animType_LOL.RUN);

            collider = new AABoundingBox(model.GetCurrentVertices(model.animSt), ColliderType.Enemy);
        }

        public void Draw(int matID)
        {
            model.Draw(matID);
        }

        public void Update(vec3 playerPos, float maxDist)
        {
            vec3 playerDistance = playerPos - mPosition;
            playerDistance.x = Math.Abs(playerDistance.x);
            playerDistance.z = Math.Abs(playerDistance.z);
            
            if (playerDistance.x > 50.0f || playerDistance.z > 50.0f)
            {
                state = EnemyStat.CHASING;
                if (model.animSt.type != animType_LOL.RUN)
                    model.StartAnimation(animType_LOL.RUN);
            }
            else if(playerDistance.x < 50.0f && playerDistance.z < 50.0f)
            {
                if (playerDistance.x < 3.0f && playerDistance.z < 3.0f)
                {
                    state = EnemyStat.ATTACKING;
                    if (model.animSt.type != animType_LOL.ATTACK1)
                        model.StartAnimation(animType_LOL.ATTACK1);
                }
                else
                {
                    state = EnemyStat.CHASING;
                    if (model.animSt.type != animType_LOL.RUN)
                        model.StartAnimation(animType_LOL.RUN);
                }
            }

            switch(state)
            {
                case EnemyStat.RUNNING:
                    directionCounter += (DateTime.Now - now).TotalSeconds;
                    if (directionCounter >= changeDirectionRate)
                    {
                        now = DateTime.Now;
                        directionCounter = 0.0f;
                        ChangeDirection();
                    }
                    Move(maxDist);
                    break;
                case EnemyStat.CHASING:
                    ChangeDirection(playerPos);
                    Move(maxDist);
                    break;
                case EnemyStat.ATTACKING:
                    ChangeDirection(playerPos);
                    break;
            }
            
            model.UpdateExportedAnimation();                       
        }
       
        public void ChangeDirection()
        {
            float degrees = RandomNumber(-1, 1);
            Yaw(degrees);
            model.rotationMatrix = glm.rotate((float)((-0.5f) * Math.PI), new vec3(1, 0, 0));
            model.rotationMatrix = glm.rotate(model.rotationMatrix, (float)(-degrees * Math.PI), new vec3(0, 0, 1));
            mDirection = new vec3((float)(-1 * Math.Sin(mAngleX))
                                  , 0
                                  , (float)(-1 * Math.Cos(mAngleX)));
            mDirection = glm.normalize(mDirection);
        }

        public void ChangeDirection(vec3 playerPos)
        {
            vec3 diff = playerPos - mPosition;
            mDirection = glm.normalize(diff);
            double angle = glm.atan(mDirection.z, mDirection.x);
            model.rotationMatrix = glm.rotate((float)((-0.5f) * Math.PI), new vec3(1, 0, 0));
            model.rotationMatrix = glm.rotate(model.rotationMatrix, (float)(0.5f * Math.PI), new vec3(0, 0, 1));
            model.rotationMatrix = glm.rotate(model.rotationMatrix, (float)(-angle), new vec3(0, 0, 1));
        }

        public void Yaw(float angleDegrees)
        {
            mAngleX += angleDegrees;
        }

        public void Move(float maxDist)
        {
            // TODO Boundary and collisions Checking
            if (CheckMove(maxDist))
            {
                vec3 translation_vector = speed * mDirection;
                mPosition += translation_vector;
                model.TranslationMatrix = glm.translate(new mat4(1),
                                                        new vec3(mPosition.x, mPosition.y, mPosition.z));
                collider.Translate(translation_vector);

            }
            else
            {
                ChangeDirection();
            }
        }

        public AABoundingBox GetCollider()
        {
            return collider;
        }
        
        public int GetHealth()
        {
            return health;
        }
        public bool isDead()
        {
            return dead;
        }

        public bool CheckMove(float maxDist)
        {

            if ((mPosition + (speed * mDirection)).x > (maxDist - 2.0f)
                    || (mPosition + (speed * mDirection)).x < (-maxDist + 2.0f)
                    || (mPosition + (speed * mDirection)).z > (maxDist - 2.0f)
                    || (mPosition + (speed * mDirection)).z < (-maxDist + 2.0f))
            {
                return false;
            }
            return true;
        }

        // Generate a random number between two numbers
        public float RandomNumber(int min, int max)
        {
            min *= 100;
            max *= 100;
            Random random = new Random();
            int rnd = random.Next(min, max);
            float finalRandom = (float)(rnd / 100.0f);
            return finalRandom;
        }
    }
}
