using System.Collections.Generic;
using UnityEngine;

public class CameraObstacleFade : MonoBehaviour
{
	[SerializeField] private GameObject player;
	[SerializeField] private LayerMask obstacleLayer;
	[SerializeField] private Camera cam;
	[SerializeField] private Dictionary<Renderer, Material[]> original;
	[SerializeField] private List<Renderer> obstacles;

	[Range(0f, 1f)] public float fadeAlpha = 0.25f;

	static readonly int LeafTintId = Shader.PropertyToID("_LeafColourTint"); // 셰이더에 따라 이름 다를 수 있음
	static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
	static readonly int ColorId = Shader.PropertyToID("_Color");

	readonly List<Renderer> _current = new();
	readonly Dictionary<Renderer, Color> _originalColor = new();
	readonly MaterialPropertyBlock _mpb = new();

	private void Awake()
	{
		original = new Dictionary<Renderer, Material[]>();
		obstacles = new List<Renderer>();
		cam = Camera.main;
		player= transform.parent.gameObject;
		obstacleLayer = LayerMask.GetMask("Tree");
	}

	private void Start()
	{
		
	}

	void LateUpdate()
	{
		Vector3 from = cam.transform.position;
		Vector3 to = player.transform.position + Vector3.up * 1.5f;

		// 여러 장애물 가능성 고려: RaycastAll 추천
		RaycastHit[] hits = Physics.RaycastAll(from, (to - from).normalized, Vector3.Distance(from, to), obstacleLayer);

		RestoreAll();

		for (int i = 0; i < hits.Length; i++)
		{
			var r = hits[i].collider.GetComponentInParent<Renderer>();
			if (r == null) continue;

			FadeRenderer(r, fadeAlpha);
		}
	}

	void FadeRenderer(Renderer r, float a)
	{
		if (!_current.Contains(r)) _current.Add(r);

		// 어떤 색 프로퍼티가 있는지 먼저 찾기
		Material m = r.sharedMaterial;
		int propId = -1;

		if (m != null)
		{
			if (m.HasProperty(LeafTintId)) propId = LeafTintId;
			else if (m.HasProperty(BaseColorId)) propId = BaseColorId;
			else if (m.HasProperty(ColorId)) propId = ColorId;
		}
		if (propId == -1) return;

		r.GetPropertyBlock(_mpb);

		// 최초 1회 원본 색 저장
		if (!_originalColor.ContainsKey(r))
		{
			Color c = m.GetColor(propId);
			_originalColor[r] = c;
		}

		Color origin = _originalColor[r];
		origin.a = a;
		_mpb.SetColor(propId, origin);
		r.SetPropertyBlock(_mpb);
	}

	void RestoreAll()
	{
		for (int i = 0; i < _current.Count; i++)
		{
			var r = _current[i];
			if (r == null) continue;

			r.SetPropertyBlock(null);
		}
		_current.Clear();
		_originalColor.Clear();
	}
}

