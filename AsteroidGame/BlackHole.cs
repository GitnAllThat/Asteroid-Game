using System;
using Microsoft.Xna.Framework;
using Things;
using _2DLevelCreator;

namespace Asteroid_Game
{
    public class BlackHole
    {
        public Transform transform;
        public Vector3 velocity;
        public float speed;
        public float rotationSpeed = 1;
        public float boundsX;
        public float boundsY;
        Thing2D thing2D;

        public BlackHole(Vector3 position, Vector3 velocity, float speed, float size, float boundsX, float boundsY, Thing2D thing2D)
        {
            this.transform = new Transform(position, 0, new Vector3(size, size, 1));
            this.velocity = velocity;
            this.speed = Math.Abs(speed);   //Has to be positive;
            this.boundsX = boundsX;
            this.boundsY = boundsY;
            this.thing2D = thing2D;
        }

        public void Update(float timeDifference)
        {
            this.UpdateRotation(timeDifference);
            this.Movement(timeDifference);
            //Shoot(timeDifference);
        }

        public void UpdateRotation(float timeDifference)
        {
            this.transform.zRotation -= (rotationSpeed * timeDifference);
        }


        /*
        public void Shoot(float timeDifference)
        {
            this.timeTillShoot -= timeDifference;
            if (this.timeTillShoot < 0)
            {
                this.timeTillShoot = (new Random().Next(this.shootDelay, this.shootDelay + 5));


                Vector3 normal = -this.transform.vPosition;
                normal.Normalize();
                normal *= this.transform.vScale.X / (normal).Length();

                Vector3 vel = -this.transform.vPosition - normal;
                vel *= this.speed/(vel).Length();


                this.listProjectiles.Add(new Projectile(this.transform.vPosition + normal, vel, new Thing2D("Meteor_Thing2D", Material.GetUniqueIdentifierByName("mat_Meteor"), new VertexPositionTextureArray(), new List<Thing2D>())));
            }
        }
        */
        

        public void BlackHoleVsBlackHoleCollision(BlackHole otherBlackHole)
        {
            Vector3 dist = this.transform.vPosition - otherBlackHole.transform.vPosition;

            float distance = dist.Length();

            distance -= (this.transform.vScale.X + otherBlackHole.transform.vScale.X) * 0.5f;


            if (distance < 0)
            {

                Vector3 Normal = otherBlackHole.transform.vPosition - this.transform.vPosition;
                Normal.Normalize();
                float nvDot = Vector3.Dot(Normal, this.velocity);
                if (nvDot >= 0)
                {
                    this.velocity *= -1;
                }
                nvDot = Vector3.Dot(-Normal, otherBlackHole.velocity);
                if (nvDot >= 0)
                {
                    otherBlackHole.velocity *= -1;
                }
            }
        }

        


        public void Movement(float timeDifference)
        {
            this.transform.vPosition += this.velocity * timeDifference;

            if (this.transform.vPosition.X > boundsX)
            {
                this.transform.vPosition.X = boundsX;
                if (this.transform.vPosition.Y == boundsY)
                {
                    this.velocity = new Vector3(0, -speed, 0);
                }
                else if (this.transform.vPosition.Y == -boundsY)
                {
                    this.velocity = new Vector3(0, speed, 0);
                }
            }

            if (this.transform.vPosition.X < -boundsX)
            {
                this.transform.vPosition.X = -boundsX;
                if (this.transform.vPosition.Y == boundsY)
                {
                    this.velocity = new Vector3(0, -speed, 0);
                }
                else if (this.transform.vPosition.Y == -boundsY)
                {
                    this.velocity = new Vector3(0, speed, 0);
                }
            }


            if (this.transform.vPosition.Y > boundsY)
            {
                this.transform.vPosition.Y = boundsY;
                if (this.transform.vPosition.X == boundsX)
                {
                    this.velocity = new Vector3(-speed, 0, 0);
                }
                else if (this.transform.vPosition.X == -boundsX)
                {
                    this.velocity = new Vector3(speed, 0, 0);
                }
            }


            if (this.transform.vPosition.Y < -boundsY)
            {
                this.transform.vPosition.Y = -boundsY;
                if (this.transform.vPosition.X == boundsX)
                {
                    this.velocity = new Vector3(-speed, 0, 0);
                }
                else if (this.transform.vPosition.X == -boundsX)
                {
                    this.velocity = new Vector3(speed, 0, 0);
                }
            }
        }

        public void Draw(AsteroidGame game)
        {
            Transform t = new Transform(this.transform.vPosition, this.transform.zRotation, this.transform.vScale);
            this.thing2D.Draw(game, t, Matrix.CreateRotationZ(this.transform.zRotation));
        }

    }
}
