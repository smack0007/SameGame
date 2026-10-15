namespace SameGame;

public struct Rectangle
{
  public int X;
  public int Y;
  public int Width;
  public int Height;

  public Rectangle(int x, int y, int width, int height)
  {
    X = x;
    Y = y;
    Width = width;
    Height = height;
  }

  public Point TopLeft()
  {
    return new Point(X, Y);
  }

  public bool Intersects(Point p)
  {
    return
      p.X >= X &&
      p.X < X + Width &&
      p.Y >= Y &&
      p.Y < Y + Height;
  }
}