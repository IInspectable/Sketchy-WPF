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

        public PointCollection Points {
            get { return (PointCollection) GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

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
            return _polygonGeometry ?? (_polygonGeometry = DefineGeometry());
        }

        Geometry DefineGeometry() {

            //var sketchStrategy = new AlternateSketchStrategy();
            var sketchStrategy = new DefaultSketchStrategy();

            var pointArray = sketchStrategy.ToSketchyPolygon(Points);

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

    }

}