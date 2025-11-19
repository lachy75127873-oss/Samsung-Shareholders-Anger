using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임화면 UI에 그래프를 그리기 위한 클래스
/// </summary>
public class DrawingGraph : MonoBehaviour
{
    /// <summary>
    /// 그래프가 들어갈 오브젝트를 삽입하는 곳
    /// </summary>
    [Tooltip("그래프가 들어갈 오브젝트를 삽입하는 곳")]
    [SerializeField]
    private RectTransform graphContainer;
}
