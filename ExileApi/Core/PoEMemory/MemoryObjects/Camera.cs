using System;
using ExileCore.Shared.Cache;
using GameOffsets;
using SharpDX;

namespace ExileCore.PoEMemory.MemoryObjects
{
    public class Camera : RemoteMemoryObject
    {
        private static Vector2 oldplayerCord;
        private readonly CameraOffsets? _cameraOffsets;
        private readonly CachedValue<CameraOffsets> _cachedValue;

        public Camera()
        {
            _cachedValue = new FrameCache<CameraOffsets>(() => M.Read<CameraOffsets>(Address));

            _cachedValue.OnUpdate += offsets =>
            {
                HalfHeight = offsets.Height * 0.5f;
                HalfWidth = offsets.Width * 0.5f;
            };
        }

        public CameraOffsets CameraOffsets => _cachedValue.Value;
        public int Width => CameraOffsets.Width;
        public int Height => CameraOffsets.Height;
        private float HalfWidth { get; set; }
        private float HalfHeight { get; set; }
        public Vector2 Size => new Vector2(Width, Height);
        public float ZFar => CameraOffsets.ZFar;
        public Vector3 Position => CameraOffsets.Position;
        public string PositionString => Position.ToString();

        //cameraarray 0x17c
        private Matrix Matrix => CameraOffsets.MatrixBytes;

        public unsafe Vector2 WorldToScreen(Vector3 vec /*, Entity Entity*/)
        {
            try
            {
                /*
                var localPlayer = TheGame.IngameState.Data.LocalPlayer;
                var isplayer = localPlayer.Address == Entity.Address && localPlayer.IsValid;
                var isMoving = false;
                if (isplayer) isMoving = Entity.GetComponent<Actor>().isMoving;
*/

                Vector2 result;
                var cord = *(Vector4*) &vec;
                cord.W = 1;
                cord = Vector4.Transform(cord, Matrix);
                cord = Vector4.Divide(cord, cord.W);
                result.X = (cord.X + 1.0f) * HalfWidth;
                result.Y = (1.0f - cord.Y) * HalfHeight;
                /*
                if (!isplayer) return result;
                if (isMoving)
                {
                    if (Math.Abs(oldplayerCord.X - result.X) < 50 || Math.Abs(oldplayerCord.X - result.Y) < 50)
                        result = oldplayerCord;
                    else
                        oldplayerCord = result;
                }
                else oldplayerCord = result;
*/

                return result;
            }
            catch (Exception ex)
            {
                Core.Logger.Error($"Camera WorldToScreen {ex}");
            }

            return Vector2.Zero;
        }
    }
}
