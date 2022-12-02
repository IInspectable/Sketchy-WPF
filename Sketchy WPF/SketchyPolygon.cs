using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Sketchy_WPF {

    public class SketchyPolygon: Shape {

        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
            name: nameof(Points),
            propertyType: typeof(PointCollection),
            ownerType: typeof(SketchyPolygon),
            typeMetadata: new FrameworkPropertyMetadata(
                defaultValue: null,
                flags: FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty FillRuleProperty = DependencyProperty.Register(
            name: nameof(FillRule),
            propertyType: typeof(FillRule),
            ownerType: typeof(SketchyPolygon),
            typeMetadata: new FrameworkPropertyMetadata(
                defaultValue: FillRule.EvenOdd,
                flags: FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty MaxStrokeDeviationProperty = DependencyProperty.Register(
            name: nameof(MaxStrokeDeviation),
            propertyType: typeof(double),
            ownerType: typeof(SketchyPolygon),
            typeMetadata: new FrameworkPropertyMetadata(
                defaultValue: 2.0,
                flags: FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double MaxStrokeDeviation {
            get => (double) GetValue(MaxStrokeDeviationProperty);
            set => SetValue(MaxStrokeDeviationProperty, value);
        }

        public static readonly DependencyProperty MinStrokeDeviationProperty = DependencyProperty.Register(
            name: nameof(MinStrokeDeviation),
            propertyType: typeof(double),
            ownerType: typeof(SketchyPolygon),
            typeMetadata: new FrameworkPropertyMetadata(
                defaultValue: .5,
                flags: FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double MinStrokeDeviation {
            get => (double) GetValue(MinStrokeDeviationProperty);
            set => SetValue(MinStrokeDeviationProperty, value);
        }

        public static readonly DependencyProperty StrokeLengthProperty = DependencyProperty.Register(
            name: nameof(StrokeLength),
            propertyType: typeof(double),
            ownerType: typeof(SketchyPolygon),
            typeMetadata: new FrameworkPropertyMetadata(
                defaultValue: 40.0,
                flags: FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double StrokeLength {
            get => (double) GetValue(StrokeLengthProperty);
            set => SetValue(StrokeLengthProperty, value);
        }

        public PointCollection Points {
            get => (PointCollection) GetValue(PointsProperty);
            set => SetValue(PointsProperty, value);
        }

        public FillRule FillRule {
            get => (FillRule)GetValue(FillRuleProperty);
            set => SetValue(FillRuleProperty, value);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _geometry=null;
            return base.ArrangeOverride(finalSize);
        }

        protected override Geometry DefiningGeometry => EnsureGeometry();

        private Geometry _geometry;

        Geometry EnsureGeometry() {
            return _geometry ?? (_geometry = DefineGeometry());
        }

        Geometry DefineGeometry() {

            //var sketchStrategy = new AlternateSketchStrategy();
            var sketchStrategy = new AlternateSketchStrategy {
                MinStrokeDeviation = MinStrokeDeviation,
                MaxStrokeDeviation = MaxStrokeDeviation,
                StrokeLength = StrokeLength,
            };

           //var sketchStrategy = new DefaultSketchStrategy {
           //    MinStrokeDeviation = MinStrokeDeviation,
           //    MaxStrokeDeviation = MaxStrokeDeviation,
           //};

            var pointArray = sketchStrategy.ToSketchyPolygon(Points);

            if (pointArray.Count == 0) {
                return Geometry.Empty;
            }

            var pathFigure = new PathFigure {
                IsClosed   = true,
                StartPoint = pointArray[0]
            };
            pathFigure.Segments.Add(new PolyLineSegment(pointArray, true));

            return new PathGeometry {
                Figures = {
                    pathFigure
                },
                FillRule = FillRule

            };
        }

    }

}