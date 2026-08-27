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

    public override void _Ready()
    {
        SetTarget(world.Player);
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


    public void SetTarget(Player actor)
    {
        _cameraTarget = actor;
    }

    public void HandleCentering()
    {
        if (Input.IsActionJustPressed("center_camera")) { _cameraMode = "following"; }
    }

    public void FollowTarget()
    {
        _camera.Position = _cameraTarget.Position;
    }

    public void SlowFollowTarget()
    {

    }

    public void LazyFollowTarget()
    {
        _camera.Position = _cameraTarget.Position;
    }

    private Vector2 _prevMousePos;
    internal void HandleCameraDrag()
    {
        Vector2 _mousePos = GetViewport().GetMousePosition();
        if (Input.IsActionPressed("drag_camera"))
        {
            if (_prevMousePos == null) { _prevMousePos = _mousePos; }
            Vector2 _velocity = _prevMousePos - _mousePos;
            _cameraMode = "still";
            _camera.Position += _velocity / _ZOOM_LEVELS[_currentZoomLevel];
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
}
