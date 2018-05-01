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

        protected override void OnRender(DrawingContext drawingContext) {
            _polygonGeometry = null;
            base.OnRender(drawingContext);
        }

        private Geometry _polygonGeometry;

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
            CacheDefiningGeometry();
            return base.MeasureOverride(constraint);

        }

        Geometry EnsureGeometry() {
            if (_polygonGeometry == null) {
                CacheDefiningGeometry();
            }

            return _polygonGeometry;
        }

        private const double StrokeLength = 10;
        private const int StrokeDeviation = 3;

        Random _random = new Random();

        void CacheDefiningGeometry() {
            PointCollection points = Points;
            if (points == null) {
                _polygonGeometry = Geometry.Empty;
            } else {

                PathFigure pathFigure = new PathFigure();
                if (points.Count > 0) {

                    pathFigure.StartPoint = points[0];
                    if (points.Count > 1) {

                        var pointArray = new List<Point>();
                        for (int index = 1; index < points.Count; ++index) {

                            var p1 = points[index - 1];
                            var p2 = points[index];

                            var dx = p2.X - p1.X;
                            var dy = p2.Y - p1.Y;
                            var l  = Math.Sqrt(dx * dx + dy * dy);

                            var ax = dx / l;
                            var ay = dy / l;

                            if (l > StrokeLength) {

                                var parts = (int) (l / StrokeLength);

                                for (int part = 0; part < parts - 1; part++) {
                                    var dl = (part + 1) * StrokeLength;

                                    var x = p1.X + dl * ax + RandomDouble();
                                    var y = p1.Y + dl * ay + RandomDouble();

                                    var point = new Point(x, y);
                                    pointArray.Add(point);
                                }

                                pointArray.Add(points[index]);

                            } else {

                                pointArray.Add(points[index]);
                            }

                        }

                        pathFigure.Segments.Add(new PolyLineSegment(pointArray, true));
                    }

                    pathFigure.IsClosed = true;
                }

                _polygonGeometry = new PathGeometry() {
                    Figures = {
                        pathFigure
                    },
                    FillRule = FillRule
                };
            }
        }

    
        double RandomDouble() {
            return _random.Next(-StrokeDeviation, StrokeDeviation)/2;
        }

        int RandomSign() {
            return _random.Next(-1, +1) < 0 ? -1 : 1;
        }

    }

}