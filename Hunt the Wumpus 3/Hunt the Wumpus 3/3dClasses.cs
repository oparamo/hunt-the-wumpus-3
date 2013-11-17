using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Hunt_the_Wumpus_3 {

    class GameConstants {
        //camera constants
        public const float NearClip = 1.0f;
        public const float FarClip = 1000.0f;
        public const float ViewAngle = 45.0f;

        //player view constants
        public const float Velocity = 0.75f;
        public const float TurnSpeed = 0.025f;
        public const int MaxRange = 98;
    }

    //3d gameobject class
    class GameObject {
        public Model Model { get; set; }
        public Vector3 Position { get; set; }
        public bool IsActive { get; set; }
        public BoundingSphere BoundingSphere { get; set; }

        public GameObject() {
            Model = null;
            Position = Vector3.Zero;
            IsActive = false;
            BoundingSphere = new BoundingSphere();
        }
    }

    class PlayerView : GameObject {
        public float ForwardDirection { get; set; }
        public int MaxRange { get; set; }

        public PlayerView() : base() {
            ForwardDirection = 0.0f;
            MaxRange = GameConstants.MaxRange;
        }
    }

    //3d objects to represent the map
    class MapRep : GameObject {
        public string MapType { get; set; }

        public MapRep() : base() {
            MapType = null;
        }

        public void LoadContent(ContentManager content, string modelName) {
            Model = content.Load<Model>(modelName);
            MapType = modelName;
            Position = Vector3.Down;
        }

        public void Draw(Matrix view, Matrix projection) {
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix translateMatrix = Matrix.CreateTranslation(Position);
            Matrix worldMatrix = translateMatrix;

            foreach (ModelMesh mesh in Model.Meshes) {
                foreach (BasicEffect effect in mesh.Effects) {
                    effect.World =
                        worldMatrix * transforms[mesh.ParentBone.Index];
                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
                mesh.Draw();
            }
        }
    }

    //camera class
    class Camera {
        public Vector3 AvatarHeadOffset { get; set; }
        public Vector3 TargetOffset { get; set; }
        public Matrix ViewMatrix { get; set; }
        public Matrix ProjectionMatrix { get; set; }

        public Camera() {
            AvatarHeadOffset = new Vector3(0, 7, -15);
            TargetOffset = new Vector3(0, 5, 0);
            ViewMatrix = Matrix.Identity;
            ProjectionMatrix = Matrix.Identity;
        }

        public void Update(float avatarYaw, Vector3 position, float aspectRatio) {
            Matrix rotationMatrix = Matrix.CreateRotationY(avatarYaw);

            Vector3 transformedheadOffset =
                Vector3.Transform(AvatarHeadOffset, rotationMatrix);
            Vector3 transformedReference =
                Vector3.Transform(TargetOffset, rotationMatrix);

            Vector3 cameraPosition = position + transformedheadOffset;
            Vector3 cameraTarget = position + transformedReference;

            //Calculate the camera's view and projection 
            //matrices based on current values.
            ViewMatrix =
                Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
            ProjectionMatrix =
                Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(GameConstants.ViewAngle), aspectRatio,
                    GameConstants.NearClip, GameConstants.FarClip);
        }
    }
}
