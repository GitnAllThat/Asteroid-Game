using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;



using System.IO;


namespace _2DLevelCreator
{
    public class Transform
    {
        #region DataMembers

        public Vector3 vPosition;
        public float zRotation { get; set; }
        public Vector3 vScale;

        #endregion

        // Default Constructor
        public Transform()
        {
            this.vPosition = new Vector3(0, 0, 0);
            this.zRotation = 0;
            this.vScale = new Vector3(1, 1, 1);
        }

        // Full Constructor
        public Transform(Vector3 vPosition, float zRotation, Vector3 vScale)
        {
            this.vPosition = vPosition;
            this.zRotation = zRotation;
            this.vScale = vScale;
        }

        // Copy constructor.
        public Transform(Transform transform)
        {
            this.vPosition = transform.vPosition;
            this.zRotation = transform.zRotation;
            this.vScale = transform.vScale;
        }
    }


    public class Motion
    {
        #region DataMembers

        public Vector3 vVelocityPS;
        public Vector3 vVelocityPS_AntiPenetration;

        public float zRotPS { get; set; }
        public float zRotPS_AntiPenetration { get; set; }

        #endregion

        public Motion()
        {
            this.vVelocityPS = new Vector3(0, 0, 0);
            this.vVelocityPS_AntiPenetration = new Vector3(0, 0, 0);
            this.zRotPS = 0;
            this.zRotPS_AntiPenetration = 0;
        }

        public Motion(Vector3 vVelocityPS, Vector3 vVelocityPS_AntiPenetration, float zRotPS, float zRotPS_AntiPenetration)
        {
            this.vVelocityPS = vVelocityPS;
            this.vVelocityPS_AntiPenetration = vVelocityPS_AntiPenetration;
            this.zRotPS = zRotPS;
            this.zRotPS_AntiPenetration = zRotPS_AntiPenetration;
        }

        public Motion(Motion motion)
        {
            this.vVelocityPS = motion.vVelocityPS;
            this.vVelocityPS_AntiPenetration = motion.vVelocityPS_AntiPenetration;
            this.zRotPS = motion.zRotPS;
            this.zRotPS_AntiPenetration = motion.zRotPS_AntiPenetration;
        }
    }
}
