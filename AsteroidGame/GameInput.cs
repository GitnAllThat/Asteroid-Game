using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using _2DLevelCreator;


namespace Asteroid_Game
{
    public class GameInput
    {

        #region Mouse Variables

            public float mouseSize = 0.03f;
            //public BoundingCircle mouseBC = new BoundingCircle(new Transform(), new Motion(), Vector3.Zero, 0.03f, 0, true);


            public MouseState mouseStateCurrent { get; set; }
            public MouseState mouseStatePrevious { get; set; }

            public Vector3 mousePositionCurrent { get; set; }
            public Vector3 mousePositionPrevious { get; set; }

            public Vector3 mouseLeftDownLocation { get; set; }
            public Vector3 mouseRightDownLocation { get; set; }


            public Vector3 mousePositionCurrentProjected { get; set; }
            public Vector3 mousePositionPreviousProjected { get; set; }

            public bool MouseLeftIsDown = false;
            public bool MouseLeftPressed = false;
            public bool MouseLeftReleased = false;
            public bool MouseRightIsDown = false;
            public bool MouseRightPressed = false;
            public bool MouseRightReleased = false;
            public bool isMouseOnScreen = false;
            public bool MouseLeftDownOnXnaWindow = false;

        #endregion

        #region Keyboard Variables

        public KeyboardState keyboardStateCurrent { get; set; }
        public KeyboardState keyboardStatePrevious { get; set; }

        #endregion

        public GameInput()
        {
        }





        #region Mouse Functions

            public void UpdatePreviousMouseState() { mouseStatePrevious = mouseStateCurrent; }
            public void UpdateCurrentMouseState() { mouseStateCurrent = Mouse.GetState(); }

            public Vector3 GetCurrentMousePosition() { return new Vector3(mouseStateCurrent.X, mouseStateCurrent.Y, 0); }
            public Vector3 GetPreviousMousePosition() { return new Vector3(mouseStatePrevious.X, mouseStatePrevious.Y, 0); }

            private bool CheckPressed(ButtonState current, ButtonState previous)
            {
                if (current == ButtonState.Pressed && previous == ButtonState.Released) return true;
                else return false;
            }
            private bool CheckReleased(ButtonState current, ButtonState previous)
            {
                if (current == ButtonState.Released && previous == ButtonState.Pressed) return true;
                else return false;
            }
            private bool CheckIsDown(ButtonState current)
            {
                if (current == ButtonState.Pressed) return true;
                else return false;
            }

        /*
            //This gets the mouse position relative to the control.
            public Vector3 GetMousePos(System.Windows.Forms.Control control, Vector3 coords)
            {
                if (control.Parent == null)
                    return (new Vector3(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y, 0) - coords - new Vector3(control.Left + 7, control.Top + 30, 0));
                else
                    return GetMousePos(control.Parent, coords + new Vector3(control.Left, control.Top, 0));
            }

            public bool IsMouseHoveredOverVertex(Vector3 vertPos)
            {
                if ((this.mouseBC.GetCollisionSolver(VectorHelper.CreateVertexCollider(vertPos, this.xnaWindow.camera.CameraPosition.Z)))[0].distance <= 0)
                    return true;
                return false;
            }


            public void UpdateMouseBoundingCircle()
            {
                this.mouseBC.transform.vPosition = this.mousePositionCurrentProjected;
                float distance = this.xnaWindow.camera.CameraPosition.Z - this.mouseBC.transform.vPosition.Z;
                if (distance == 0) return;
                this.mouseBC.RescaleTo(new Vector3(distance * mouseSize, distance * mouseSize, 1));
            }
        */

        #endregion




        #region Keyboard Functions

            public void UpdatePreviousKeyboardState() { this.keyboardStatePrevious = this.keyboardStateCurrent; }
            public void UpdateCurrentKeyboardState() { this.keyboardStateCurrent = Keyboard.GetState(); }

        #endregion








        public void Update(Camera camera, GraphicsDevice graphicsDevice)
        {
            this.UpdatePreviousMouseState();
            this.UpdatePreviousKeyboardState();
            this.UpdateCurrentMouseState();
            this.UpdateCurrentKeyboardState();


            //Mouse.WindowHandle = this.xnaWindow.Handle;   //Needed for MouseWheel (Windows Forms)

            this.mousePositionPrevious = this.mousePositionCurrent;
            this.mousePositionCurrent = new Vector3(this.mouseStateCurrent.X, this.mouseStateCurrent.Y, 0);

            







            this.mousePositionPreviousProjected = this.mousePositionCurrentProjected;
            this.mousePositionCurrentProjected = this.GetWorldMouseCoords_XYPlane(camera, this.mousePositionCurrent, 0, graphicsDevice.PresentationParameters.BackBufferWidth, graphicsDevice.PresentationParameters.BackBufferHeight); //This needs to be calculated after any camera movement.

            this.MouseLeftPressed = this.CheckPressed(this.mouseStateCurrent.LeftButton, this.mouseStatePrevious.LeftButton);

            this.MouseLeftIsDown = this.CheckIsDown(this.mouseStateCurrent.LeftButton);
            this.MouseRightIsDown = this.CheckIsDown(this.mouseStateCurrent.RightButton);

            this.MouseLeftReleased = this.CheckReleased(this.mouseStateCurrent.LeftButton, this.mouseStatePrevious.LeftButton);




            if (MouseLeftPressed) this.mouseLeftDownLocation = this.mousePositionCurrentProjected;
            if (MouseRightPressed) this.mouseRightDownLocation = this.mousePositionCurrentProjected;

            this.MouseLeftDownOnXnaWindow = (MouseLeftPressed) ? true : false;

            //this.UpdateMouseBoundingCircle();
        }



