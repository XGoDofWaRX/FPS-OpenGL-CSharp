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
    enum EnemyStat { IDLE, RUNNING, CHASING, ATTACKING, DYING}
    class Enemy
    {
        private float health;
        private float damage;
        private bool dead;
        private float speed;
        private md2LOL model;

        float mAngleX = 0;
        vec3 mDirection;
        vec3 mPosition;

        public EnemyStat state;
        private AABoundingBox collider;
        private HealthBar healthBar;        

        public Enemy()
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            model = new md2LOL(projectPath + "\\ModelFiles\\zombie.md2");
            Initialize();
        }

        public void Initialize()
        {
            health = 1.0f;
            damage = 0.001f;
            dead = false;
            speed = 0.01f;
            mAngleX = 0;
            float x = RandomNumber(-50, 50);
            float z = RandomNumber(-50, 50);
            mPosition = new vec3(x, 0, z);
            mDirection = new vec3(0, 0, 0) - mPosition;
            mDirection = glm.normalize(mDirection);
            float scale_value = 0.05f;
            vec3 scale_vec = new vec3(scale_value);
            model.rotationMatrix = glm.rotate((float)((-0.5f) * Math.PI), new vec3(1, 0, 0));
            model.scaleMatrix = glm.scale(new mat4(1), scale_vec);
            model.TranslationMatrix = glm.translate(new mat4(1), mPosition);

            state = EnemyStat.IDLE;
            model.AnimationSpeed = 0.003f;
            model.StartAnimation(animType_LOL.STAND);

            collider = new AABoundingBox(model.GetCurrentVertices(model.animSt), ColliderType.Enemy);
            collider.Scale(scale_vec);
            collider.Scale(1.1f, 3.0f, 0);
            collider.SetCenter(mPosition);

            healthBar = new HealthBar(mPosition, health, HealthType.Enemy);
        }

        public void Draw(int matID)
        {
            model.Draw(matID);
            if(state != EnemyStat.DYING)
                healthBar.drawHealth(health);
        }

        public void Update(Player hero, float maxDist, List<AABoundingBox> objects)
        {            
            if (!dead)
            {
                if (state != EnemyStat.DYING)
                {
                    vec3 playerDistance = hero.GetPosition() - mPosition;
                    playerDistance.x = Math.Abs(playerDistance.x);
                    playerDistance.z = Math.Abs(playerDistance.z);

                    if (playerDistance.x < 1.0f && playerDistance.z < 1.0f)
                    {
                        hero.Damage(damage);
                        state = EnemyStat.ATTACKING;
                        model.AnimationSpeed = 0.03f;
                        if (model.animSt.type != animType_LOL.ATTACK1)
                            model.StartAnimation(animType_LOL.ATTACK1);
                    }
                    else
                    {
                        state = EnemyStat.CHASING;
                        model.AnimationSpeed = 0.003f;
                        if (model.animSt.type != animType_LOL.RUN)
                            model.StartAnimation(animType_LOL.RUN);
                    }

                    if (state == EnemyStat.CHASING)
                    {
                        Move(maxDist, objects);
                    }

                    ChangeDirection(hero.GetPosition());

                    if (health <= 0)
                    {
                        state = EnemyStat.DYING;
                        model.AnimationSpeed = 0.003f;
                        if (model.animSt.type != animType_LOL.DEATH)
                            model.StartAnimation(animType_LOL.DEATH);
                    }
                }

                else
                {
                    if (model.animSt.curr_frame == model.animSt.endframe)
                    {
                        dead = true;
                    }
                }
                model.UpdateExportedAnimation();
                //healthBar.Update(mPosition);
            }
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

        public void Move(float maxDist, List<AABoundingBox> objects)
        {
            vec3 translation_vector = speed * mDirection;
            if (!Collided(objects, translation_vector))
            {
                if (CheckMove(maxDist))
                {
                    mPosition += translation_vector;
                    model.TranslationMatrix = glm.translate(new mat4(1), new vec3(mPosition.x, mPosition.y, mPosition.z));
                    collider.Translate(translation_vector);
                }
                else
                {
                    ChangeDirection();
                }
            }
            else
            {
                ChangeDirection();
            }
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

        public bool Collided(List<AABoundingBox> objects, vec3 offset)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (collider.CheckCollision(objects[i], offset))
                {
                    return true;
                }
            }
            return false;
        }

        public void Damage(float dmg)
        {
            health -= dmg;
        }

        public AABoundingBox GetCollider()
        {
            return collider;
        }
        public float GetHealth()
        {
            return health;
        }
        public bool isDead()
        {
            return dead;
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
