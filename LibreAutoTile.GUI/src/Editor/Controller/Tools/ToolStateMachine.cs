using Godot;

namespace LibreAutoTile.GUI.Editor.Controller.Tools;

public class ToolStateMachine
{
  private readonly EditorContext context;
  private readonly IToolState[] states;

  private int previousTabIndex;

  public TilesToolController TilesTool { get; }
  public ProbabilityToolController ProbabilityTool { get; }
  public PreviewToolController PreviewTool { get; }

  public IToolState CurrentState { get; private set; }

  public bool ShowsMouseLabel => CurrentState.ShowsMouseLabel;

  public ToolStateMachine(EditorContext context)
  {
    this.context = context;

    TilesTool = new TilesToolController(context);
    ProbabilityTool = new ProbabilityToolController(context);
    PreviewTool = new PreviewToolController(context);

    // Order must match the tabs in ToolsPanel
    states =
    [
      new IdleToolState(context),
      TilesTool,
      ProbabilityTool,
      PreviewTool
    ];

    previousTabIndex = context.View.ToolsPanel.CurrentTab;
    CurrentState = states[previousTabIndex];

    context.View.ToolsPanel.TabChanged += OnTabChanged;
  }

  private void OnTabChanged(long tab)
  {
    int newTab = (int)tab;
    if (newTab == previousTabIndex)
      return;

    var target = states[newTab];

    if (!target.CanActivate())
    {
      context.View.MessageDisplay.DisplayText($"[color=red]{target.ActivationError}[/color]");
      context.View.ToolsPanel.CurrentTab = previousTabIndex;
      return;
    }

    CurrentState.Exit();
    CurrentState = target;
    CurrentState.Enter();
    previousTabIndex = newTab;
  }

  public void HandleMouseClick(InputEventMouse mouse)
  {
    if (GodotExtensions.IsMouseOnElements(context.View.UiElements))
      return;

    CurrentState.HandleMouseInput(mouse, context.View.GetGlobalMousePosition());
  }

  public void HandleMouseMotion(Vector2 position)
    => CurrentState.HandleMouseMotion(position);

  public bool HandleMouseWheel(int delta, bool shiftPressed, bool isMouseOnUi)
  {
    if (isMouseOnUi)
      return false;

    return CurrentState.HandleMouseWheel(delta, shiftPressed);
  }
}
