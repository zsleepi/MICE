using Godot;
using Godot.NativeInterop;
using System;
using System.Drawing;

public partial class CameraController : Node
{
    // References
    [Export] private Camera2D _camera;
    [Export] private World _world;

    // Constants (Configurable)
    private static readonly float[] _ZOOM_LEVELS = [1, 1.2f, 1.45f, 1.7f, 2, 2.5f, 3, 4, 5, 6, 8, 11, 15, 20];
    private static readonly float _LAZY_FOLLOW_MARGIN = 0.1f;

    // States
    private int _currentZoomLevel = 1;
    private string _cameraMode = "following";
    private Player _cameraTarget;

    public override void _Ready()
    {
        SetCameraFollow(_world.Player);
    }

    public override void _Process(double delta)
    {
        HandleCentering();
        HandleCameraZoom();
        HandleCameraDrag();
        if (_cameraMode == "still") { return; }
        if (_cameraMode == "following") { FollowTarget(); }
        if (_cameraMode == "lazyFollow") { LazyFollowTarget(); }
    }

    // state setters
    public void SetCameraFollow(Player actor)
    {
        _cameraTarget = actor;
        _cameraMode = "following";
    }
    public void SetCameraLazyFollow(Player actor)
    {
        _cameraTarget = actor;
        _cameraMode = "lazyFollow";
    }
    internal void SetCameraStill()
    {
        _cameraMode = "still";
    }

    public void Unstill(Player actor)
    {
        if (_cameraMode == "still") { SetCameraLazyFollow(actor); }
    }

    // state operators
    public void FollowTarget()
    {
        SetCameraPosition(_cameraTarget.Position);
    }

    public void LazyFollowTarget()
    {
        Rect2 rect = GetCameraRect();
        Vector2 margin = rect.Size * _LAZY_FOLLOW_MARGIN;
        rect.Size -= margin*2;
        rect.Position += margin;
        if (!rect.HasPoint(_cameraTarget.Position))
        {
            Vector2 dif = _cameraTarget.Position - _camera.Position;
            dif *= 1.5f;
            SetCameraPosition(_camera.Position + dif);
        }
    }

    // input handlers
    private Vector2 _prevMousePos;
    internal void HandleCameraDrag()
    {
        Vector2 _mousePos = GetViewport().GetMousePosition();
        if (Input.IsActionPressed("drag_camera"))
        {
            if (_prevMousePos == null) { _prevMousePos = _mousePos; }
            Vector2 _velocity = _prevMousePos - _mousePos;
            SetCameraStill();
            SetCameraPosition(_camera.GetScreenCenterPosition() + _velocity / _ZOOM_LEVELS[_currentZoomLevel]);
        }
        _prevMousePos = _mousePos;
    }
    private void HandleCameraZoom()
    { 
	    if (Input.IsActionJustPressed("zoom_in"))
        {
            if (_cameraMode == "lazyFollow") { SetCameraStill(); }
            _currentZoomLevel += 1;
            _currentZoomLevel = Math.Min(_currentZoomLevel, _ZOOM_LEVELS.Length - 1);
        }
	    if (Input.IsActionJustPressed("zoom_out"))
        {
            if (_cameraMode == "lazyFollow") { SetCameraStill(); }
            _currentZoomLevel -= 1;
            _currentZoomLevel = Math.Max(_currentZoomLevel, 0);
        }
        _camera.Zoom = new Vector2(_ZOOM_LEVELS[_currentZoomLevel], _ZOOM_LEVELS[_currentZoomLevel]);
    }
    public void HandleCentering()
    {
        if (Input.IsActionJustPressed("center_camera")) { SetCameraFollow(_world.Player); }
    }

    // utils
    private Rect2 GetRoomBounds()
    {
        var terrain = _world.CurrentRoom.Terrain;
        Rect2I usedRect = terrain.GetUsedRect();
        Vector2 topLeft = terrain.ToGlobal(terrain.MapToLocal(usedRect.Position));
        Vector2 bottomRight = terrain.ToGlobal(terrain.MapToLocal(usedRect.Position + usedRect.Size));
        topLeft -= new Vector2I(9, 9); bottomRight -= new Vector2I(9, 9);
        Rect2 RoomBounds = new Rect2(topLeft, bottomRight-topLeft);
        return RoomBounds;
    }
    internal Rect2 GetCameraRect()
    {
        Vector2 pos = _camera.GetScreenCenterPosition();
        Vector2 size = GetViewport().GetVisibleRect().Size / _ZOOM_LEVELS[_currentZoomLevel];
        return new Rect2(pos - size / 2, size);
    }

    internal Vector2 GetBoundedPosition(Vector2 _cameraPosition)
    {
        Rect2 cameraBounds = GetCameraRect();
        Rect2 roomBounds = GetRoomBounds();

        Vector2 newPosition = _cameraPosition;
        newPosition.X = Math.Clamp(_cameraPosition.X, roomBounds.Position.X + cameraBounds.Size.X / 2, roomBounds.End.X - cameraBounds.Size.X / 2);
        newPosition.Y = Math.Clamp(_cameraPosition.Y, roomBounds.Position.Y + cameraBounds.Size.Y / 2, roomBounds.End.Y - cameraBounds.Size.Y / 2);
        return newPosition;
    }

    internal void SetCameraPosition(Vector2 position)
    {
        _camera.Position = GetBoundedPosition(position);
    }
}
