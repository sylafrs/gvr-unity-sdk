using UnityEngine;
using System.Collections;

public class FadeCamera : MonoBehaviour {
	
	public float distance = -1;

#if UNITY_EDITOR
	public bool preview;
#endif

	public Material fadeMaterial = null;
	public static bool IsFading { get; private set; }
	public static float Alpha { get; private set; } // For the last fadeMaterial !!

	private static IEnumerator LastRoutine;

	public static void SetCurrent(Material material, Color color, float alpha)
	{
		Alpha = alpha;
		color.a = alpha;
		material.color = color;
		LastRoutine = null;
	}

	public static YieldInstruction FadeCurrent(MonoBehaviour runner, Material material, Color color, float alphaEnd, float duration)
	{
		if (material.color.a == alphaEnd)
			return null;

		LastRoutine = FadeEnum(material, color, material.color.a, alphaEnd, duration);
		return runner.StartCoroutine(LastRoutine);
	}

	public static YieldInstruction Fade(MonoBehaviour runner, Material material, Color color, float alphaStart, float alphaEnd, float duration)
	{
		LastRoutine = FadeEnum(material, color, alphaStart, alphaEnd, duration);
		return runner.StartCoroutine(LastRoutine);
	}

	static IEnumerator FadeEnum(Material material, Color color, float alphaStart, float alphaEnd, float duration)
	{
		IEnumerator thisRoutine = LastRoutine;
		IsFading = true;

		float alpha = alphaStart;
		Alpha = alpha;

		color.a = alphaStart;
		material.color = color;

		float speed = (alphaEnd - alphaStart) / duration;
		float t = 0;

		while (t < duration)
		{
			yield return new WaitForEndOfFrame();
			if (thisRoutine != LastRoutine)
				yield break;

			t		+= Time.deltaTime;
			alpha	+= Time.deltaTime * speed;
			color.a = alpha;
			material.color = color;

			Alpha = alpha;
		}

		color.a = alphaEnd;
		material.color = color;

		Alpha = alphaEnd;

		IsFading = (alphaEnd != 0f);
	}

	/// <summary>
	/// Renders the fade overlay when attached to a camera object
	/// </summary>
	void OnPostRender()
	{
#if UNITY_EDITOR
		if(IsFading || preview)
#else
		if (IsFading)
#endif
		{
			fadeMaterial.SetPass(0);
			GL.PushMatrix();
			GL.LoadOrtho();
			GL.Color(fadeMaterial.color);
			GL.Begin(GL.QUADS);
			GL.Vertex3(0f, 0f, distance);
			GL.Vertex3(0f, 1f, distance);
			GL.Vertex3(1f, 1f, distance);
			GL.Vertex3(1f, 0f, distance);
			GL.End();
			GL.PopMatrix();
		}
	}
}