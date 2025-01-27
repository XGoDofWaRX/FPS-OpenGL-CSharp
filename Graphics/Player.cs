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
using System.Media;

namespace Graphics
{
    class Player
    {
        private float health;
        private bool dead;
        private float speed;
        private md2LOL model;
        public Camera camera;

        vec3 mPosition;
        private AABoundingBox collider;
        private HealthBar healthBar;

        float fireRate;
        float nextFire;
        DateTime now;

        SoundPlayer mySound;

        public Player()
        {            
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            model = new md2LOL(projectPath + "\\ModelFiles\\zombie.md2");
            mySound = new SoundPlayer(projectPath + "\\Sounds\\fire.wav");
            Initialize();          
        }

        public void Initialize()
        {
            health = 1.0f;
            dead = false;
            speed = 0.5f;
            mPosition = new vec3(0, 0, 0);
            float scale_value = 0.05f;
            vec3 scale_vec = new vec3(scale_value);
            model.rotationMatrix = glm.rotate((float)((-1.0f) * Math.PI), new vec3(0, 1, 1));
            model.scaleMatrix = glm.scale(new mat4(1), scale_vec);
            model.TranslationMatrix = glm.translate(new mat4(1), mPosition);
            camera = new Camera();
            camera.Reset(mPosition.x, mPosition.y + 2.25f, mPosition.z, 0, 0, 0, 0, 1, 0);
            fireRate = 0.25f;
            nextFire = 0.0f;
            now = DateTime.Now;
            //model.AnimationSpeed = 0.001f;
            //model.StartAnimation(animType_LOL.STAND);

            collider = new AABoundingBox(model.GetCurrentVertices(model.animSt), ColliderType.Player);
            collider.Scale(scale_vec);
            collider.SetCenter(mPosition);

            healthBar = new HealthBar(mPosition, health, HealthType.Player);
        }

        public void Draw(int matID)
        {
            model.Draw(matID);
            healthBar.drawHealth(health);
        }

        public void Update()
        {
            camera.UpdateViewMatrix();
            nextFire += (float)(DateTime.Now - now).TotalSeconds;
            now = DateTime.Now;
            if (health <= 0)
                dead = true;
            //healthBar.Update(mPosition);
            //model.UpdateExportedAnimation();
        }

        public void Yaw(float angleDegrees)
        {
            camera.Yaw(angleDegrees);
            model.rotationMatrix = glm.rotate(model.rotationMatrix, angleDegrees, new vec3(0, 0, 1));            
        }

        public void Walk(int dir, float maxDist, List<AABoundingBox> objects)
        {            
            vec3 translation_vector = (dir * speed) * camera.GetLookDirection();
            translation_vector.y = 0;
            if (!Collided(objects, translation_vector))
            {
                if (camera.Walk(dir * speed, maxDist))
                {
                    mPosition += translation_vector;                 
                    model.TranslationMatrix = glm.translate(model.TranslationMatrix, translation_vector);
                    collider.Translate(translation_vector);
                }
            }
        }

        public void Strafe(int dir, float maxDist, List<AABoundingBox> objects)
        {
            vec3 translation_vector = (dir * speed) * camera.GetRightDirection();
            translation_vector.y = 0;
            if (!Collided(objects, translation_vector))
            {
                if (camera.Strafe(dir * speed, maxDist))
                {
                    mPosition += translation_vector;
                    model.TranslationMatrix = glm.translate(model.TranslationMatrix, translation_vector);
                    collider.Translate(translation_vector);
                }
            }
        }

        public void Fire(List<Bullet> bulletList)
        {
            if (nextFire >= fireRate)
            {
                Bullet bullet = new Bullet(camera.GetCameraPosition(), camera.GetLookDirection());
                bulletList.Add(bullet);
                nextFire = 0.0f;
                mySound.Play();
            }
        }

        public bool Collided(List<AABoundingBox> objects, vec3 offset)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if(collider.CheckCollision(objects[i], offset))
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

        public bool isDead()
        {
            return dead;
        }
        public float GetHealth()
        {
            return health;
        }
        public vec3 GetPosition()
        {
            return mPosition;
        }
        public AABoundingBox GetCollider()
        {
            return collider;
        }

    }
}
