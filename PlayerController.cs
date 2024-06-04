using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    //MultiplayerData
    public NetworkVariable<int> VehicleType = new NetworkVariable<int>();

    //LocalPlayerData
    public float CameraSensitivity = 1f;
    Vector2 mouseLook;
    public float CameraZoom=5;
    bool FreeLookBool;

    //VehicleTypeData
    public float MaxSpeed = 10f;
    public float MaxAcceleration = 5f;
    public float MaxTurnAngle = 0.4f;
    public float TurnRate = 1f;
    public float TurretTurnRate = 25;

    //VehicleLocalData
    public float CurrentTurnAngle = 0f;
    public float PlayerTurnwheelAngle = 0f;
    public float CurrentSpeed = 0f;
    public float CurrentAcceleration = 0f;
    public Vector2 movementVector;
    public float TurretY;
    public float CameraY;
    public Vector3 NextPosition = new Vector3(0, 0, 0);
    Quaternion CameraRotation;

    void Start()
    {
        if (IsOwner)
        {
            this.gameObject.transform.Find("PlayerCameraControlObject").transform.Find("PlayerCamera").GetComponent<Camera>().enabled = true;
            this.gameObject.transform.Find("PlayerCameraControlObject").transform.Find("PlayerCamera").GetComponent<AudioListener>().enabled = true;
        }
        if (IsHost&&IsOwner)
        {
            this.gameObject.name = "Host";
        }
    }
    
    public void Move(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        movementVector = context.ReadValue<Vector2>();
    }

    public void Look(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
            mouseLook = context.ReadValue<Vector2>();
            float mouseX = mouseLook.x * CameraSensitivity * Time.deltaTime * 5;
            float mouseY = mouseLook.y * CameraSensitivity * Time.deltaTime * 5;

            //Z axis - left/right
            this.gameObject.transform.Find("PlayerCameraControlObject").transform.rotation = Quaternion.AngleAxis(mouseX, Vector3.up) * this.gameObject.transform.Find("PlayerCameraControlObject").transform.rotation;

            //X axis - up/down
            if (this.gameObject.transform.Find("PlayerCameraControlObject").transform.localRotation.eulerAngles.x <= 70 || this.gameObject.transform.Find("PlayerCameraControlObject").transform.localRotation.eulerAngles.x >= 315)
            {
                this.gameObject.transform.Find("PlayerCameraControlObject").transform.rotation *= Quaternion.Euler(-mouseY, 0, 0);
            }

            else if ((this.gameObject.transform.Find("PlayerCameraControlObject").transform.localRotation.eulerAngles.x > 310) && (mouseY < 0))
            {
                this.gameObject.transform.Find("PlayerCameraControlObject").transform.localRotation = Quaternion.Euler(-45, this.gameObject.transform.Find("PlayerCameraControlObject").transform.localRotation.eulerAngles.y, 0);
            }

            else if ((this.gameObject.transform.Find("PlayerCameraControlObject").transform.localRotation.eulerAngles.x < 90) && (mouseY > 0))
            {
                this.gameObject.transform.Find("PlayerCameraControlObject").transform.localRotation = Quaternion.Euler(70, this.gameObject.transform.Find("PlayerCameraControlObject").transform.localRotation.eulerAngles.y, 0);
            }

    }

    public void FreeLook(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        int freeLook = (int)context.ReadValue<float>();
        if (freeLook == 1)
        {
            FreeLookBool = true;
        }
        else
        {
            FreeLookBool = false;
        }
        if (freeLook == 0)
        {
            this.gameObject.transform.Find("PlayerCameraControlObject").transform.localRotation = this.gameObject.transform.Find("Model").transform.Find("Turret").transform.localRotation;
        }
    }

    public void Zoom(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        CameraZoom -= context.ReadValue<float>() * Time.deltaTime/2;
        if (CameraZoom > 10)
        {
            CameraZoom = 10;
        }
        if (CameraZoom < 1)
        {
            CameraZoom = 1;
        }
        this.gameObject.transform.Find("PlayerCameraControlObject").transform.Find("PlayerCamera").gameObject.transform.localPosition = new Vector3(0, 0, -CameraZoom);
    }

    Quaternion TurnWheelsFront()
    {
        Debug.Log("TurningWheels");
        if (PlayerTurnwheelAngle <= 45 && PlayerTurnwheelAngle >= -45)
        {
            PlayerTurnwheelAngle += movementVector.x * Time.deltaTime * 90;
        }
        else if (PlayerTurnwheelAngle > 45)
        {
            PlayerTurnwheelAngle = 45;
        }
        else
        {
            PlayerTurnwheelAngle = -45;
        }
        if (CurrentSpeed != 0 && movementVector.x == 0)
        {
            if (Mathf.Abs(PlayerTurnwheelAngle) < 0.1f)
            {
                PlayerTurnwheelAngle = 0;
            }
            else if (PlayerTurnwheelAngle > 0)
            {
                PlayerTurnwheelAngle -= Time.deltaTime * 20;
            }
            else
            {
                PlayerTurnwheelAngle += Time.deltaTime * 20;
            }
        }
        return Quaternion.Euler(0, PlayerTurnwheelAngle, this.gameObject.transform.Find("Model").transform.Find("FrontL").transform.localRotation.eulerAngles.z); //TURN FRONT WHEELS

    }
    Quaternion TurnModel()
    {
        if (Mathf.Abs(CurrentSpeed) > 0.2f)
        {
            if (CurrentSpeed > 0)
            {
                return Quaternion.Euler(0, PlayerTurnwheelAngle * Time.deltaTime, 0);
            }
            else if (CurrentSpeed < 0)
            {
                return Quaternion.Euler(0, -PlayerTurnwheelAngle * Time.deltaTime, 0);
            }
            else return Quaternion.Euler(0, 0, 0);
        }
        else return Quaternion.Euler(0, 0, 0);
    }
    void Accelerate() {
        //Acceleration
        if (CurrentAcceleration >= -MaxAcceleration && CurrentAcceleration <= MaxAcceleration && movementVector.y != 0)
        {
            CurrentAcceleration += 5 * movementVector.y * MaxAcceleration * Time.deltaTime;
        }
        else if (movementVector.y == 0)
        {
            if (CurrentAcceleration > 0.01f)
            {
                CurrentAcceleration -= MaxAcceleration * Time.deltaTime;
            }
            else if (CurrentAcceleration < -0.01f)
            {
                CurrentAcceleration += MaxAcceleration * Time.deltaTime;
            }
            else if (Mathf.Abs(CurrentAcceleration) < 0.01f)
            {
                CurrentAcceleration = 0;
            }
        }
        else if (CurrentAcceleration > MaxAcceleration && movementVector.y != 0)
        {
            CurrentAcceleration = MaxAcceleration;
        }
        else if (CurrentAcceleration < -MaxAcceleration && movementVector.y != 0)
        {
            CurrentAcceleration = -MaxAcceleration;
        }
    }
    void Velocity()
    {
        //Velocity
        if (Mathf.Abs(CurrentSpeed) < 0.1f && CurrentAcceleration == 0)
        {
            CurrentSpeed = 0;
        }
        if (CurrentSpeed > -MaxSpeed / 2 && CurrentSpeed < MaxSpeed && CurrentAcceleration != 0)
        {
            CurrentSpeed += CurrentAcceleration * Time.deltaTime;
        }
        if (CurrentSpeed > 0)
        {
            CurrentSpeed -= MaxSpeed / 5 * Time.deltaTime;
        }
        if (CurrentSpeed < 0)
        {
            CurrentSpeed += MaxSpeed / 5 * Time.deltaTime;
        }
    }
    Quaternion TurnWheels()
    {
        return Quaternion.Euler(0, 0, CurrentSpeed / 0.4f);
    }
    float CalculateSickAngles(float turretY)
    {
        if (this.gameObject.transform.Find("PlayerCameraControlObject").transform.localRotation.eulerAngles.y < 90 || this.gameObject.transform.Find("PlayerCameraControlObject").transform.localRotation.eulerAngles.y > 270)
        {
            if (turretY > 180)
            {
                turretY = turretY - 360;
                return turretY;
            }
            else return turretY;
        }
        else return turretY;
    }
    Quaternion MooveTurret()
    {
        TurretY = transform.Find("Model").transform.Find("Turret").transform.localRotation.eulerAngles.y;
        CameraY = transform.Find("PlayerCameraControlObject").transform.localRotation.eulerAngles.y;
        TurretY = CalculateSickAngles(TurretY);
        CameraY = CalculateSickAngles(CameraY);
        if (Mathf.Abs(this.gameObject.transform.Find("Model").transform.Find("Turret").transform.localRotation.eulerAngles.y - this.gameObject.transform.Find("PlayerCameraControlObject").transform.localRotation.eulerAngles.y) > 1)
        {
            if (TurretY < CameraY)
            {
                return Quaternion.Euler(0, TurretTurnRate * Time.deltaTime, 0);
            }
            else
            {
                return Quaternion.Euler(0, -TurretTurnRate * Time.deltaTime, 0);
            }
        }
            else
            {
                if (TurretY < CameraY)
                {
                    return Quaternion.Euler(0, -TurretTurnRate * Time.deltaTime, 0);
                }
                else
                {
                    return Quaternion.Euler(0, -TurretTurnRate * Time.deltaTime, 0);
                }
            }

    }
    Quaternion MooveGun()
    {
        if (this.gameObject.transform.Find("PlayerCameraControlObject").transform.localRotation.eulerAngles.x > 315 || this.gameObject.transform.Find("PlayerCameraControlObject").transform.localRotation.eulerAngles.x < 10)
        {
            return Quaternion.Euler(0, 0, this.gameObject.transform.Find("PlayerCameraControlObject").transform.localRotation.eulerAngles.x);
        }
        else if(this.gameObject.transform.Find("PlayerCameraControlObject").transform.localRotation.eulerAngles.x > 10&& this.gameObject.transform.Find("PlayerCameraControlObject").transform.localRotation.eulerAngles.x<180) return Quaternion.Euler(0, 0, 10);
        else return Quaternion.Euler(0, 0, 315);

    }
    void UpdateMovementHost()
    {
        transform.Find("Model").transform.Find("Turret").transform.localRotation *= MooveTurret();
        transform.Find("Model").transform.Find("Turret").transform.Find("Gun").transform.localRotation = MooveGun();
        transform.Find("Model").transform.Find("FrontL").transform.localRotation = TurnWheelsFront();
        transform.Find("Model").transform.Find("FrontR").transform.localRotation = TurnWheelsFront();
        transform.rotation *= TurnModel();
        Accelerate();
        transform.Find("Model").transform.Find("WheelsBack").transform.localRotation *= TurnWheels();
        transform.Find("Model").transform.Find("WheelsM1").transform.localRotation *= TurnWheels();
        transform.Find("Model").transform.Find("WheelsM2").transform.localRotation *= TurnWheels();
        transform.Find("Model").transform.Find("FrontL").transform.localRotation *= TurnWheels();
        transform.Find("Model").transform.Find("FrontR").transform.localRotation *= TurnWheels();

    }

    void FixedUpdate()
    {
        if (!IsOwner) { return; }
        Accelerate();
        Velocity();
        MoovePlayerServerRpc(TurnWheelsFront(), CurrentSpeed, CurrentAcceleration, TurnModel(), TurnWheels(), MooveTurret(), MooveGun(), FreeLookBool);
    }

    [ServerRpc]
    public void MoovePlayerServerRpc(Quaternion FrontWheelsTurn, float velocity, float acceleration, Quaternion modelturn, Quaternion wheelturn, Quaternion turretrotation, Quaternion gunrotation, bool freelookbool)
    {
        transform.Find("Model").transform.Find("FrontL").transform.localRotation = FrontWheelsTurn;
        transform.Find("Model").transform.Find("FrontR").transform.localRotation = FrontWheelsTurn;
        transform.position += transform.rotation * Vector3.forward * velocity * Time.deltaTime;
        transform.rotation *= modelturn;

        transform.Find("Model").transform.Find("WheelsBack").transform.localRotation *= wheelturn;
        transform.Find("Model").transform.Find("WheelsM1").transform.localRotation *= wheelturn;
        transform.Find("Model").transform.Find("WheelsM2").transform.localRotation *= wheelturn;
        transform.Find("Model").transform.Find("FrontL").transform.localRotation *= wheelturn;
        transform.Find("Model").transform.Find("FrontR").transform.localRotation *= wheelturn;

        if (acceleration != 0)
        {
            transform.Find("Model").transform.Find("PENIFV").transform.localRotation = Quaternion.Euler(0, 0, -acceleration/2);
        }
        if (freelookbool == false)
        {
            transform.Find("Model").transform.Find("Turret").transform.localRotation *= turretrotation;
            transform.Find("Model").transform.Find("Turret").transform.Find("Gun").transform.localRotation = gunrotation;
        }

    }

}
