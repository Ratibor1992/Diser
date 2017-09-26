using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace WindowsFormsApplication1
{
    class Vertex
    {
        public double x, y;

        public Vertex(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }

    class Edge
    {
        public int v1, v2;

        public Edge(int v1, int v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }
    }

    class VertexData
    {
        public int id;
        public double netForceX;
        public double netForceY;
        public double VelocityX;
        public double VelocityY;
    }
    class DrawGraph
    {
        Bitmap bitmap;
        Pen blackPen;
        Pen redPen;
        Pen darkGoldPen;
        Graphics gr;
        Font fo;
        Brush br;
        PointF point;
        public int R = 19; //радиус окружности вершины

        public DrawGraph(int width, int height)
        {
            bitmap = new Bitmap(width, height);
            gr = Graphics.FromImage(bitmap);
            clearSheet();
            blackPen = new Pen(Color.Black);
            blackPen.Width = 2;
            redPen = new Pen(Color.Red);
            redPen.Width = 2;
            darkGoldPen = new Pen(Color.DarkGoldenrod);
            darkGoldPen.Width = 2;
            fo = new Font("Arial", 15);
            br = Brushes.Black;
        }

        public Bitmap GetBitmap()
        {
            return bitmap;
        }

        public void clearSheet()
        {
            gr.Clear(Color.White);
        }

        public void drawVertex(int x, int y, string number)
        {
            gr.FillEllipse(Brushes.White, (x - R), (y - R), 2 * R, 2 * R);
            gr.DrawEllipse(blackPen, (x - R), (y - R), 2 * R, 2 * R);
            point = new PointF(x - 9, y - 9);
            gr.DrawString(number, fo, br, point);
        }

        public void drawSelectedVertex(int x, int y)
        {
            gr.DrawEllipse(redPen, (x - R), (y - R), 2 * R, 2 * R);
        }

        public void drawEdge(Vertex V1, Vertex V2, Edge E, int numberE)
        {
            if (E.v1 == E.v2)
            {
                gr.DrawArc(darkGoldPen, ((int)V1.x - 2 * R), ((int)V1.y - 2 * R), 2 * R, 2 * R, 90, 270);
                point = new PointF((int)V1.x - (int) (2.75 * R), (int)V1.y - (int) (2.75 * R));
                gr.DrawString(((char) ('a' + numberE)).ToString(), fo, br, point);
                drawVertex((int)V1.x, (int)V1.y, (E.v1 + 1).ToString());
            }
            else
            {
                gr.DrawLine(darkGoldPen, (int)V1.x, (int)V1.y, (int)V2.x, (int)V2.y);
                point = new PointF(((int)V1.x + (int)V2.x) / 2, ((int)V1.y + (int)V2.y) / 2);
                gr.DrawString(((char) ('a' + numberE)).ToString(), fo, br, point);
                drawVertex((int)V1.x, (int)V1.y, (E.v1 + 1).ToString());
                drawVertex((int)V2.x, (int)V2.y, (E.v2 + 1).ToString());
            }
        }

        public void drawALLGraph(List<Vertex> V, List<Edge> E)
        {
            /*
            foreach(var vertex in V)
            {
                vertex.x /= 2;
                vertex.y /= 2;
            }
            */
            //рисуем ребра
            for (int i = 0; i < E.Count; i++)
            {
                if (E[i].v1 == E[i].v2)
                {
                    gr.DrawArc(darkGoldPen, ((int)V[E[i].v1].x - 2 * R), ((int)V[E[i].v1].y - 2 * R), 2 * R, 2 * R, 90, 270);
                    point = new PointF((int)V[E[i].v1].x - (int) (2.75 * R), (int)V[E[i].v1].y - (int) (2.75 * R));
                    gr.DrawString(((char) ('a' + i)).ToString(), fo, br, point);
                }
                else
                {
                    gr.DrawLine(darkGoldPen, (int)V[E[i].v1].x, (int)V[E[i].v1].y, (int)V[E[i].v2].x, (int)V[E[i].v2].y);
                    point = new PointF(((int)V[E[i].v1].x + (int)V[E[i].v2].x) / 2, ((int)V[E[i].v1].y + (int)V[E[i].v2].y) / 2);
                    gr.DrawString(((char) ('a' + i)).ToString(), fo, br, point);
                }
            }
            //рисуем вершины
            for (int i = 0; i < V.Count; i++)
            {
                drawVertex((int)V[i].x, (int)V[i].y, (i + 1).ToString());
            }
        }

        public void VertexesReposition(int XMax, int YMax, List<Vertex> V, List<Edge> E)
        {

            foreach (var ventex in V)
            {
                ventex.x = 0;
                ventex.y = 0;
            }
            var maxGravityDistanceSqr = Math.Max(XMax, YMax) / 5.0;
            maxGravityDistanceSqr = maxGravityDistanceSqr * maxGravityDistanceSqr;
            //Math.min(viewportSize.x, viewportSize.y) / 2.0;
            var velocityDamping = 0.85;
            var diameter = R;
            var maxDistance = diameter * 3;
            var gravityDistanceSqr = 10 * (maxDistance * maxDistance);
            double edgeGravityKof = 10.0 / (maxDistance);
            double kCenterForce = 10.0 / (maxDistance * 10);
            double centerPointX = XMax / 2.0;
            double centerPointY = YMax / 2.0;
            var velocityMax = maxDistance * 10;

            int[] edgesMatrix = new int[V.Count * 1000 + V.Count];
            for (var i = 0; i < E.Count; i++)
            {
                edgesMatrix[E[i].v1 + E[i].v2 * 1000] = 1;
                edgesMatrix[E[i].v2 + E[i].v1 * 1000] = 1;
            }

            Random rnd = new Random();
            var startAngel = rnd.NextDouble() * 180.0;
            for (var i = 0; i < V.Count; i++) // loop through vertices
            {
                //BUG: double to int, possible loss of data
                V[i].x = Orbit(true, XMax / 2.0, (XMax - diameter * 2) / 2.0, (YMax - diameter * 2) / 2.0, 360.0 * i / V.Count + startAngel);
                V[i].y = Orbit(false, YMax / 2.0, (XMax - diameter * 2) / 2.0, (YMax - diameter * 2) / 2.0, 360.0 * i / V.Count + startAngel);
            }

            var k = 0;
            var bChanged = true;
            while (k < 1000 && bChanged)
            {
                List<VertexData> vertexData = new List<VertexData>();
                for (var i = 0; i < V.Count; i++) // loop through vertices
                {
                    // Has no in newVertexes.
                    VertexData currentVertex = new VertexData();
                    currentVertex.id = i;

                    for (var j = 0; j < V.Count; j++) // loop through other vertices
                    {
                        Vertex otherVertex = V[j];

                        if (j == currentVertex.id) continue;

                        // squared distance between "u" and "v" in 2D space

                        var rsq = (V[currentVertex.id].x - otherVertex.x) * (V[currentVertex.id].x - otherVertex.x) +
                                  (V[currentVertex.id].y - otherVertex.y) * (V[currentVertex.id].y - otherVertex.y);

                        // counting the repulsion between two vertices


                        var tempForceX = (V[currentVertex.id].x - otherVertex.x);
                        var tempForceY = (V[currentVertex.id].y - otherVertex.y);

                        var l = Math.Sqrt(tempForceX * tempForceX + tempForceY * tempForceY);
                        var thickness = gravityDistanceSqr / rsq;
                        //BUG:  double to int, possible loss of data
                        currentVertex.netForceX += (tempForceX / l * thickness);
                        currentVertex.netForceY += (tempForceY / l * thickness);
                    }

                    for (var j = 0; j < V.Count; j++) // loop through edges
                    {
                        Vertex otherVertex = V[j];
                        //if (edgesMatrix.hasOwnProperty(currentVertex.object.id + 1000 * otherVertex.id))
                        if (edgesMatrix[currentVertex.id + 1000 * j] == 1)
                        {
                            var distance = Math.Sqrt((V[currentVertex.id].x - otherVertex.x) * (V[currentVertex.id].x - otherVertex.x) +
                                  (V[currentVertex.id].y - otherVertex.y) * (V[currentVertex.id].y - otherVertex.y));

                            if (distance > maxDistance)
                            {
                                // countin the attraction
                                var tempForceX = (otherVertex.x - V[currentVertex.id].x );
                                var tempForceY = (otherVertex.y - V[currentVertex.id].y);

                                var l = Math.Sqrt(tempForceX * tempForceX + tempForceY * tempForceY);
                                double thickness = edgeGravityKof * (distance - maxDistance);
                                //BUG:  double to int, possible loss of data
                                currentVertex.netForceX += (tempForceX / l * thickness);
                                currentVertex.netForceY += (tempForceY / l * thickness);
                            }
                        }
                    }

                    // Calculate force to center of world.
                    var distanceToCenter = Math.Sqrt((centerPointX - V[currentVertex.id].x) * (centerPointX - V[currentVertex.id].x) +
                                  (centerPointY - V[currentVertex.id].y) * (centerPointY - V[currentVertex.id].y));

                    var tempFinishForceX = (centerPointX - V[currentVertex.id].x);
                    var tempFinishForceY = (centerPointY - V[currentVertex.id].y);

                    var len = Math.Sqrt(tempFinishForceX * tempFinishForceX + tempFinishForceY * tempFinishForceY);
                    var Finishthickness = distanceToCenter * kCenterForce;
                    //BUG:  double to int, possible loss of data
                    currentVertex.netForceX += (tempFinishForceX / len * Finishthickness);
                    currentVertex.netForceY += (tempFinishForceY / len * Finishthickness);

                    // counting the velocity (with damping 0.85)
                    currentVertex.VelocityX += currentVertex.netForceX;
                    currentVertex.VelocityY += currentVertex.netForceY;

                    vertexData.Add(currentVertex);
                }

                bChanged = false;

                for(var i = 0; i < vertexData.Count; i++) // set new positions
                {
                    var v = vertexData[i];
                    var velocityX = v.VelocityX;
                    var velocityY = v.VelocityY;
                    var velocityLength = Math.Sqrt(velocityX * velocityX + velocityY * velocityY);
                    if (velocityLength > velocityMax)
                    {
                        //BUG:  double to int, possible loss of data
                        velocityX = (velocityX / velocityLength * velocityMax);
                        velocityY = (velocityY / velocityLength * velocityMax);
                        
                        //velocity = velocity.normalize(velocityMax);
                    }
                    V[v.id].x += velocityX;
                    V[v.id].y += velocityY;

                    velocityLength = Math.Sqrt(velocityX * velocityX + velocityY * velocityY);

                    if (velocityLength >= 1)
                    {
                        bChanged = true;
                    }
                }
                k++;
            }

            // Looks like somthing going wrong and will use circle algorithm for reposition.
            //var bbox = this.getGraphBBox();

            var pointMin = new Point((int)V[0].x, (int)V[0].y);
            var pointMax = new Point((int)V[0].x, (int)V[0].y);
           
            for (var i = 0; i < V.Count; i++)
            {
                var vertex = V[i];
                var deltaVector = new Point(1 * diameter, diameter);

                if (pointMin.Y > (V[i].y - deltaVector.Y))
                    pointMin.Y = (int)(V[i].y - deltaVector.Y);
                if (pointMin.X > (V[i].x - deltaVector.X))
                    pointMin.X = (int)(V[i].x - deltaVector.X);
                if (pointMax.Y < (V[i].y + deltaVector.Y))
                    pointMax.Y = (int)(V[i].y + deltaVector.Y);
                if (pointMax.X < (V[i].x + deltaVector.X))
                    pointMax.X = (int)(V[i].x + deltaVector.X);
            }

            var bboxLen =  Math.Sqrt((pointMax.X - pointMin.X) * (pointMax.X - pointMin.X)
                                       + (pointMax.Y - pointMin.Y) * (pointMax.Y - pointMin.Y));
            var ViewPortLength = Math.Sqrt((XMax* XMax) + (YMax * YMax));
            if (bboxLen > ViewPortLength * 1000)
            {
                for (var i = 0; i < V.Count; i++) // loop through vertices
                {
                    //BUG: double to int, possible loss of data
                    V[i].x = Orbit(true, XMax / 2.0, (XMax - diameter * 2) / 2.0, (YMax - diameter * 2) / 2.0, 360.0 * i / V.Count + startAngel);
                    V[i].y = Orbit(false, YMax / 2.0, (XMax - diameter * 2) / 2.0, (YMax - diameter * 2) / 2.0, 360.0 * i / V.Count + startAngel);
                }
            }
            else
            {
                // Try to rotate graph to fill small area.
                var count = 20;
                var agnle = 360.0 / count;
                var viewportAspect = XMax / YMax;
                var bestIndex = 0;
                var bestAspect = (pointMax.X - pointMin.X) / (pointMax.Y - pointMin.Y);
                //var center = bbox.center();
                var centerX = (pointMin.X + pointMax.X) / 2.0;
                var centerY = (pointMin.Y + pointMax.Y) / 2.0;

                for (var i = 1; i < count; i++)
                {
                    for (var j = 0; j < V.Count; j++) // loop through vertices
                    {
                        var radians = agnle * (Math.PI / 180);
                        var offsetX = V[j].x - centerX;
                        var offsetY = V[j].y - centerY;
                        //BUG: double to int, possible loss of data
                        V[j].x = (offsetX * Math.Cos(radians) - offsetY * Math.Sin(radians));
                        V[j].y = (offsetX * Math.Sin(radians) + offsetY * Math.Cos(radians));
                        V[j].x = (V[j].x + centerX);
                        V[j].y = (V[j].y + centerY);
                    }

                    var newpointMin = new Point((int)V[0].x, (int)V[0].y);
                    var newpointMax = new Point((int)V[0].x, (int)V[0].y);

                    for (var j = 0; j < V.Count; j++)
                    {
                        var deltaVector = new Point(1 * diameter, diameter);

                        if (newpointMin.Y > (V[j].y - deltaVector.Y))
                            newpointMin.Y = (int)(V[j].y - deltaVector.Y);
                        if (newpointMin.X > (V[j].x - deltaVector.X))
                            newpointMin.X = (int)(V[j].x - deltaVector.X);
                        if (newpointMax.Y < (V[j].y + deltaVector.Y))
                            newpointMax.Y = (int)(V[j].y + deltaVector.Y);
                        if (newpointMax.X < (V[j].x + deltaVector.X))
                            newpointMax.X = (int)(V[j].x + deltaVector.X);
                    }

                    var newAspect = (newpointMax.X - newpointMin.X) / (newpointMax.Y - newpointMin.Y);
                    if (Math.Abs(newAspect - viewportAspect) < Math.Abs(bestAspect - viewportAspect))
                    {
                        bestAspect = newAspect;
                        bestIndex = i;
                    }
                }

                // Rotate to best aspect.
                for (var j = 0; j < V.Count; j++) // loop through vertices
                {
                    //newVertexes[j].position.rotate(center, );
                    /*
                    function(center, degrees){
                    var radians = degrees * (Math.PI / 180);
                    offset = this.subtract(center);
                    this.x = offset.x * Math.cos(radians) - offset.y * Math.sin(radians);
                    this.y = offset.x * Math.sin(radians) + offset.y * Math.cos(radians);
                    this.x = this.x + center.x;
                    this.y = this.y + center.y;
                    */
                    var radians = (-agnle * (count - bestIndex - 1)) * (Math.PI / 180);
                    var offsetX = V[j].x - centerX;
                    var offsetY = V[j].y - centerY;
                    //BUG: double to int, possible loss of data
                    V[j].x = (offsetX * Math.Cos(radians) - offsetY * Math.Sin(radians));
                    V[j].y = (offsetX * Math.Sin(radians) + offsetY * Math.Cos(radians));
                    V[j].x = (V[j].x + centerX);
                    V[j].y = (V[j].y + centerY);
                }
            }
        }

        private double Orbit(bool isX, double val, double arcWidth, double arcHeight, double degrees)
        {
            var radians = degrees * (Math.PI / 180);

            if (isX)
                return val + arcWidth * Math.Cos(radians);
            return val + arcHeight * Math.Sin(radians);
        }
    }
}
