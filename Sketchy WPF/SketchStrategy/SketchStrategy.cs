using System;
using System.Collections.Generic;
using System.Windows;

namespace Sketchy_WPF {

    abstract class SketchStrategy {

        static readonly Random Random = new Random();

        protected double RandomRange(double start, double end) {
            return Random.Next((int) (start * 10), (int) (end * 10)) / 10.0;
        }

        protected int RandomSign() {
            return Random.Next(0, 1) == 0 ? -1 : 1;
        }

        public List<Point> ToSketchyPolygon(IEnumerable<Point> sourcePoints) {
            var points = new List<Point>(sourcePoints);

            if (points.Count <= 1) {
                return points;
            }

            if (points[0] != points[points.Count - 1]) {
                points.Add(points[0]);
            }

            return ToSketchyPolygonOverride(points);

        }

        protected abstract List<Point> ToSketchyPolygonOverride(IReadOnlyList<Point> sourcePoints);

    }

}