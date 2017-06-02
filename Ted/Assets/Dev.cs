using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using System;
using kenbu.Ted;

public class Dev : MonoBehaviour {

    [SerializeField]
    private Button _button1;
    [SerializeField]
    private Button _button2;

    [SerializeField]
    private Transform[] _cubeList;

    [SerializeField]
    private Animation _animation;

    [SerializeField]
    private Ted _ted;

	// Use this for initialization
	void Awake () {
        //通常トゥイーン
        _ted.Add (TimeScalablableTween.Wrap (transform.DOScale (Vector3.one* 2.0f, 10.0f), true));

        //ループなトゥイーン
        foreach (var cube in _cubeList) {
            var v = new Vector3 (UnityEngine.Random.Range(-5.0f, 5.0f), UnityEngine.Random.Range(-5.0f, 5.0f), UnityEngine.Random.Range(-5.0f, 5.0f));
            cube.localPosition = v;
            v *= -1;
            var t = cube.DOLocalMove (v, 1.0f).SetEase (Ease.InOutQuad).SetLoops (-1, LoopType.Yoyo);
            _ted.Add (TimeScalablableTween.Wrap (t));
        }
        //Animation
        _ted.Add (TimeScalablableAnimation.Wrap (_animation));


        _button1.onClick.AddListener (()=>{
            _ted.Children[0].TimeScale = 1;
            _ted.TimeScale = 1.0f;

        });
        _button2.onClick.AddListener (()=>{
            _ted.Children[0].TimeScale = 20;
            _ted.TimeScale = 0.05f;
        });
    }
	
	// Update is called once per frame
	void Update () {
        _ted.MarkAndSweep ();
    }
}

