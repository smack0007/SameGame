using System.Numerics;
using RaylibSharp;
using SameGame.Gameplay;

namespace SameGame;

public class Mouse
{
  public Point Position { get; private set; }

  public bool IsLeftMouseButtonPressed { get; private set; }

  public void Update()
  {
  var mousePosition = Raylib.GetMousePosition();

  Position = new Point()
  {
    X = (int)mousePosition.x,
    Y = (int)mousePosition.y,
  };

  IsLeftMouseButtonPressed = Raylib.IsMouseButtonPressed(Raylib.MOUSE_BUTTON_LEFT);
  }
}