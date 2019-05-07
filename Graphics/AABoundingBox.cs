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
    enum ColliderType { Player, Bullet, Enemy, Obstacle }
    class AABoundingBox
    {
        public ColliderType myType;
        public vec3 center;
        public vec3 size;
        public vec3 halfSize;

        public AABoundingBox(List<vec3> verts, ColliderType type)
        {
            myType = type;
            MakeBox(verts);
        }

        public void MakeBox(List<vec3> verts)
        {
            vec3 minVertex = new vec3(float.MaxValue);
            vec3 maxVertex = new vec3(float.MinValue);

            for (int i = 0; i < verts.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (minVertex[j] > verts[i][j])
                    {
                        minVertex[j] = verts[i][j];
                    }
                    if (maxVertex[j] < verts[i][j])
                    {
                        maxVertex[j] = verts[i][j];
                    }
                }
            }
            center = (maxVertex + minVertex) / 2.0f;
            size = new vec3 (Math.Abs(maxVertex.x - minVertex.x),
                             Math.Abs(maxVertex.y - minVertex.y),
                             Math.Abs(maxVertex.z - minVertex.z));
            halfSize = new vec3(size / 2.0f);
        }

        public void Scale(float scale)
        {
            vec3 scaleVec = new vec3(scale, scale, scale);
            Scale(scaleVec);
        }

        public void Scale(float scaleX, float scaleY, float scaleZ)
        {
            vec3 scaleVec = new vec3(scaleX, scaleY, scaleZ);
            Scale(scaleVec);
        }

        public void Scale(vec3 scaleVec)
        {
            vec3 newSize = size* scaleVec;
            vec3 newCenter = center * scaleVec;
            SetSize(newSize);
            SetCenter(newCenter);
        }

        public void SetSize(vec3 size)
        {
            this.size = size;
            halfSize = size / 2.0f;
        }

        public void SetCenter(vec3 new_center)
        {
            center = new_center;
        }

        public void Translate(vec3 translation)
        {
            center += translation;
        }

        public bool CheckCollision(AABoundingBox box, vec3 offset)
        {
            vec3 newCenter = center + offset;
            vec3 distance = new vec3(Math.Abs(newCenter.x - box.center.x),
                                     Math.Abs(newCenter.y - box.center.y),
                                     Math.Abs(newCenter.z - box.center.z));
            vec3 length = this.halfSize + box.halfSize;

            if ((distance.x <= length.x) &&
                (distance.y <= length.y) &&
                (distance.z <= length.z))
            {
                return true;
            }
            return false;
        }
    }
}
