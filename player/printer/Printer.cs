using Godot;

public class Printer : Spatial, IInteractable
{
    private float _openCloseSpeed = 4f;
    private Spatial _gunHolder;
    private Spatial _cameraHolder;
    private Spatial _topBox;
    private bool _isOpen;
    private bool _isClosed;
    private Player _player;
    private bool _isBeingUsed;
    private bool _isGunInHolder;
    private bool _isPlayerExiting;
    private Vector2 _lastMousePos = new Vector2();


    public override void _Ready()
    {
        _player = GetTree().Root.FindNode("Player", true, false) as Player;

        _gunHolder = GetNode<Spatial>("GunHolder");
        _cameraHolder = GetNode<Spatial>("CameraHolder");
        _topBox = GetNode<Spatial>("Top");
        _isOpen = false;
        _isClosed = true;
        _topBox.Translation = new Vector3(0, 1.5f, 0);
    }

    public override void _Process(float delta)
    {
        if (_isBeingUsed)
        {

            if (_isPlayerExiting)
            {
                GiveGunBackToPlayer(delta);
            }
            else if (!_isGunInHolder)
            {
                PutGunInHolder(delta);
            }
            else
            {

                if (Input.IsMouseButtonPressed((int)ButtonList.Left))
                {
                    var gun = _gunHolder.GetNode<Gun>("Gun");

                    Vector2 currentMousePos = GetViewport().GetMousePosition();
                    Vector2 mouseDelta = currentMousePos - _lastMousePos;
                    _lastMousePos = currentMousePos; // Update the last known mouse position
                    Vector3 rotationDelta = new Vector3(-mouseDelta.y, mouseDelta.x, 0) * delta;
                    Vector3 rotationRadians = rotationDelta * Mathf.Pi / 180;

                    var rotateSpeed = 600f;
                    gun.RotateZ(rotationRadians.x * delta * rotateSpeed * 2);
                    gun.RotateY(rotationRadians.y * delta * rotateSpeed);
                }
                _lastMousePos = GetViewport().GetMousePosition();

                if (Input.IsActionJustPressed("interact"))
                {
                    _isPlayerExiting = true;
                    _isGunInHolder = false;

                    var gun = _gunHolder.GetNode<Gun>("Gun");
                    ReparentNode(gun, _player.GetNode<Spatial>("Head/GunHolder"));

                    var camera = _cameraHolder.GetNode<Camera>("Camera");
                    ReparentNode(camera, _player.GetNode<Spatial>("Head"));

                    Input.MouseMode = Input.MouseModeEnum.Captured;
                }
            }
        }
        else
        {
            OpenAndCloseUpdate(delta);
        }
    }

    public string GetInteractText()
    {
        return "Press E to use printer";
    }

    public void Interact(Player player)
    {
        var gun = player.GetNode<Gun>("Head/GunHolder/Gun");
        ReparentNode(gun, _gunHolder);

        var camera = player.GetNode<Camera>("Head/Camera");
        ReparentNode(camera, _cameraHolder);

        _isBeingUsed = true;
        _player.IsUsingPrinter = true;
        _player.SetInteractText("Press E to exit printer");
    }

    public void PutGunInHolder(float delta)
    {
        var gun = _gunHolder.GetNode<Gun>("Gun");
        var camera = _cameraHolder.GetNode<Camera>("Camera");
        MoveTowardsOrigin(gun, delta * 3f);
        if (gun.Translation.Length() < .01f)
        {
            gun.Translation = Vector3.Zero;
            _isGunInHolder = true;
            Input.MouseMode = Input.MouseModeEnum.Visible;
            camera.Translation = Vector3.Zero;
        }

        MoveTowardsOrigin(camera, delta * 3f);
        if (camera.Translation.Length() < .01f)
        {
            camera.Translation = Vector3.Zero;
        }
    }

    public void GiveGunBackToPlayer(float delta)
    {
        var camera = _player.GetNode<Camera>("Head/Camera");
        var gun = _player.GetNode<Gun>("Head/GunHolder/Gun");
        MoveTowardsOrigin(gun, delta * 6f);
        if (gun.Translation.Length() < .01f)
        {
            gun.Translation = Vector3.Zero;
            Input.MouseMode = Input.MouseModeEnum.Captured;
            camera.Translation = Vector3.Zero;
            _isBeingUsed = false;
            _isGunInHolder = false;
            _isPlayerExiting = false;
            _player.IsUsingPrinter = false;
        }

        MoveTowardsOrigin(camera, delta * 6f);
        if (camera.Translation.Length() < .01f)
        {
            camera.Translation = Vector3.Zero;
        }
    }

    public void MoveTowardsOrigin(Spatial target, float weight)
    {
        Vector3 newPosition = target.GlobalTranslation.LinearInterpolate(target.GetParent<Spatial>().ToGlobal(Vector3.Zero), weight);

        Quat currentRot = new Quat(target.GlobalTransform.basis).Normalized();
        Quat desiredRot = new Quat(target.GetParent<Spatial>().GlobalTransform.basis).Normalized();

        Quat newRotation = currentRot.Slerp(desiredRot, weight);

        target.GlobalTransform = new Transform(newRotation, newPosition);
    }

    public void ReparentNode(Spatial node, Spatial parent)
    {
        var globalTransform = node.GlobalTransform;
        node.GetParent().RemoveChild(node);
        parent.AddChild(node);
        node.GlobalTransform = globalTransform;
    }

    public void OpenAndCloseUpdate(float delta)
    {
        var playerInteractRayHit = _player.InteractRayCast();
        var node = playerInteractRayHit.Count == 0 ? null : playerInteractRayHit["collider"] as Node;
        if (node == this)
        {
            _isClosed = false;
            if (!_isOpen)
            {
                _topBox.Translate(Vector3.Up * _openCloseSpeed * delta);
                if (_topBox.Translation.y >= 2.5f)
                {
                    _topBox.Translation = new Vector3(0, 2.5f, 0);
                    _isOpen = true;
                }
            }
        }
        else
        {
            _isOpen = false;
            if (!_isClosed)
            {
                _topBox.Translate(Vector3.Down * _openCloseSpeed * delta);
                if (_topBox.Translation.y <= 1.5f)
                {
                    _topBox.Translation = new Vector3(0, 1.5f, 0);
                    _isClosed = true;
                }
            }
        }
    }
}