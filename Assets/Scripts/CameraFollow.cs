using UnityEngine;

public sealed class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform target;

    [Header("Configuración de Posición")]
    [SerializeField] private float heightOffset = 10f;
    [SerializeField] private float distanceOffset = -10f; // Distancia en Z respecto al player

    [Header("Configuración de Ángulo")]
    [SerializeField] private float xAngle = 45f;

    [Header("Suavizado")]
    [SerializeField] private float smoothSpeed = 0.125f;

    private void Start()
    {
        // Aplicamos la rotación inicial en X solicitada
        transform.rotation = Quaternion.Euler(xAngle, 0, 0);
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // 1. Calculamos la posición deseada basada en la posición del player + offsets
        // Seguimos X y Z del player, aplicamos Y fijo (height) y alejamos en Z (distance)
        Vector3 desiredPosition = new Vector3(
            target.position.x,
            target.position.y + heightOffset,
            target.position.z + distanceOffset
        );

        // 2. Interpolamos para un movimiento suave (opcional, pero recomendado)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 3. Aplicamos la posición
        transform.position = smoothedPosition;
    }
}