using UnityEngine;

namespace Boxfriend.Utils
{

    /// <summary>
    /// Class to draw debug information such as physics2d casts
    /// </summary>
    public static class PhysicsCastDebug
    {
        /// <summary>
        /// Casts a Physics2D BoxCast with debug lines drawn
        /// </summary>
        public static RaycastHit2D BoxCast(Vector2 origin,
            Vector2 size,
            float angle,
            Vector2 direction,
            float distance = 0,
            int layerMask = Physics2D.AllLayers,
            float minDepth = -Mathf.Infinity,
            float maxDepth = Mathf.Infinity)
        {
            var hit = Physics2D.BoxCast(origin, size, angle, direction, distance, layerMask, minDepth, maxDepth);

            //Setting up the points to draw the origin box and end box
            var points = new Vector2[8];
            var width = size.x * 0.5f;
            var height = size.y * 0.5f;
            
            points[0] = new Vector2(-width, height); //Upper left corner
            points[1] = new Vector2(width, height); //Upper right corner
            points[2] = new Vector2(width, -height); //Lower right corner
            points[3] = new Vector2(-width, -height); //Lower left corner

            //Calculates origin box corners using provided angle and origin point
            var q = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
            for (var i = 0; i < 4; i++)
            {
                points[i] = q * points[i];
                points[i] += origin;
            }
            
            //Calculates end points using origin box points and provided distance
            var realDistance = direction.normalized * distance;
            for (var i = 0; i < 4; i++)
            {
                points[i + 4] = points[i] + realDistance;
            }

            //Draw hit normal if a hit was detected
            if (hit) Debug.DrawLine(hit.point, hit.point + hit.normal.normalized*0.2f, Color.yellow);
            
            //Draw boxes
            var color = hit ? Color.green : Color.red;
            for (var i = 0; i < 4; i++)
            {
                var j = i == 3 ? 0 : i + 1;
                //Draws origin box using first 4 points
                Debug.DrawLine(points[i],points[j], color);
            }

            //Exit early if distance is 0, don't need to draw end position or translation if there is no distance
            if (distance == 0) return hit;  
            
            //Draws end box using last 4 points
            for (var i = 0; i < 4; i++)
            {
                var j = i == 3 ? 0 : i + 1;
                Debug.DrawLine(points[i+4],points[j+4], color);
            }
            //Shows translation from origin box to end box in grey
            for (var i = 0; i < 4; i++)
            {
                var j = i + 4;
                Debug.DrawLine(points[i],points[j], Color.grey);
            }

            return hit;
        }
    }
}