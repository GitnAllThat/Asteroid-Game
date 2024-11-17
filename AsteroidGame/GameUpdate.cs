using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


using _2d_Objects;

namespace Asteroid_Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public partial class AsteroidGame : Microsoft.Xna.Framework.Game
    {
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            timeDifference = (float)TargetElapsedTime.TotalSeconds;


            //Globals.timeDifference *= 0.1f;
            gameInput.Update(camera, this.GraphicsDevice);  //reimplement Check This 


            camera.CameraMovement(gameInput, this);

            //UpdateGravityPerFrame();

            gameTimeElapsed += timeDifference;



            Vector3 mouseCurrent = gameInput.mousePositionCurrentProjected;
            Vector3 mousePrevious = gameInput.mousePositionPreviousProjected;

            //if ((Globals.gameInput.mouseStatePrevious.ScrollWheelValue != Globals.gameInput.mouseStateCurrent.ScrollWheelValue))
            if (gameInput.keyboardStateCurrent.IsKeyDown(Keys.LeftAlt))
            {
                int bob =0;
            }


            if (!gameOver)
            {

                //Just gets rid of pointless touch strokes. i.e ones with no or little length.
                if (gameInput.MouseLeftReleased)
                {
                    if ((Last_ListOfRL_Count() > 0) && (Last_RL().halfLineVector.Length() < 0.05f))
                    {
                        Last_ListOfRL().RemoveAt(Last_ListOfRL_Count() - 1);
                        //_touchScreenLine.RemoveAt(i);
                    }
                }


                //Extends the users touch stroke. 
                if (gameInput.MouseLeftPressed)  //This handles new touch press. i.e a new line
                {
                    _touchScreenLine.Add(new List<RainbowLine>());
                    Last_ListOfRL().Add(new RainbowLine(mouseCurrent, mouseCurrent, rainbowColours.GetPrevColour(), rainbowColours.GetNextColour(), rainbowColours.GetCurrent(), rainbowColours.GetNextColour(), lineThickness));
                }

                if (gameInput.MouseLeftIsDown)
                {
                    if (Last_ListOfRL_Count() > 0 && Last_RL().halfLineVector.Length() < minLineLength)
                    {
                        lineUsed -= Last_RL().halfLineVector.Length();
                        rainbowColours.GetPrevColour(); //Stops Rave Flashing
                        Last_ListOfRL().Add(new RainbowLine(Last_RL().position - Last_RL().halfLineVector, mouseCurrent, rainbowColours.GetPrevColour(), rainbowColours.GetNextColour(), rainbowColours.GetCurrent(), rainbowColours.GetNextColour(), lineThickness));
                        Last_ListOfRL().RemoveAt(Last_ListOfRL_Count() - 2);
                        lineUsed += Last_RL().halfLineVector.Length();
                    }
                    else
                    {
                        Last_ListOfRL().Add(new RainbowLine(mousePrevious, mouseCurrent, rainbowColours.GetPrevColour(), rainbowColours.GetNextColour(), rainbowColours.GetCurrent(), rainbowColours.GetNextColour(), lineThickness));
                        lineUsed += Last_RL().halfLineVector.Length();
                    }
                    FixLineTearing();
                }





                //Calculate total length
                touchStrokeLength = 0;
                for (int i = 0; i < _touchScreenLine.Count; ++i)
                {
                    for (int j = 0; j < _touchScreenLine[i].Count; j++)
                    {
                        touchStrokeLength += _touchScreenLine[i][j].halfLineVector.Length(); // Should be ...Length() * 2 because linevector is omly half the line. Doesnt matter though
                    }
                }

                //Shorten touchStroke if above its maximum length
                if (touchStrokeLength > touchStrokeLengthMax)
                {
                    float neededToShorten = touchStrokeLength - touchStrokeLengthMax;

                    for (int i = 0; i < _touchScreenLine.Count; ++i)
                    {
                        for (int j = 0; j < _touchScreenLine[i].Count; ++j)
                        {
                            float length = _touchScreenLine[i][j].halfLineVector.Length();
                            if (length <= neededToShorten)
                            {
                                _touchScreenLine[i].RemoveAt(j);
                                --j;
                                neededToShorten -= length;
                                if (_touchScreenLine[i].Count == 0)
                                {
                                    _touchScreenLine.RemoveAt(i);
                                    --i;
                                    if (i < 0) break;

                                }
                            }
                            else
                            {

                                _touchScreenLine[i][j].ShortenFromBack(neededToShorten / length);

                                i = _touchScreenLine.Count; // Will Break the i for loop
                                break;                      // Breaks the j loop
                            }
                        }
                    }
                }































                //Checks for collisions between blackholes. Will alter their positions as if they bumped into each other.
                for (int i = 0, iMax = listBlackHoles.Count; i < iMax; ++i)
                {
                    for (int j = i + 1, jMax = listBlackHoles.Count; j < jMax; ++j)
                    {
                        listBlackHoles[i].BlackHoleVsBlackHoleCollision(listBlackHoles[j]);
                    }
                }

                //This will update blackholes
                for (int i = 0, iMax = listBlackHoles.Count; i < iMax; ++i) listBlackHoles[i].Update(timeDifference);






                //Handle Creation of Projectiles
                TimeTillNextProjectile -= timeDifference;
                if (TimeTillNextProjectile <= 0)
                {
                    TimeTillNextProjectile = randNo.Next((int)(TimeTillNextProjectileMin * 1000), (int)(TimeTillNextProjectileMax * 1000)) * 0.001f;
                    CreateNewProjectile();
                }
                if (lineUsed > lineLengthBeforeAsteroid)
                {
                    lineUsed = 0;
                    CreateNewProjectile();
                }


                //Handle Projectile Collisions
                for (int i = listProjectiles.Count - 1; i >= 0; --i)                //Reverse traverse as may lose a projectile to a black hole.
                {
                    ProjectileVsEarth(listProjectiles[i]);                          //Detect Earths destruction
                    KeepProjectileInBounds(listProjectiles[i]);                     //Reflect projectile off of screen boundarys.
                    ProjectileVsRainbowLine(listProjectiles[i]);                    //Reflect projectile off of Rainbow lines.
                    if (ProjectileVsBlackHole(listProjectiles[i]))                  //Detect if meteor hits a blackhole.
                    {
                        listProjectiles.RemoveAt(i);                                //Destroy meteor
                        score += 1;                                                 //Increase score by 1
                    }
                }


                //Update Projectiles Velocity and rotation
                for (int i = 0, iMax = listProjectiles.Count; i < iMax; ++i)
                {
                    listProjectiles[i].Update(timeDifference);
                }



                //Need to add in more badguys as the game progresses
                //Make a Spawing animation so player knows there will be a blackhole spawning soon(Can aim up shots preemptivly)


                createBlackHoleDelay -= timeDifference;
                if (listBlackHoles.Count < maxNoBlackHoles && createBlackHoleDelay <= 0)
                {
                    createBlackHoleDelay = randNo.Next(createBlackHoleDelayTimeMin, createBlackHoleDelayTimeMax);


                    int randInt = randNo.Next(0, 4);
                    float blackHoleBoundsX = 0.5f * (xBound - blackHoleSize);
                    float blackHoleBoundsY = 0.5f * (yBound - blackHoleSize);
                    float x = blackHoleBoundsX;
                    float y = blackHoleBoundsY;
                    if (randInt == 1) { x *= -1; }
                    if (randInt == 2) { x *= -1; y *= -1; }
                    if (randInt == 3) { y *= -1; }

                    randInt = randNo.Next(0, 2);
                    float speed = randNo.Next((int)(blackHoleSpeedMin * 1000000), (int)(blackHoleSpeedMax * 1000000)) * 0.000001f;
                    if (randInt == 0)
                    {
                        x *= randNo.Next(0, 100) * 0.01f;
                        int direction = 1;
                        if (randNo.Next(0, 2) == 1) direction = -1;
                        listBlackHoles.Add(new BlackHole(new Vector3(x, y, 0), new Vector3(speed * direction, 0, 0), speed, blackHoleSize, blackHoleBoundsX, blackHoleBoundsY, thing2D_BlackHole));
                    }
                    else
                    {
                        y *= randNo.Next(0, 100) * 0.01f;
                        int direction = 1;
                        if (randNo.Next(0, 2) == 1) direction = -1;
                        listBlackHoles.Add(new BlackHole(new Vector3(x, y, 0), new Vector3(0, speed * direction, 0), speed, blackHoleSize, blackHoleBoundsX, blackHoleBoundsY, thing2D_BlackHole));
                    }
                }


                //Make Badguy waft away lines that are super close to them

                //Need to reduce auto aim as the game progresses





                //Auto aim meteors
                //Gets a number which represents how much the meteor is traveling in the direction of the black hole (from -1 to 1)
                //If this number is very much in the direction then the velocity will be altered to make it travek even more so.
                //This will be based off of the set accuracy modifier.
                //Also modifies the velocity when the meteor is near the black hole. Pulls it towards it.

                for (int i = 0, iMax = listProjectiles.Count; i < iMax; ++i)
                {
                    for (int j = 0, jMax = listBlackHoles.Count; j < jMax; ++j)
                    {
                        Vector3 bhPos = listBlackHoles[j].transform.vPosition;
                        Vector3 mPos = listProjectiles[i].transform.vPosition;
                        Vector3 mVel = listProjectiles[i].velocity;
                        mVel.Normalize();


                        Vector3 dirFromMetToBlack = bhPos - mPos;
                        float distance = dirFromMetToBlack.Length();
                        dirFromMetToBlack.Normalize();

                        float dot = Vector3.Dot(dirFromMetToBlack, mVel);
                        if (dot > 0.94f)
                        {
                            float speed = listProjectiles[i].velocity.Length();
                            dot *= autoAim * 0.01f;

                            listProjectiles[i].velocity = (dirFromMetToBlack * dot) + (mVel * (1 - dot));
                            listProjectiles[i].velocity.Normalize();
                            listProjectiles[i].velocity *= speed;
                        }


                        //Simulate gravitational pull.
                        float PullDistStart = listBlackHoles[j].transform.vScale.X * gravitationalPull;
                        if (distance < PullDistStart)
                        {
                            distance *= 0.4f;
                            listProjectiles[i].velocity += (dirFromMetToBlack * (PullDistStart - distance)) * timeDifference;
                        }
                    }
                }











                for (int i = 0; i < listDyingRainbowLines.Count; ++i)
                {
                    listDyingRainbowLines[i].Update(timeDifference);
                }

                for (int i = 0; i < listPlusOne.Count; ++i)
                {
                    listPlusOne[i].Update(timeDifference);
                    if (listPlusOne[i].life <= 0)
                    {
                        listPlusOne.RemoveAt(i);
                    }
                }


            }
            else    //Game has ended Code
            {
                //Update Projectiles and blackhole rotation. Makes it look like the game hasnt just crashed.
                for (int i = 0, iMax = listProjectiles.Count; i < iMax; ++i) listProjectiles[i].UpdateRotation(timeDifference);
                for (int i = 0, iMax = listBlackHoles.Count; i < iMax; ++i) listBlackHoles[i].UpdateRotation(timeDifference);

                delayBeforeGameOverScreen -= timeDifference;

                if (delayBeforeGameOverScreen < 0)
                {
                    ResetGame();
                }
            }


            IncreaseDifficulty();

            //Make earth explode when meteor hits it

            //Make gameplay harder as game progresses


            //Make Outline on Rainbow Line

            //Create Own Drawings

            //Different Asteroid Colours and point score

            //Create an ASSteroid

            //Convert to Phone App
            //Make Black holes suck in rainbow lines ... Meh



            //this.XnaWindow.camera.FocusOnPosition(new Vector3(0, 0, 0), this.XnaWindow, 0);
            //eli.CameraFollow(this.XnaWindow);


            base.Update(gameTime);
        }


        public void IncreaseDifficulty()
        {
            if (lastGameScore != score)
            {
                bool hasDifficultyIncreased = true;

                int randInt = randNo.Next(0, 4);

                if (randInt == 0)
                {
                    //Lower Auto Aim
                    autoAim -= timeDifference * 0.1f;
                    if (autoAim < 0) { autoAim = 0; hasDifficultyIncreased = false; }
                }
                else if (randInt == 1)
                {
                    //Alter lineLength Before Asteroid Spawns
                    lineLengthBeforeAsteroid -= timeDifference;
                    if (lineLengthBeforeAsteroid < MinLineLengthBeforeAsteroid) { lineLengthBeforeAsteroid = MinLineLengthBeforeAsteroid; hasDifficultyIncreased = false; }
                }
                else if (randInt == 2)
                {
                    //Alter speed
                    if (projectileSpeedMax < MaximumSpeed)  //Increase max speed if it hasnt reached the maximum
                    {
                        projectileSpeedMax += timeDifference;
                        if (projectileSpeedMax > MaximumSpeed) { projectileSpeedMax = MaximumSpeed; hasDifficultyIncreased = false; }
                    }
                    else                                    //Increase the min speed if maximum has been reached.
                    {
                        projectileSpeedMin += timeDifference;
                        if (projectileSpeedMin > MaximumSpeed) { projectileSpeedMin = MaximumSpeed; hasDifficultyIncreased = false; }
                    }
                }
                else if (randInt == 3)
                {
                    //Alter Time till projectile is created
                    if (TimeTillNextProjectileMin > 1)  //Increase max speed if it hasnt reached the maximum
                    {
                        TimeTillNextProjectileMin -= timeDifference * 30;
                        if (TimeTillNextProjectileMin < 1) { TimeTillNextProjectileMin = 1; hasDifficultyIncreased = false; }
                    }
                    else                                    //Increase the min speed if maximum has been reached.
                    {
                        TimeTillNextProjectileMax += timeDifference * 30;
                        if (TimeTillNextProjectileMax < 1) { TimeTillNextProjectileMax = 1; hasDifficultyIncreased = false; }
                    }
                }





                if (hasDifficultyIncreased) lastGameScore = score;
            }
        }








        public void FixLineTearing()
        {
            
            if (Last_ListOfRL_Count() > 1)
            {
                int countMax = Last_ListOfRL_Count() - 1;
                int countBefore = Last_ListOfRL_Count() - 2;

                _touchScreenLine[_touchScreenLine.Count - 1][countMax].vpcArray[0].Position = _touchScreenLine[_touchScreenLine.Count - 1][countBefore].vpcArray[2].Position;
                _touchScreenLine[_touchScreenLine.Count - 1][countMax].vpcArray[1].Position = _touchScreenLine[_touchScreenLine.Count - 1][countBefore].vpcArray[3].Position;
            }

        }
        public List<RainbowLine> Last_ListOfRL()
        {
            return _touchScreenLine[_touchScreenLine.Count - 1];
        }
        public RainbowLine Last_RL()
        {
            return _touchScreenLine[_touchScreenLine.Count - 1][_touchScreenLine[_touchScreenLine.Count - 1].Count - 1];
        }
        public int Last_ListOfRL_Count()
        {
            return _touchScreenLine[_touchScreenLine.Count - 1].Count;
        }

        public Vector3 GetRandomPositionOnBoundary(float boundaryX, float boundaryY)
        {
            Vector3 pos = new Vector3(boundaryX, boundaryY, 0);

            int randInt = randNo.Next(0, 4);

            if (randInt == 1) { pos.X *= -1; }
            if (randInt == 2) { pos.X *= -1; pos.Y *= -1; }
            if (randInt == 3) { pos.Y *= -1; }

            randInt = randNo.Next(0, 2);

            if (randInt == 0)
            { pos.X *= randNo.Next(0, 100) * 0.01f; }
            else
            { pos.Y *= randNo.Next(0, 100) * 0.01f; }

            return pos;
        }

        public void KeepProjectileInBounds(Projectile projectile)
        {
            if (projectile.transform.vPosition.X > halfBoundsX && projectile.velocity.X > 0) projectile.velocity.X *= -1;
            if (projectile.transform.vPosition.X < -halfBoundsX && projectile.velocity.X < 0) projectile.velocity.X *= -1;
            if (projectile.transform.vPosition.Y > halfBoundsY && projectile.velocity.Y > 0) projectile.velocity.Y *= -1;
            if (projectile.transform.vPosition.Y < -halfBoundsY && projectile.velocity.Y < 0) projectile.velocity.Y *= -1;
        }

        public void CreateNewProjectile()
        {
            Vector3 position = GetRandomPositionOnBoundary(((xBound) * 0.5f) + projectileSize, ((yBound) * 0.5f) + projectileSize);
            float speed = randNo.Next((int)(projectileSpeedMin * 1000000), (int)(projectileSpeedMax * 1000000)) * 0.000001f;
            listProjectiles.Add(new Projectile(position, projectileSize, -(position * (speed / position.Length())), thing2D_Meteor));
        }




        public void ProjectileVsRainbowLine(Projectile projectile)
        {
            float invTimeDifference = 1 / timeDifference;

            for (int i = 0; i < _touchScreenLine.Count; ++i)
            {
                for (int j = 0; j < _touchScreenLine[i].Count; ++j)
                {
                    CollisionDataBasic cdb = Collision_Methods.GetSolver_PointVsLine(projectile.transform.vPosition, _touchScreenLine[i][j]);

                    float relNv = Vector3.Dot(projectile.velocity, cdb.vNormal);          // get normal velocity
                    float remove = relNv + Math.Max(cdb.distance * invTimeDifference, 0);

                    if (remove < 1)
                    {
                        Vector3 reflection = projectile.velocity - 2 * cdb.vNormal * Vector3.Dot(cdb.vNormal, projectile.velocity);
                        projectile.velocity = reflection;

                        SplitTheRainbow(_touchScreenLine[i], j, i, cdb);
                        ++i; // Need to increment because SplitTheRainbow adds an extra Line
                    }
                }
            }
        }

        public void SplitTheRainbow(List<RainbowLine> lineToSplit, int index, int touchScreenLineIndex, CollisionDataBasic cdb)
        {
            float lineDecayModifier = 1;
            for (int i = 0; i < listDyingRainbowLines.Count; ++i)
            {
                if (lineToSplit == listDyingRainbowLines[i].rainbowLine)
                {
                    lineDecayModifier += listDyingRainbowLines[i].speedModifier * 2;
                    break;
                }
            }

            List<RainbowLine> newListRainbowLine = new List<RainbowLine>();

            lineToSplit.Insert(index, new RainbowLine(lineToSplit[index]));

            for (int i = index + 1; i < lineToSplit.Count; )
            {
                newListRainbowLine.Add(lineToSplit[i]);
                lineToSplit.RemoveAt(i);
            }

            float length = newListRainbowLine[0].halfLineVector.Length() * 2;


            //Shorten newListRainbowLine's  back
            Vector3 from = newListRainbowLine[0].position - newListRainbowLine[0].halfLineVector;
            float shortenPercentage = (from - cdb.vContactPoint).Length() / length;

            //Shorten lineToSplit's  front
            lineToSplit[index].ShortenFromFront(1 - shortenPercentage);
            listDyingRainbowLines.Add(new DyingRainbowLine(lineToSplit, false, lineDecayModifier));



            newListRainbowLine[0].ShortenFromBack(shortenPercentage);
            _touchScreenLine.Insert(touchScreenLineIndex + 1, newListRainbowLine);
            listDyingRainbowLines.Add(new DyingRainbowLine(newListRainbowLine, true, lineDecayModifier));


            //Fix Line Tearing
            if (index != 0)
            {
                lineToSplit[index].vpcArray[0].Position = lineToSplit[index - 1].vpcArray[2].Position;
                lineToSplit[index].vpcArray[1].Position = lineToSplit[index - 1].vpcArray[3].Position;
            }
            if (newListRainbowLine.Count - 1 > 1)
            {
                newListRainbowLine[1].vpcArray[0].Position = newListRainbowLine[0].vpcArray[2].Position;
                newListRainbowLine[1].vpcArray[1].Position = newListRainbowLine[0].vpcArray[3].Position;
            }
        }



        public bool ProjectileVsBlackHole(Projectile projectile)
        {
            float distance;
            for (int i = 0; i < listBlackHoles.Count; ++i)
            {
                distance = (projectile.transform.vPosition - listBlackHoles[i].transform.vPosition).Length() - (listBlackHoles[i].transform.vScale.X * 0.2f);
                if (distance < 0)
                {
                    listPlusOne.Add(new PlusOne(thing2D_PlusOne, listBlackHoles[i].transform.vPosition, plusOneSize));
                    return true;
                }

                //Alter Meteor size the closer the meteor is to the black hole
                //distance = 1 / blackHoleSize; //Changes the distance relative to the black hole's size
                if (distance < blackHoleSize)
                {
                    float scaler = 1 / blackHoleSize;   //Want the distance modified relative to the blackhole' size
                    projectile.transform.vScale = new Vector3((float)Math.Sqrt(distance * scaler) * projectileSize, (float)Math.Sqrt(distance * scaler) * projectileSize, 1);

                    if (projectile.transform.vScale.X > projectileSize)
                        projectile.transform.vScale = new Vector3(projectileSize, projectileSize, 1);
                }
            }
            return false;
        }

        public void ProjectileVsEarth(Projectile projectile)
        {
            float distance = projectile.transform.vPosition.Length() - (earth.transform.vScale.X * 0.5f) - (projectile.transform.vScale.X * 0.25f);
            if (distance < 0)
            {
                gameOver = true;
                earth.impactRotation = MathHelper.ToRadians(Collision_Methods.GetAngle(projectile.transform.vPosition)) - MathHelper.ToRadians(90);
            }





            Vector3 vDistance = -projectile.transform.vPosition;
            //distance = dirFromMetToBlack.Length();
            vDistance.Normalize();

            float PullDistStart = earth.transform.vScale.X * gravitationalPull;
            if (distance < PullDistStart)
            {
                distance *= 0.9f;
                projectile.velocity += (vDistance * (PullDistStart - distance)) * timeDifference;
            }


            /*
            //Simulate gravitational pull.
            if (distance < earth.transform.vScale.X * earthGravitationalPull)
            {
                distance = projectile.transform.vPosition.Length();
                Vector3 v = projectile.transform.vPosition; v.Normalize();
                projectile.velocity -= (v * ((earth.transform.vScale.X * projectile.transform.vScale.X) / (distance * distance))) * Globals.timeDifference;
            }
            */
        }

    }
}
