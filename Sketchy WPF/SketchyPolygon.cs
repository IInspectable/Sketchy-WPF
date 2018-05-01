using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Sketchy_WPF {

    public class SketchyPolygon: Shape {

        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
            nameof(Points), typeof(PointCollection),
            typeof(SketchyPolygon),
            new FrameworkPropertyMetadata(defaultValue: null,
                                          flags: FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty FillRuleProperty = DependencyProperty.Register(
            nameof(FillRule), typeof(FillRule),
            typeof(SketchyPolygon),
            new FrameworkPropertyMetadata(FillRule.EvenOdd,
                                          FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender)
        );

        /// <summary>Gets or sets a collection that contains the vertex points of the polygon.   </summary>
        /// <returns>A collection of <see cref="T:System.Windows.Point" /> structures that describe the vertex points of the polygon. The default is a null reference (<see langword="Nothing" /> in Visual Basic).</returns>
        public PointCollection Points {
            get { return (PointCollection) GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        /// <summary>Gets or sets a <see cref="T:System.Windows.Media.FillRule" /> enumeration that specifies how the interior fill of the shape is determined.   </summary>
        /// <returns>One of the <see cref="T:System.Windows.Media.FillRule" /> enumeration values. The default is <see cref="F:System.Windows.Media.FillRule.EvenOdd" />.</returns>
        public FillRule FillRule {
            get { return (FillRule) GetValue(Polygon.FillRuleProperty); }
            set { SetValue(Polygon.FillRuleProperty, value); }
        }

        protected override Geometry DefiningGeometry => EnsureGeometry();

        protected override Size MeasureOverride(Size constraint) {
            EnsureGeometry();
            return base.MeasureOverride(constraint);

        }

        private Geometry _polygonGeometry;

        Geometry EnsureGeometry() {
            if (_polygonGeometry == null) {
                _polygonGeometry = DefineGeometry();
            }

            return _polygonGeometry;
        }

        private const double StrokeLength    = 40;
        private const int    StrokeDeviation = 3;

        Geometry DefineGeometry() {

            var pointArray = ScruffyPolygon(Points, StrokeLength, StrokeDeviation);
            if (pointArray.Count == 0) {
                return Geometry.Empty;
            }

            var pathFigure = new PathFigure {
                IsClosed   = true,
                StartPoint = pointArray[0]
            };
            pathFigure.Segments.Add(new PolyLineSegment(pointArray, true));

            return new PathGeometry() {
                Figures = {
                    pathFigure
                },
                FillRule = FillRule,

            };
        }

        static List<Point> ScruffyPolygon(IList<Point> sourcePoints, double strokeLength, double maxDeviation) {

            var points = new List<Point>();

            if (sourcePoints.Count <= 1) {
                return points;
            }

            if (sourcePoints[0] != sourcePoints[sourcePoints.Count - 1]) {
                sourcePoints.Add(sourcePoints[0]);
            }

            for (int index = 1; index < sourcePoints.Count; ++index) {

                var p1 = sourcePoints[index - 1];
                var p2 = sourcePoints[index];
                var dx = p2.X - p1.X;
                var dy = p2.Y - p1.Y;
                var l  = Math.Sqrt(dx * dx + dy * dy);
                var ax = dx / l;
                var ay = dy / l;

                if (l > strokeLength) {

                    var parts = (int) (l / strokeLength);

                    for (int part = 0; part < parts - 1; part++) {

                        var dl = part * strokeLength + strokeLength;
                        var x  = p1.X + dl * ax      + RandomRange(0.5, maxDeviation) * RandomSign();
                        var y  = p1.Y + dl * ay      + RandomRange(0.5, maxDeviation) * RandomSign();

                        points.Add(new Point(x, y));
                    }

                    points.Add(sourcePoints[index]);

                } else {

                    points.Add(sourcePoints[index]);
                }

            }

            return points;
        }

        static readonly Random Random = new Random();

        static double RandomRange(double start, double end) {
            return Random.Next((int) (start * 10), (int) (end * 10)) / 10.0;
        }

        static int RandomSign() {
            return Random.Next(0, 1) == 0 ? -1 : 1;
        }

    }

}