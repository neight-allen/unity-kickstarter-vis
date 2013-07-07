using UnityEngine;
using System.Collections;
using System;

public class Easing {

    public static float TimeScale = 1f;
	
	private Vector3 v3_start;
    private Vector3 v3_stop;
	
	private float f_start;
	private float f_stop;
	
	private DateTime dt_start;
	private DateTime dt_stop;

    private float f_duration;
    private float f_progress;
    private float f_lerp = 1;
    private float f_lastTime;

    public bool finished = false;
    public bool started = false;
	
	public float myTimeScale = 1f;

    private EaseType et_type;
	
	[SerializeField]
	public enum EaseType {
		Bounce,
		Ease,
		EasyEase,
		EaseIn,
		Back,
		Elastic,
		Linear
	};

    public Vector3 Vector3{
        get
        {
            if (finished) return v3_stop;

            if(!started){
                started = true;
                f_lastTime = Time.time;
            }

            Update();
			
			if(f_lerp > 1)
				return (v3_stop - v3_start) * f_lerp + v3_start;
			else
            	return Vector3.Lerp(v3_start, v3_stop, f_lerp);
        }
        set{}
    }
	
	public Vector3 EulerAngles{
		get
		{
			if (finished) return v3_stop;

            if(!started){
                started = true;
                f_lastTime = Time.time;
            }

            Update();
			
			Vector3 returnVector = new Vector3();
			returnVector.x = Mathf.LerpAngle(v3_start.x, v3_stop.x, f_lerp);
			returnVector.y = Mathf.LerpAngle(v3_start.y, v3_stop.y, f_lerp);
			returnVector.z = Mathf.LerpAngle(v3_start.z, v3_stop.z, f_lerp);
			
			return returnVector;
		}
		set{}
	}
	
	public float Float{
        get
        {
            if(finished) return f_stop;

            if(!started){
                started = true;
                f_lastTime = Time.time;
            }

            Update();			

            return Mathf.Lerp(f_start, f_stop, f_lerp);
        }
        set{}
    }
	
	public DateTime DateTime{
		get{
			if(finished) return dt_stop;

            if(!started){
                started = true;
                f_lastTime = Time.time;
            }

            Update();
			
			double l_lerp = (double)(dt_stop.Ticks - dt_start.Ticks) * (double)f_lerp + (double)dt_start.Ticks;

            return new DateTime((long)l_lerp);
		}
		set{}
	}
		
		
		
	public Easing(EaseType type, float duration)
	{
		et_type = type;
        f_duration = duration;
        f_progress = 0;
		f_start = 0;
		f_stop = 1;
        started = false;
        finished = false;
	}
	
	public Easing(EaseType type, float start, float stop, float duration)
    {
		et_type = type;
        f_start = start;
        f_stop = stop;
        f_duration = duration;
        f_progress = 0;
        started = false;
        finished = false;
    }
    
	
	public Easing(EaseType type, Vector3 start, Vector3 stop, float duration)
    {
		et_type = type;
        v3_start = start;
        v3_stop = stop;
		f_start = 0;
		f_stop = 1;
        f_duration = duration;
        f_progress = 0;
        started = false;
        finished = false;
    }
	
	public Easing(EaseType type, DateTime start, DateTime stop, float duration)
    {
		et_type = type;
        dt_start = start;
        dt_stop = stop;
		f_start = 0;
		f_stop = 1;
        f_duration = duration;
        f_progress = 0;
        started = false;
        finished = false;
    }
	
	

    void Awake()
    {

    }

    void Start()
    {

    }

    public void Reset()
    {
        started = false;
        finished = false;
        f_progress = 0;
    }

    void Update()
    {
        //print(v3_stop);
        if (started && !finished)
        {
            f_progress += (Time.time - f_lastTime) * Easing.TimeScale * myTimeScale;
            f_lastTime = Time.time;

            if (f_progress >= f_duration)
            {
                finished = true;
                f_progress = f_duration;
            }

            switch (et_type)
            {
                case EaseType.Ease:
                    f_lerp = quinticOut(f_progress / f_duration);
                    break;
				case EaseType.EaseIn:
                    f_lerp = quinticIn(f_progress / f_duration);
                    break;
				case EaseType.EasyEase:
                    f_lerp = (easeOut(f_progress / f_duration) * 2 + (f_progress / f_duration)) / 3;
                    break;
                case EaseType.Back:
                    f_lerp = backOut(f_progress / f_duration);
                    break;
                case EaseType.Bounce:
                    f_lerp = bounceOut(f_progress / f_duration);
                    break;
				case EaseType.Elastic:
					f_lerp = elasticOut(f_progress / f_duration);
					break;
                case EaseType.Linear:
                default:
                    f_lerp = f_progress / f_duration;
                    break;
            }
        }

    }

    private float backOut(float t)
    {
        if (t >= 1) return 1;
        float bounce = 1.70158f;
		return (t=t-1) * t * ((bounce+1)*t + bounce) + 1;

    }

    private float backOut(float t, float bounce)
    {
        if (t >= 1) return 1;
        return (t=t-1) * t * ((bounce+1)*t + bounce) + 1;
    }


    private float easeOut(float t)
    {
        //return (t > 1) ? 1 : (float)(-Mathf.Pow(2, -10 * t) + 1);
		return ( t >= 1 ) ? 1 : (float)(-Mathf.Pow( 2, -15 * t ) + 1 );
		return (float)(-Mathf.Pow(2, -10 * t) + 1);
    }
	
	private float quinticOut(float t)
	{
		--t; return (t * t * t * t * t+ 1f);
	}
	
	private float quinticIn(float t)
	{
		return t*t*t*t*t;
	}
	
	private float easeIn(float t)
    {
        //return (t > 1) ? 1 : (float)(-Mathf.Pow(2, -10 * t) + 1);
		return ((1 - easeOut(1-t)) + 1.5f * t) / 2.5f;
		
    }
	

    private float bounceOut(float t)
    {

        if (t > 1)
            return 1;

        else if (t < (1 / 2.75f))
            return 7.5625f * t * t;

        else if (t < (2 / 2.75))
            return 7.5625f * (t -= (1.5f / 2.75f)) * t + .75f;

        else if (t < (2.5 / 2.75))
            return 7.5625f * (t -= (2.25f / 2.75f)) * t + .9375f;

        else
            return 7.5625f * (t -= (2.625f / 2.75f)) * t + .984375f;

    }
	
	private float elasticOut(float t) {
		// t: current time, b: beginning value, c: change in value, d: duration, a: amplitude (optional), p: period (optional)
		// t and d can be in frames or seconds/milliseconds
		if (t==0) return 0;  
		if (t==1) return 1;  
		float p=.4f;
		float a=2;
		float s;
		if (a < 1) 
		{
			a = 1; 
			s = p / 4f;
		}
		else 
			s = p/(2*Mathf.PI) * Mathf.Asin(1 / a);
			
		return a * Mathf.Pow(2,-10*t) * Mathf.Sin( (t-s) * (2*Mathf.PI) / p ) + 1f;
	}

    
}
