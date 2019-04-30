﻿using System;
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
            speed = 0.008f;
            mAngleX = 0;
            mPosition = new vec3(0, 0, -100);
            mDirection = new vec3(0, 0, 0) - mPosition;
            mDirection = glm.normalize(mDirection);
            model.rotationMatrix = glm.rotate((float)((-0.5f) * Math.PI), new vec3(1, 0, 0));
            model.scaleMatrix = glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.1f));
            model.TranslationMatrix = glm.translate(new mat4(1), mPosition);
            changeDirectionRate = 10000.0f;
            directionCounter = 0.0f;
            now = DateTime.Now;
            state = EnemyStat.RUNNING;

            model.AnimationSpeed = 0.001f;
            model.StartAnimation(animType_LOL.RUN);
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
            
            if (playerDistance.x > 20.0f && playerDistance.z > 20.0f )
            {
                state = EnemyStat.RUNNING;
                if (model.animSt.type != animType_LOL.RUN)
                    model.StartAnimation(animType_LOL.RUN);
            }
            else if(playerDistance.x < 20.0f && playerDistance.z < 20.0f)
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
                mPosition += speed * mDirection;
                model.TranslationMatrix = glm.translate(new mat4(1),
                                                        new vec3(mPosition.x, 0, mPosition.z));
            }
            else
            {
                ChangeDirection();
            }
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
