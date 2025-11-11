using UnityEngine;

/// <summary>
/// Definit l angle d inclinaison de la pointe de mon canon afin de suivre celle de mon pointeur au sol
/// Source - explication du pitch : https://www.youtube.com/watch?v=hOgGJ8UoTWw
/// Source - Quaternion.LookRotation : https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Quaternion.LookRotation.html
/// Source - Quaternion.RotateTowards : https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Quaternion.RotateTowards.html
/// <remarks>
/// Code généré par : OpenAI. (2025). ChatGPT (version 5 août 2025) [Modèle massif 
/// de langage]. https://openai.com/
/// </remarks>
/// </summary>
public class ViseeTourelle : MonoBehaviour
{
    public PointeurAuSol pointeur;    // Pour obtenir la cible au sol
    public Transform canon;            // Pivot du canon
    public float vitesseRotation = 180f;
    public Vector2 limitesPitch = new Vector2(-10f, 30f);

    /// <summary>
    /// Une méthode permettant de faire pivoter la tourelle du tank pour viser le point au sol indiqué par le pointeur.
    /// </summary>
    void LateUpdate()
    {
        if (pointeur == null) return;

        if (pointeur.AvoirPointAuSol(out Vector3 cible))
        {
            // Rotation de la tourelle en axe des y
            Vector3 pos = transform.position;
            Vector3 direction = new Vector3(cible.x, pos.y, cible.z) - pos;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion rotationVoulue = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationVoulue, vitesseRotation * Time.deltaTime);
            }

            // Inclinaison (pitch) du canon
            if (canon != null)
            {
                Vector3 dirCanon = cible - canon.position;
                float pitchAngle = Quaternion.LookRotation(dirCanon).eulerAngles.x;

                if (pitchAngle > 180f) pitchAngle -= 360f;
                pitchAngle = Mathf.Clamp(pitchAngle, limitesPitch.x, limitesPitch.y);

                Vector3 e = canon.localEulerAngles;
                canon.localEulerAngles = new Vector3(pitchAngle, e.y, e.z);
            }
        }
    }
}
