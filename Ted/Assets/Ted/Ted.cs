using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
namespace kenbu.Ted
{
    
    public class Ted : MonoBehaviour, ITimeScaleContainer
    {
        private TimeScaleContainer _container;
        private TimeScaleContainer Container {
            get {
                if (_container == null) {
                    _container = new TimeScaleContainer ();
                }
                return _container;
            }
        }

        public void Add(ITimeScalablable c){
            Container.Add (c);
        }

        public void Remove(ITimeScalablable c){
            Container.Remove (c);
        }

        public void MarkAndSweep(){
            Container.MarkAndSweep ();
        }

        public bool IsRelease {
            get {
                throw new NotImplementedException ();
            }
            set {
                throw new NotImplementedException ();
            }
        }

        public List<ITimeScalablable> Children{ 
            get { 
                return Container.Children;
            }
        }

        public void Pause ()
        {
            Container.Pause ();
        }
        public void Resume ()
        {
            Container.Resume ();
        }
        public bool IsActive
        { 
            get { 
                return Container.IsActive; 
            } 
        }
        public float TimeScale {
            get {
                return Container.TimeScale;
            }
            set {
                Container.TimeScale = value;
            }
        }
        public float ParentTimeScale {
            get {
                return Container.ParentTimeScale;
            }
            set {
                Container.ParentTimeScale = value;
            }
        }
        public bool IsAutoRelease {
            get {
                return Container.IsAutoRelease;
            }
            set {
                Container.IsAutoRelease = value;
            }
        }

            
    }

    /// <summary>
    /// ITimeScalable格納するやつ
    /// </summary>
    public class TimeScaleContainer: ITimeScalablable, ITimeScaleContainer
    {

        public List<ITimeScalablable> Children{ get; protected set;}


        public void Add(ITimeScalablable child){
            child.TimeScale = TimeScale;
            if (isPause) {
                child.Pause ();
            }

            if (Children == null) {
                Children = new List<ITimeScalablable>() ;
            }
            Children.Add (child);
        }

        public void Remove (ITimeScalablable c)
        {
            Children.Remove (c);
        }

        public void MarkAndSweep(){
            foreach (var child in Children) {
                child.MarkAndSweep ();
            }
            Children.RemoveAll (checkRemove);

            if (IsAutoRelease && IsActive) {
                IsRelease = true;
            }
        }

        private static bool checkRemove(ITimeScalablable child){
            return child.IsRelease;
        }
            

        private float _timeScale = 1.0f;
        public float TimeScale{ 
            get{ 
                return _timeScale;
            }
            set{ 
                _timeScale = value;
                foreach (var child in Children) {
                    child.ParentTimeScale = _timeScale * _parentTimeScale;
                }
            }
        }
        private float _parentTimeScale = 1.0f;

        public float ParentTimeScale { 
            get { 
                return _parentTimeScale;
            }
            set { 
                _parentTimeScale = value;
                TimeScale = TimeScale;
            }
        }

        private bool isPause;
        public void Pause(){
            isPause = true;
            foreach (var child in Children) {
                child.Pause ();
            }

        }
        public void Resume(){
            isPause = false;
            foreach (var child in Children) {
                child.Resume ();
            }
        }
        public bool IsActive {
            get { 
                foreach (var child in Children) {
                    if (child.IsActive) {
                        return true;
                    }
                }
                return false;
            }
        }
        public bool IsRelease{ get; set;}

        public bool IsAutoRelease{ get; set;}

    }

    public interface ITimeScaleContainer: ITimeScalablable{
        void Add(ITimeScalablable c);
        void Remove(ITimeScalablable c);
        List<ITimeScalablable> Children{ get;}
    }

    /// <summary>
    /// DoTween用wrapper
    /// </summary>
    public class TimeScalablableTween: TimeScalablable{

        public static TimeScalablableTween Wrap(Tween t, bool isAutoRelease = true){
            var result = new TimeScalablableTween (t);
            result.IsAutoRelease = isAutoRelease;
            return result;
        }

        private Tween _tween;
        public TimeScalablableTween(Tween tween){
            _tween = tween;
        }

        public Tween Tween(){
            return _tween;
        }

        public override float TimeScale{ 
            get{ 
                return _timeScale;
            }
            set{ 
                base.TimeScale = value;
                _tween.timeScale = _parentTimeScale * _timeScale;
            }
        }

        public override void Pause(){
            _tween.Pause ();
        }
        public override void Resume(){
            _tween.Restart ();
        }
        public override bool IsActive{ 
            get { 
                return _tween.IsActive (); 
            } 
        }

    }
    /// <summary>
    /// DoTween用wrapper
    /// </summary>
    public class TimeScalablableAnimation: TimeScalablable{

        public static TimeScalablableAnimation Wrap(Animation t){
            var result = new TimeScalablableAnimation (t);
            return result;
        }

        private Animation _animation;
        public TimeScalablableAnimation(Animation animation){
            _animation = animation;
        }

        public Animation Animation(){
            return _animation;
        }

        public override float TimeScale{ 
            get{ 
                return _timeScale;
            }
            set{ 
                base.TimeScale = value;
                var t = _parentTimeScale * _timeScale;
                foreach (AnimationState state in _animation) {
                    state.speed = t;
                }
            }
        }

        public override void Pause(){
            _animation.DOPause ();
        }
        public override void Resume(){
            _animation.DORestart ();
        }
        public override bool IsActive{ 
            get { 
                return _animation.isActiveAndEnabled;
            } 
        }

    }


    public class TimeScalablable: ITimeScalablable{

        protected float _timeScale = 1.0f;
        public virtual float TimeScale{ 
            get{ 
                return _timeScale;
            }
            set{ 
                _timeScale = value;
            }
        }
        protected float _parentTimeScale = 1.0f;

        public float ParentTimeScale { 
            get { 
                return _parentTimeScale;
            }
            set { 
                _parentTimeScale = value;
                TimeScale = TimeScale;
            }
        }

        public virtual void Pause(){
        }
        public virtual void Resume(){
        
        }
        public virtual bool IsActive{ get;}
        public bool IsAutoRelease{ get; set;}

        public bool IsRelease{ get; set;}

        public void MarkAndSweep(){
            if (IsAutoRelease && !IsActive) {
                IsRelease = true;
            }

        }

    }




    public interface ITimeScalablable{

        float TimeScale{ get; set;}
        float ParentTimeScale{ get; set;}
        void Pause();
        void Resume();
        bool IsActive{ get; }
        bool IsAutoRelease{ get; set;}
        bool IsRelease{ get; set;}
        void MarkAndSweep();

    }

    //対DoTween
    //対Animator
    //
}