        /*
        public static void IsMouseOnScreen()
        {
            if ((mouseCurrent.X < Globals.graphics.PreferredBackBufferWidth && mouseCurrent.X > 0) &&
                (mouseCurrent.Y < Globals.graphics.PreferredBackBufferHeight && mouseCurrent.Y > 0))
                GameInput.isMouseOnScreen = true;
            else
                GameInput.isMouseOnScreen = false;
        }
        public static void IsMouseOnScreen(float windowWidth, float windowHeight)
        {
            if ((mouseCurrentPos.X < windowWidth && mouseCurrentPos.X > 0) &&
                (mouseCurrentPos.Y < windowHeight && mouseCurrentPos.Y > 0))
                GameInput.isMouseOnScreen = true;
            else
                GameInput.isMouseOnScreen = false;
        }
        */







        /*
        public static Vector3 ProjectScreenCoordsTo_XYPlane(Vector3 screenCoords, float planeDepth)
        {
            Vector3 worldPos = new Vector3(screenCoords.X, screenCoords.Y, -0.3f);  //Note: the "-0.3f" is the negative of the cams nearplanedistance
            worldPos = Vector3.Transform(worldPos, Globals.camera.InvProjectionMatrix);
            worldPos = Vector3.Transform(worldPos, Globals.camera.InvViewMatrix);
            worldPos = Vector3.Transform(worldPos, Globals.camera.InvWorldMatrix);


            float no = (Globals.camera.CameraPosition.Z - planeDepth) / worldPos.Z;
            worldPos *= -no;
            worldPos.Z = planeDepth;


            return worldPos;
        }
        */


        /// <summary>
        /// Projects the mouse's position from screen space to world coordinates positioned on the xy plane at the desired depth.
        /// Please note that it is based of of the buffer size. May be multiple windows sharing a buffer so window may not represent
        /// the actual size of the buffer therefore caalculations will be incorrect for mouse position
        /// </summary>
        /// 
        /// <param name="mousePos"></param>
        /// <param name="planeDepth"> A planedepth of -100 would project the mouse onto the xy plane at depth -100  ie (?, ?, -100). </param>
        /// <returns> Mouse coords on xy plane </returns>
        /*
        public static Vector3 GetWorldMouseCoords_XYPlane(Vector3 mousePos, float planeDepth)
        {
            //First: Convert mouse coords from screenspace to viewport space.
            //Screen coords go from -1 to +1 for both X and Y
            //Also note that for this case (-1, -1) is top left of screen. So needed to flip y value.
            int window_x = (int)mousePos.X - Globals.graphics.PreferredBackBufferWidth / 2;
            float norm_x = (float)(window_x) / (float)(Globals.graphics.PreferredBackBufferWidth / 2);
            int window_y = (Globals.graphics.PreferredBackBufferHeight - (int)mousePos.Y) - Globals.graphics.PreferredBackBufferHeight / 2;
            float norm_y = (float)(window_y) / (float)(Globals.graphics.PreferredBackBufferHeight / 2);


            Vector3 worldMousePos = new Vector3(norm_x, norm_y, -0.3f);  //Note: the "-0.3f" is the negative of the cams nearplanedistance
            worldMousePos = Vector3.Transform(worldMousePos, Globals.camera.InvProjectionMatrix);
            worldMousePos = Vector3.Transform(worldMousePos, Globals.camera.InvViewMatrix);
            worldMousePos = Vector3.Transform(worldMousePos, Globals.camera.InvWorldMatrix);

            float no = (Globals.camera.CameraPosition.Z - planeDepth) / worldMousePos.Z;
            worldMousePos *= -no;
            worldMousePos += Globals.camera.CameraPosition;
            worldMousePos.Z = +planeDepth;


            return worldMousePos;
        }*/
        public Vector3 GetWorldMouseCoords_XYPlane(Camera camera, Vector3 mousePos, float planeDepth, float BackBufferWidth, float BackBufferHeight)
        {
            //First: Convert mouse coords from screenspace to viewport space.
            //Screen coords go from -1 to +1 for both X and Y
            //Also note that for this case (-1, -1) is top left of screen. So needed to flip y value.
            int window_x = (int)(mousePos.X - (BackBufferWidth / 2));
            float norm_x = (float)(window_x) / (float)(BackBufferWidth / 2);
            int window_y = (int)((BackBufferHeight - mousePos.Y) - (BackBufferHeight / 2));
            float norm_y = (float)(window_y) / (float)(BackBufferHeight / 2);


            Vector3 worldMousePos = new Vector3(norm_x, norm_y, -0.3f);  //Note: the "-0.3f" is the negative of the cams nearplanedistance
            worldMousePos = Vector3.Transform(worldMousePos, camera.InvProjectionMatrix);
            worldMousePos = Vector3.Transform(worldMousePos, camera.InvViewMatrix);
            worldMousePos = Vector3.Transform(worldMousePos, camera.InvWorldMatrix);

            float no = (camera.CameraPosition.Z - planeDepth) / worldMousePos.Z;
            worldMousePos *= -no;
            worldMousePos += camera.CameraPosition;
            worldMousePos.Z = +planeDepth;


            return worldMousePos;
        }
    }
}
