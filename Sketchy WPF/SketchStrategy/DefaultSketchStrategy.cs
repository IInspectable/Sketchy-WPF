using System;
using System.Collections.Generic;
using System.Windows;

// ReSharper disable UnusedMember.Global
namespace Sketchy_WPF {

    class DefaultSketchStrategy: SketchStrategy {

        public double MinStrokeLength    { get; set; } = 4;
        public double MinStrokeDeviation { get; set; } = 0.5;
        public double MaxStrokeDeviation { get; set; } = 2;

        protected override List<Point> ToSketchyPolygonOverride(IReadOnlyList<Point> sourcePoints) {
            var points = new List<Point>();

            if (sourcePoints.Count <= 1) {
                return points;
            }

            for (int index = 1; index < sourcePoints.Count; ++index) {

                var p1 = sourcePoints[index - 1];
                var p2 = sourcePoints[index];
                var dx = p2.X - p1.X;
                var dy = p2.Y - p1.Y;
                var l  = Math.Sqrt(dx * dx + dy * dy);
                var ax = dx / l;
                var ay = dy / l;

                if (l > 3 * MinStrokeLength) {

                    var dl = RandomRange(MinStrokeLength, l - MinStrokeLength);
                    var x  = p1.X + dl * ax + RandomRange(MinStrokeDeviation, MaxStrokeDeviation) * RandomSign();
                    var y  = p1.Y + dl * ay + RandomRange(MinStrokeDeviation, MaxStrokeDeviation) * RandomSign();

                    points.Add(new Point(x, y));

                    points.Add(sourcePoints[index]);

                } else {

                    points.Add(sourcePoints[index]);
                }

            }

            return points;
        }

    }

}