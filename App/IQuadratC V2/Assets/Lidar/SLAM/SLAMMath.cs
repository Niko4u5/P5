using System;
using Unity.Mathematics;
using UnityEngine;

namespace Lidar.SLAM
{
    public class SLAMMath
    {
        public static float MapAcsess(float2 p, SLAMMap map)
        {
            int2 p0 = (int2) p / map.scale;
            int2 p1 = p0 + new int2(1, 1);
            p = p / map.scale;
            float a = p.y - p0.y;
            float b = p1.y - p.y;
            float c = p.x - p0.x;
            float d = p1.x - p.x;
            
            float m00 = map.GetMap(p0);
            float m01 = map.GetMap(new int2(p0.x, p1.y));
            float m10 = map.GetMap(new int2(p1.x, p0.y));
            float m11 = map.GetMap(p1);

            float x = a * (c * m11 + d * m01)
                      + b * (c * m10 + d * m00);
            return x;
        }
        
        public static float2 MapAcsessDirtative(float2 p, SLAMMap map)
        {
            int2 p0 = (int2) p / map.scale;
            int2 p1 = p0 + new int2(1, 1);
            p = p / map.scale;
            float a = p.y - p0.y;
            float b = p1.y - p.y;
            float c = p.x - p0.x;
            float d = p1.x - p.x;
            
            float m00 = map.GetMap(p0);
            float m01 = map.GetMap(new int2(p0.x, p1.y));
            float m10 = map.GetMap(new int2(p1.x, p0.y));
            float m11 = map.GetMap(p1);

            float2 x = new float2(
                a * (m11 - m01) + b * (m10 - m00),
                c * (m11 - m01) + d * (m10 - m00));
            return x;
        }
        
        public static float2 Si(float3 t, float2 si)
        {
            float2x2 a = new float2x2(math.cos(t.z), -math.sin(t.z), math.sin(t.z), math.cos(t.z));
            float2 x = math.mul(a, si) + t.xy;
            return x;
        }
        
        public static float2 TransformDeltaDir(float3 t, SLAMLidarDataSet dataSet, SLAMMap map)
        {
            float2 dir = new float2();
            foreach (float2 point in dataSet.points)
            {
                float2 gradiant = MapAcsessDirtative(Si(t, point), map);
                dir += gradiant;
            }

            dir = math.normalize(dir);
            return dir;
        }

        public static float CalcError(float3 t, SLAMLidarDataSet dataSet, SLAMMap map)
        {
            float error = 0;
            foreach (float2 point in dataSet.points)
            {
                float errorForPoint = MapAcsess(Si(t, point), map);
                error += 1 - errorForPoint;
            }

            return error;
        }

        public static float CalcBestAngle(float3 t, SLAMLidarDataSet dataSet, SLAMMap map, float x)
        {
            float bestRad = 0;
            float bestError = float.MaxValue;
            for (int i = 0; i < x; i++)
            {
                float rad = 2 * math.PI * (i - x / 2) / x; // (360 / x) * (i - (x / 2)) * math.PI / 180;

                float error = CalcError(TransformAddRoation(t, rad), dataSet, map);

                if (error < bestError)
                {
                    bestError = error;
                    bestRad = rad;
                }
            }

            return bestRad;
        }

        public static float3 TransformAddRoation(float3 t, float rad)
        {
            float3 newT = float3.zero;
            float2x2 a = new float2x2(-math.cos(rad), math.sin(rad), -math.sin(rad), -math.cos(rad));
            newT.xy = math.mul(a, t.xy);
            newT.z = t.z + rad;
            return newT;
        }
        
        /*
        
        public static float3 DeltaT(float3 t, SLAMLidarDataSet dataSet, SLAMMap map)
        {
            float3 deltat = new float3();
            foreach (float2 si in dataSet.points)
            {
                deltat += Func12(t, si, map);
            }

            deltat.xy = math.normalize(deltat.xy);
            return deltat;
        }

        public static float3 Func12(float3 t, float2 si, SLAMMap map)
        {
            float3 h = math.pow(H(t, si, map), new float3(-1));
            float2 a = Si(t, si);
            float2 b = MapAcsessDirtative(a, map);
            float2x3 mat14 = Func14(t, si);
            float3 c = math.mul(b, mat14);
            
            float d = MapAcsess(a, map);

            float3 e = (c * (1 - d));
            return e;
        }
        
        
        public static float3 H(float3 t, float2 si, SLAMMap map)
        {
            float2x3 mat14 = Func14(t, si);
            float2 a = Si(t, si);
            float2 b = MapAcsessDirtative(a, map);
            float3 c = math.mul(b, mat14);
            float3 d = c;
            return d;
        }
        
        public static float2x3 Func14(float3 t, float2 si)
        {
            float2x3 mat = new float2x3(
                1, 0, -math.sin(t.z) * si.x -math.cos(t.z) * si.y,
                0, 1,  math.cos(t.z) * si.x -math.sin(t.z) * si.y);
            return mat;
        }
        */
    }
}
