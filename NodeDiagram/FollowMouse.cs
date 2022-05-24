using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class FollowMouse : MonoBehaviour
{
	private RectTransform rtc;
	public void Start()
	{
		rtc = GetComponent<RectTransform>();
	}
	public void Update()
    {
		rtc.position = Input.mousePosition;
    }
}
