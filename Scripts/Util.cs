/**
 * Util.cs
 * Various conversion functions.
 *
 * by muskit
 * July 26, 2022
 **/

using System;

using Godot;

using WacK.Configuration;

namespace WacK
{
    public static class Util
    {
        public static float Seg2Rad(float seg)
        {
			return Mathf.DegToRad(6f * seg);
		}
        public static float Rad2Seg(float angle)
        {
            return Mathf.RadToDeg(angle / 6f);
        }

        public static int InterpInt(int a, int b, float ratio)
        {
            if (a == 0 && b == 0)
                return 0;
            return (int)Math.Round(a + (b - a) * ratio);
        }

        public static float InterpFloat(float a, float b, float ratio)
        {
            if (a == 0 && b == 0)
                return 0;
            return a + (b - a) * ratio;
        }

        // Returns an equivalent destination angle that's closest to the origin.
        public static float NearestAngle(float origin, float destination)
        {
            float result = destination;

            float plus = destination + 2f * Mathf.Pi;
            float minus = destination - 2f * Mathf.Pi;
            float minusDelta = Mathf.Abs(minus - origin);
            float normDelta = Mathf.Abs(destination - origin);
            float plusDelta = Mathf.Abs(plus - origin);
            if (plusDelta < normDelta)
                result = plus;
            if (minusDelta < normDelta)
                result = minus;

            return result;
        }

        // Return an equivalent minute that's closest to the origin.
        public static float NearestMinute(int origin, int destination)
        {
            int result = destination % 60;

            int plus = destination + 60;
            int minus = destination - 60;
            int minusDelta = Math.Abs(minus - origin);
            int normDelta = Math.Abs(destination - origin);
            int plusDelta = Math.Abs(plus - origin);
            if (plusDelta < normDelta)
                result = plus;
            if (minusDelta < normDelta)
                result = minus;

            return result;
        }

        public static float ScreenPixelToRad(Vector2 pos)
        {
            var resolution = DisplayServer.WindowGetSize();
            var origin = new Vector2(resolution.X / 2 - 1, resolution.Y / 2 - 1);

            return Mathf.Atan2(pos.Y - origin.Y, pos.X - origin.X);
        }

        public static int TouchPosToSegmentInt(Vector2 pos, Vector2 touchResolution)
        {
            var origin = new Vector2(touchResolution.X / 2 - 1, touchResolution.Y / 2 - 1);
            var angle = Mathf.Atan2(pos.Y - origin.Y, pos.X - origin.X);

            if (angle > 0)
                angle = 2f * Mathf.Pi - angle;

            return Mathf.FloorToInt(Mathf.Abs(angle) / 2f * Mathf.Pi * 60) % 60;
        }

        public static int ScreenPixelToSegmentInt(Vector2 pos)
        {
            var angle = ScreenPixelToRad(pos);
            if (angle > 0)
                angle = 2f * Mathf.Pi - angle;

            return Mathf.FloorToInt(Mathf.Abs(angle) / 2f * Mathf.Pi * 60) % 60;
        }

        public static float NoteTime(int measure, int beat, float tempo, int beatsPerMeasure)
        {
            if (tempo == 0) return 0; // avoid divide by 0

            return 60f / tempo * beatsPerMeasure * ((float)measure + (float)beat / 1920f);
        }

        public static float NotePosition(int measure, int beat, float tempo, int beatsPerMeasure)
        {
            if (tempo == 0) return 0; // avoid divide by 0
            return TimeToPosition(60f / tempo * beatsPerMeasure * ((float)measure + (float)beat / 1920f));
        }

        public static float TimeToPosition(float time)
        {
            return time * PlaySettings.playSpeedMultiplier * Constants.SCROLL_MULT;
        }

        public static float PositionToTime(float pos)
        {
            return pos / PlaySettings.playSpeedMultiplier / Constants.SCROLL_MULT;
        }

        public static string DifficultyValueToString(float diffPoint)
        {
            return Mathf.FloorToInt(diffPoint).ToString() + (diffPoint > Mathf.Floor(diffPoint) ? "+" : "");
        }
    }
}