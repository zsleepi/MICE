using Godot;
using Godot.NativeInterop;
using System;
using System.Drawing;

public partial class CameraController : Node
{
    [Export] private Camera2D _camera;
    [Export] private World world;
    private string _cameraMode = "following";
    private Player _cameraTarget;
    [Export] private float _lazyCameraTrackingMargin = 0.1f;

    public override void _Ready()
    {
        SetCameraFollow(world.Player);
    }

    public override void _Process(double delta)
    {
        UpdateCameraBounds(world.CurrentRoom);
        HandleCentering();
        HandleCameraZoom();
        HandleCameraDrag();
        if (_cameraMode == "still") { return; }
        if (_cameraMode == "following") { FollowTarget(); }
        if (_cameraMode == "lazyFollow") { LazyFollowTarget(); }
    }


    public void SetCameraFollow(Player actor)
    {
        _cameraTarget = actor;
        _cameraMode = "following";
        _camera.PositionSmoothingEnabled = true;
    }

    public void Unstill(Player actor)
    {
        if (_cameraMode == "still") { SetCameraLazyFollow(actor); }
    }
    public void SetCameraLazyFollow(Player actor)
    {
        _cameraTarget = actor;
        _cameraMode = "lazyFollow";
        _camera.PositionSmoothingEnabled = true;
    }

    public void HandleCentering()
    {
        if (Input.IsActionJustPressed("center_camera")) { SetCameraFollow(world.Player); }
    }

    public void FollowTarget()
    {
        _camera.Position = _cameraTarget.Position;
    }

    public void LazyFollowTarget()
    {
        Rect2 rect = GetCameraRect();
        Vector2 margin = rect.Size * _lazyCameraTrackingMargin;
        rect.Size -= margin*2;
        rect.Position += margin;
        if (!rect.HasPoint(_cameraTarget.Position))
        {
            _camera.Position = _cameraTarget.Position;
        }
    }

    private Vector2 _prevMousePos;

    internal Rect2 GetCameraRect()
    {
        Vector2 pos = _camera.GetScreenCenterPosition();
        Vector2 size = GetViewport().GetVisibleRect().Size / _ZOOM_LEVELS[_currentZoomLevel];
        return new Rect2(pos-size/2, size);
    }

    internal void SetCameraStill()
    {
        _cameraMode = "still";
        _camera.PositionSmoothingEnabled = false;
    }
    internal void HandleCameraDrag()
    {
        Vector2 _mousePos = GetViewport().GetMousePosition();
        if (Input.IsActionPressed("drag_camera"))
        {
            if (_prevMousePos == null) { _prevMousePos = _mousePos; }
            Vector2 _velocity = _prevMousePos - _mousePos;
            SetCameraStill();
            _camera.Position = _camera.GetScreenCenterPosition() + _velocity / _ZOOM_LEVELS[_currentZoomLevel];
        }
        _prevMousePos = _mousePos;
    }

    private readonly float[] _ZOOM_LEVELS = [1, 1.2f, 1.45f, 1.7f, 2, 2.5f, 3, 4, 5, 6, 8, 11, 15, 20];
    private int _currentZoomLevel = 1;

    private void HandleCameraZoom()
    { 
	    if (Input.IsActionJustPressed("zoom_in"))
        {
            _currentZoomLevel += 1;
            _currentZoomLevel = Math.Min(_currentZoomLevel, _ZOOM_LEVELS.Length - 1);
        }
	    if (Input.IsActionJustPressed("zoom_out"))
        {
            _currentZoomLevel -= 1;
            _currentZoomLevel = Math.Max(_currentZoomLevel, 0);
        }
        _camera.Zoom = new Vector2(_ZOOM_LEVELS[_currentZoomLevel], _ZOOM_LEVELS[_currentZoomLevel]);
    }

    private void UpdateCameraBounds(Room room)
    {
        var terrain = room.Terrain;
        Rect2I usedRect = terrain.GetUsedRect();
        Vector2 topLeft = terrain.ToGlobal(terrain.MapToLocal(usedRect.Position));
        Vector2 bottomRight = terrain.ToGlobal(terrain.MapToLocal(
            usedRect.Position + usedRect.Size));
        topLeft -= new Vector2I(9, 9); bottomRight -= new Vector2I(9, 9);

        _camera.LimitLeft = (int)topLeft.X;
        _camera.LimitTop = (int)topLeft.Y;
        _camera.LimitRight = (int)bottomRight.X;
        _camera.LimitBottom = (int)bottomRight.Y;
    }
}
