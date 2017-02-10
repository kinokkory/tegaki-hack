using System;
using System.Collections.Generic;
using static handhack.UtilStatic;

namespace handhack
{
    public enum Touchevent { Down, Move, Up }

    public partial class Editor : IDrawable
    {
        public DPoint<Internal> size;
        public float width { get { return size.dx; } set { size.dx = value; } }
        public Size<Internal> Width { get { return new Size<Internal>(width); } set { width = value.value; } }
        public float height { get { return size.dy; } set { size.dy = value; } }
        public Size<Internal> Height { get { return new Size<Internal>(height); } set { height = value.value; } }
        List<IShape> shapes, redoshapes;
        public Paint paint, gridpaint;
        public AShapeCreator shapeCreator;
        IShape grid;

        public bool undoable { get { return shapes.Count > 0; } }
        public bool redoable { get { return redoshapes.Count > 0; } }

        public Action update;
        public BoolAction setUndoAbility, setRedoAbility;

        public Editor(DPoint<Internal> size)
        {
            this.size = size;
            shapes = new List<IShape>();
            redoshapes = new List<IShape>();
            paint = new Paint(new Color(0, 255, 0, 255), new Size<Internal>(0.5f), new Color(255, 255, 0, 128));
            gridpaint = new Paint(new Color(192, 192, 192, 255), default(Size<Internal>), new Color(0, 0, 0, 0), new Size<External>(1));
            shapeCreator = new FreehandCreator(paint);

            shapeCreator.editted += () =>
            {
                redoshapes.Clear();
                Update();
            };
        }

        public void Undo()
        {
            if (shapes.Count > 0)
            {
                redoshapes.Add(shapes.Pop());
                Update();
            }
        }
        public void Redo()
        {
            if (redoshapes.Count > 0)
            {
                shapes.Add(redoshapes.Pop());
                Update();
            }
        }
        public void Touch(Touchevent touchevent, Point<Internal> p)
        {
            if (shapeCreator != null)
            {
                shapeCreator.Touch(touchevent, p);
            }
        }
        public void Update()
        {
            setUndoAbility(undoable);
            setRedoAbility(redoable);
            SetGrid();
            update();
        }

        public void SetGrid()
        {
            var shapes = new List<IShape>();
            for (float x = 0; x <= width; x++)
            {
                shapes.Add(new Polyline(gridpaint, newList(new Point<Internal>(x, 0), new Point<Internal>(x, height))));
            }
            for (float y = 0; y <= height; y++)
            {
                shapes.Add(new Polyline(gridpaint, newList(new Point<Internal>(0, y), new Point<Internal>(width, y))));
            }
            grid = new ShapeGroup(shapes.ToArray());
        }
    }
}