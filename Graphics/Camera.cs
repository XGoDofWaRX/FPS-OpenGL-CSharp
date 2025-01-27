﻿using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{
    class Camera
    {
        float mAngleX = 0;
        float mAngleY = 0;
        vec3 mDirection;
        vec3 mPosition;
        vec3 mRight;
        vec3 mUp;
        mat4 mViewMatrix;
        mat4 mProjectionMatrix;
        public Camera()
        {
            Reset(0, 0, 5, 0, 0, 0, 0, 1, 0);
            SetProjectionMatrix(45, 4 / 3, 0.1f, 1000);
        }

        public vec3 GetLookDirection()
        {
            return mDirection;
        }

        public vec3 GetRightDirection()
        {
            return mRight;
        }

        public mat4 GetViewMatrix()
        {
            return mViewMatrix;
        }

        public mat4 GetProjectionMatrix()
        {
            return mProjectionMatrix;
        }
        public vec3 GetCameraPosition()
        {
            return mPosition;
        }
        public void Reset(float eyeX, float eyeY, float eyeZ, float centerX, float centerY, float centerZ, float upX, float upY, float upZ)
        {
            vec3 eyePos = new vec3(eyeX, eyeY, eyeZ);
            vec3 centerPos = new vec3(centerX, centerY, centerZ);
            vec3 upVec = new vec3(upX, upY, upZ);

            mPosition = eyePos;
            mDirection = centerPos - mPosition;
            mRight = glm.cross(mDirection, upVec);
            mUp = upVec;
            mUp = glm.normalize(mUp);
            mRight = glm.normalize(mRight);
            mDirection = glm.normalize(mDirection);

            mViewMatrix = glm.lookAt(mPosition, centerPos, mUp);
        }

        public void UpdateViewMatrix()
        {
            mDirection = new vec3((float)(-Math.Cos(mAngleY) * Math.Sin(mAngleX))
                , (float)(Math.Sin(mAngleY))
                , (float)(-Math.Cos(mAngleY) * Math.Cos(mAngleX)));
            mRight = glm.cross(mDirection, new vec3(0, 1, 0));
            mUp = glm.cross(mRight, mDirection);

            vec3 center = mPosition + mDirection;

            mViewMatrix = glm.lookAt(mPosition, center, mUp);
        }
        public void SetProjectionMatrix(float FOV, float aspectRatio, float near, float far)
        {
            mProjectionMatrix = glm.perspective(FOV, aspectRatio, near, far);
        }

        public void Yaw(float angleDegrees)
        {
            mAngleX += angleDegrees;
        }

        public void Pitch(float angleDegrees)
        {
            mAngleY += angleDegrees;
        }

        public bool Walk(float dist, float maxDist)
        {
            if (checkWalk(dist, maxDist))
            {
                float y = mPosition.y;
                mPosition += dist * mDirection;
                mPosition.y = y;
                return true;
            }
            return false;
        }
        public bool Strafe(float dist, float maxDist)
        {
            if (checkStrafe(dist, maxDist))
            {
                float y = mPosition.y;
                mPosition += dist * mRight;
                mPosition.y = y;
                return true;
            }
            return false;
        }
        public bool Fly(float dist, float maxDist)
        {
            if (checkFly(dist, maxDist))
            {
                mPosition += dist * mUp;
                return true;
            }
            return false;
        }

        public bool checkWalk(float dist, float maxDist)
        {
            if((mPosition + (dist * mDirection)).x > (maxDist - 2.0f)
                || (mPosition + (dist * mDirection)).x < (-maxDist + 2.0f)
                || (mPosition + (dist * mDirection)).y > (maxDist - 2.0f)
                || (mPosition + (dist * mDirection)).y < (2.0f)
                || (mPosition + (dist * mDirection)).z > (maxDist - 2.0f)
                || (mPosition + (dist * mDirection)).z < (-maxDist + 2.0f))
            {
                return false;
            }
            return true;
        }
        public bool checkStrafe(float dist, float maxDist)
        {
            if ((mPosition + (dist * mRight)).x > (maxDist - 2.0f)
                || (mPosition + (dist * mRight)).x < (-maxDist + 2.0f)
                || (mPosition + (dist * mRight)).y > (maxDist - 2.0f)
                || (mPosition + (dist * mRight)).y < (2.0f)
                || (mPosition + (dist * mRight)).z > (maxDist - 2.0f)
                || (mPosition + (dist * mRight)).z < (-maxDist + 2.0f))
            {
                return false;
            }
            return true;
        }
        public bool checkFly(float dist, float maxDist)
        {
            if ((mPosition + (dist * mUp)).y > (maxDist - 2.0f)
                || (mPosition + (dist * mUp)).y < (2.0f))
            {
                return false;
            }
            return true;
        }
    }
}
